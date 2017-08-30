using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveList : MonoBehaviour {
	//link to the battle ui being used by this, needed so that other elements can be enabled when movelist is active
	public BattleUI battleUI;

	//the prefab to make all ability buttons from
	public AbilityButton abilityButtonTemplate;
	//the prefab to make all category link buttons from, and also the back button
	public CategoryButton categoryButtonTemplate;
	//the prefab to make all other buttons from, such as attack and defend
	public BasicCommandButton basicButtonTemplate;
	//the inactive gameobject to attached pooled ui to
	private GameObject poolHolder;
	//the gameobject to child buttons to
	[SerializeField]
	private GameObject buttonHolder;
	//the mouse unit selector used to select units in world space
	private MouseUnitSelector mouseSelector;

	//pool of currently created ability buttons
	private List<AbilityButton> abilityBtnPool = new List<AbilityButton>();
	//pool for categories of abilities
	private List<CategoryButton> categoryBtnPool = new List<CategoryButton>();
	//pool for basic commands buttons
	private List<BasicCommandButton> basicCommandPool = new List<BasicCommandButton>();

	//the list of buttons currently in use on the movelist
	private List<AbilityButton> abilityBtn = new List<AbilityButton>();
	private List<CategoryButton> categoryBtn = new List<CategoryButton>();
	private List<BasicCommandButton> basicCommandBtn = new List<BasicCommandButton>();

	//the button used to exit from targetting mode and return to the move list
	private Button exitTargetButton;

	//the playerbrain currently being used by the movelist
	private PartyMemberBrain storedPlayerBrain = null;

	//the cached state of the playing field
	private UnitController user;
	private UnitController[] leftUnits;
	private UnitController[] rightUnits;
	private UnitController[] leftReserve;
	private UnitController[] rightReserve;

	//the current state of the move list
	private MoveListState state = MoveListState.StartSelect;
	//the state of the list just before targetting mode was entered
	private MoveListState preTargetState = MoveListState.StartSelect;
	//the type of targetting being performed
	private TargetTypeMode targetMode = TargetTypeMode.Single;

	//the animator
	private Animator animator;
	//the animation hashes
	private int MOVE_IN_HASH = Animator.StringToHash("MoveIn");
	private int MOVE_OUT_HASH = Animator.StringToHash("MoveOut");

	//the ability slot of the current ability being targetted with, used to take resources from user when targets are confirmed
	//if this is null it means a free ability is being used
	private AbilitySlot selectedSlot = null;
	//the current ability being targeted with, it must be cast to one of these types of abilities for ease of use with the targetting system
	//these should be kept as null when not in use
	private SingleAbility singleAbilityTarget = null;
	private MultiAbility multiAbilityTarget = null;

	//the currently selected unit (mouse cursor or button highlight is over them)
	private UnitController selectedUnit = null;
	//the currently targeted units (must be checked with check valid target before a target is confirmed)
	private List<UnitController> targetedUnits = new List<UnitController>();

	//the battle camera controller being used for this battle
	//used to move the camera when certain pop up menus appear
	public BattleCameraControl cameraControl;
	//the left field position, used to avoid rendering over units
	public Rect leftField;

	//the background image of the movelist, used to get the width size
	private Image backgroundImage;

	//the rectangle to use for camera framing purposes
	//add to cam hints when list is expanded
	//remove when list is hidden
	private Rect listFramingRect = new Rect();

	//the current targetting mode, for each different targetting type
	//self is used automatically and so doesn't have a mode
	private enum TargetTypeMode {
		Single, SetNumber, Group, All
	}

	//each possible state of the movelist component while active
	private enum MoveListState
	{
		StartSelect, CategorySelect, Targeting
	}

	void Awake() {
		backgroundImage = transform.Find ("MovelistBackground").GetComponent<Image>();
		poolHolder = transform.Find ("ElementPool").gameObject;
		animator = GetComponent<Animator> ();
		exitTargetButton = GetComponentInChildren<Button> ();
		mouseSelector = GetComponent<MouseUnitSelector> ();
		mouseSelector.parentMoveList = this;
	}

	//method to set up the buffer on the left side of the screen for the movelist
	//should be called at the start of the battle after the scenery's camera hints r added
	public void Initialise() {
		//place the move list rect to the left of this and in the centre so that an extra space to the left will be displayed
		listFramingRect.xMax = leftField.xMin;
		//give rectangle no height and place at centre of current frame so it doesnt extend view vertically
		listFramingRect.yMin = leftField.center.y;
		listFramingRect.yMax = leftField.center.y;
		//now calculate how wide the framing rectangle will have to be 
		//the rectangle has to be wide enough to accomidate the pixel width of the movelist
		//this will depend on the size of the camera with the new framing rectangle added and the aspect ratio
		//now set the xmin to be the same as xmax to start with
		float bufferZone = FramingRectWidth();
		listFramingRect.xMin = listFramingRect.xMax - bufferZone;
		//and add it to the hints list
		//cameraControl.hints.keepInFrame.Add(listFramingRect);

		//recursively increase the width of the framing rect until the movelist fits on screen
		/*while (true) {
			//check if the 
			//take the overall framing rectangle
			Rect frameRect = cameraControl.hints.GetOverallFrame();

			float aspect = cameraControl.AttachedCamera.aspect;

			//predict the size and position of the camera using the hints
			Vector3 position;
			float size;
			cameraControl.hints.CalculateTargetCameraConfig (aspect, out size, out position);

			//find distance from the edge of the camera to the leftfield
			float distance = leftField.xMin - (position.x - size);
			//convert to pixels using screen width
			distance = distance * cameraControl.AttachedCamera.pixelWidth / (size * aspect * 2f);
			//if this is greater than or equal to the width then break
			if (distance >= moveListWidth) {
				break;
			}
			listFramingRect.xMin -= 0.3f;

			//otherwise incriment size of rect by an arbitrary amount until a correct distance is found
			cameraControl.hints.keepInFrame[index] = listFramingRect;
		}*/

	}

	//method to display the movelist for the selected user with the selected set of moves from the turnbrain
	//also take in the active units on the left and right for targetting purposes
	//and the reserve units (some rare moves can target reserves)
	public void StartupMoveList(PartyMemberBrain playerBrain, UnitController user, UnitController[] leftUnits, UnitController[] rightUnits,
		UnitController[] leftReserve, UnitController[] rightReserve) {
		//cache the playing field's state
		this.user = user;
		this.leftUnits = leftUnits;
		this.rightUnits = rightUnits;
		this.leftReserve = leftReserve;
		this.rightReserve = rightReserve;

		//and the brain
		storedPlayerBrain = playerBrain;

		//make sure targetting button is disabled
		exitTargetButton.gameObject.SetActive(false);

		//begin on the start menu
		EnterStartMenu ();

		//lastly tell the menu to start animating out from the side using its animation controller
		animator.Play(MOVE_IN_HASH, -1, 0);
		animator.Update (0f);

		//and add the buffer to the hints list
		cameraControl.hints.keepInFrame.Add(listFramingRect);

		//tell the turn order system to pop out
		battleUI.DisplayMoveEssentialUI(user);
	}

	//return the required width of a framing rect for this movelist, designed to move camera to keep units in view when the overlay is active
	private float FramingRectWidth() {
		//get the current overall frame
		Rect overall = cameraControl.hints.GetOverallFrame();
		//get the pixel width of the movelist
		float moveListWidth = backgroundImage.rectTransform.rect.width * backgroundImage.canvas.scaleFactor;
		float aspect = cameraControl.AttachedCamera.aspect;
		//find out the distance from the overall limit to the leftfield limit
		float leftFieldPadding = leftField.xMin - overall.xMin;

		//

		return 3;
	}

	//enter the starting select menu
	public void EnterStartMenu() {
		//clearup any previous state
		ClearHeldButtons ();

		state = MoveListState.StartSelect;

		//setup the basic commands first
		if (storedPlayerBrain.basicAttack != null) {
			//get a basic command button and add it, also set the onclick events
			BasicCommandButton button = GetBasicButton();
			button.transform.SetParent (buttonHolder.transform);
			button.Button.onClick.AddListener (Attack);
			button.SetText ("Attack");
		}
		if (storedPlayerBrain.defend != null) {
			BasicCommandButton button = GetBasicButton();
			button.transform.SetParent (buttonHolder.transform);
			button.Button.onClick.AddListener (Defend);
			button.SetText ("Defend");
		}
		//and then the categories below it
		for (int i = 0; i < storedPlayerBrain.abilityCategories.Count; i++) {
			MoveListCategory category = storedPlayerBrain.abilityCategories [i];
			CategoryButton button = GetCategoryButton ();
			button.transform.SetParent (buttonHolder.transform);
			//wrap call in delegate to pass parameters
			button.Button.onClick.AddListener (delegate{ CategoryButtonClicked(button);});
			button.SetCategory (category);
		}

		//make sure they are interactable
		DefaultEnableButtons();
	}

	//enter the category select menu
	public void EnterCategoryMenu(MoveListCategory category) {
		ClearHeldButtons ();

		state = MoveListState.CategorySelect;

		//set up the ability buttons
		for (int i = 0; i < category.moves.Count; i++) {
			AbilityButton button = GetAbilityButton ();
			button.transform.SetParent (buttonHolder.transform);
			//set the ability of the button, this will also setup the visuals
			button.SetAbility(category.moves[i]);
			button.Button.onClick.AddListener (delegate {
				AbilityButtonClicked (button);
			});
		}
		//and end with a back button
		BasicCommandButton backButton = GetBasicButton();
		backButton.transform.SetParent (buttonHolder.transform);
		backButton.SetText ("Back");
		backButton.Button.onClick.AddListener (Back);

		//make sure they are interactable
		DefaultEnableButtons();
	}

	//logic for pressing selections should be made in late update because the selections are often made in update
	void LateUpdate() {
		//check the left mouse button click
		if (Input.GetMouseButtonUp(0)) {
			PressSelectedUnit ();
		}
	}

	//select the given unit, this selection event may be given by mouse events or key events
	//it can be given before entering targeting mode so 
	public void SelectUnit(UnitController unit) {
		//if the selected unit is the same as the prev selected one then do nothing
		if (selectedUnit != unit) {
			selectedUnit = unit;

			//if in targeting mode store the selection as a target based on what mode the targetting is
			//don't allow invalid targets to be added as this would negate the valididity check
			if (state == MoveListState.Targeting) {
				if (targetMode == TargetTypeMode.Single) {
					//check if the selected unit is valid and if so target it it
					if (singleAbilityTarget.CheckValidTarget (user, unit)) {
						targetedUnits.Add (unit);
					}
				} else if (targetMode == TargetTypeMode.Group) {
					//check if all units on the side selected are valid, select all valid ones
					bool selectedLeft = unit.LeftSide;
					//the units to check
					UnitController[] unitsToCheck;
					if (selectedLeft == true) {
						unitsToCheck = leftUnits;
					} else {
						unitsToCheck = rightUnits;
					}

					for (int i = 0; i < unitsToCheck.Length; i++) {
						if (multiAbilityTarget.CheckValidTarget(user, unitsToCheck[i])) {
							targetedUnits.Add(unitsToCheck[i]);
						}
					}
				} else if (targetMode == TargetTypeMode.All) {
					//just check all the units
					for (int i = 0; i < leftUnits.Length; i++) {
						if (multiAbilityTarget.CheckValidTarget(user, leftUnits[i])) {
							targetedUnits.Add(leftUnits[i]);
						}
					}
					for (int i = 0; i < rightUnits.Length; i++) {
						if (multiAbilityTarget.CheckValidTarget(user, rightUnits[i])) {
							targetedUnits.Add(rightUnits[i]);
						}
					}
				}
			}
		}

	}
	//deselect the current selected unit, used when a mouse moves away from a potential selection
	public void DeselectUnit() {
		selectedUnit = null;

		//if not in setnumber targeting mode then clear the targets
		if (state == MoveListState.Targeting && targetMode != TargetTypeMode.SetNumber) {
			targetedUnits.Clear ();
		}
	}
	//press the selected unit
	public void PressSelectedUnit() {
		if (selectedUnit != null && state == MoveListState.Targeting) {
			//if the targeting mode is setnumber then just add the selection to the list of targets if not there already
			if (targetMode == TargetTypeMode.SetNumber) {
				//check if target is valid
				if (multiAbilityTarget.CheckValidTarget (user, selectedUnit)) {
					//now check if target is unique and doesnt yet exist in the list
					bool unique = true;

					for (int i = 0; i < targetedUnits.Count; i++) {
						if (selectedUnit == targetedUnits [i]) {
							unique = false;
							break;
						}
					}

					if (unique == true) {
						//add the target
						targetedUnits.Add(selectedUnit);

						//if targets has reach the max amount use the ability
						if (targetedUnits.Count >= multiAbilityTarget.TargetNumber) {
							UseTargetedAbility ();
						}
					}
				}
			} else {
				UseTargetedAbility ();
			}
				
			//check if the unit is a valid target before doing anything
			//if ((singleAbilityTarget != null && singleAbilityTarget.CheckValidTarget(user, selectedUnit)) ||
			//	(multiAbilityTarget != null && singleAbilityTarget.CheckValidTarget(user, selectedUnit))
		}
	}

	//called when targets are selected for an ability and it is to be used
	private void UseTargetedAbility() {
		//if there is at least one valid target stored when this is pressed then the ability will be used and movelist closed
		if (targetedUnits.Count > 0) {
			if (targetMode == TargetTypeMode.Single) {
				singleAbilityTarget.UseAbility (user, targetedUnits [0]);
			} else {
				//ability must be multi ability
				multiAbilityTarget.UseAbility(user, targetedUnits.ToArray());
			}

			//use the resources if ability was not free, and close out the menu
			AbilityUsed ();
		}
	}

	//method to close the movelist once done with it, should reset it for the next user
	public void CloseMoveList() {
		//place all the buttons currently being used back into the pool and clear any refereances on them
		ClearHeldButtons();

		//clear all references so that they can be garbage collected
		storedPlayerBrain = null;

		//the cached state of the playing field
		user = null;
		leftUnits = null;
		rightUnits = null;
		leftReserve = null;
		rightReserve = null;

		selectedSlot = null;

		//make sure to remove any stray camera hints used
		cameraControl.hints.keepInFrame.Remove(listFramingRect);

		//turn off the move list dependant ui
		battleUI.HideMoveEssentialUI();

		//deactivate this component
		gameObject.SetActive(false);
	}

	//method for when an ability button is clicked, pass in the button in question
	public void AbilityButtonClicked(AbilityButton abilityButton) {
		//store the slot so that the resources can be taken away if needed
		selectedSlot = abilityButton.Ability;

		AbilityCommandClicked (selectedSlot.ability);
	}

	//ability command clicked
	//called when a basic command is clicked or as part of an ability button clicked function
	public void AbilityCommandClicked(BattleAbility ability) {
		//if the ability is a self ability then just use it right away after checking the user is a valid target
		if (ability.GetType().IsSubclassOf(typeof(SelfAbility))) {
			SelfAbility useAbility = (SelfAbility)ability; 
			//check target is valid (abilities may have conditions)
			if (useAbility.CheckValidTarget (user, user)) {
				AbilityUsed ();
				useAbility.UseAbility (user);
			}
			//if not valid then just do nothing
		} else {
			//enter the correct targeting mode for the ability (self, single, multi, group, etc)
			//also cache the ability in the single or multi slot
			if (ability.GetType().IsSubclassOf(typeof(SingleAbility))) {
				singleAbilityTarget = (SingleAbility)ability;
				targetMode = TargetTypeMode.Single;
			} else if (ability.GetType().IsSubclassOf(typeof(MultiAbility))) {
				multiAbilityTarget = (MultiAbility)ability;
				//select based on the multi targeting type
				if (multiAbilityTarget.MultiTargetType == MultiAbility.MultiTargetMode.ALL) {
					targetMode = TargetTypeMode.All;
				} else if (multiAbilityTarget.MultiTargetType == MultiAbility.MultiTargetMode.GROUP) {
					targetMode = TargetTypeMode.Group;
				} else if (multiAbilityTarget.MultiTargetType == MultiAbility.MultiTargetMode.SETNUMBER) {
					targetMode = TargetTypeMode.SetNumber;
				}
			}

			//if it is a self only ability then just use it straight up without targeting, but only if it is a valid target
			EnterTargetMode();
		}
	}

	//method to call when an ability is to be triggered, removes resources using an ability slot if stored
	private void AbilityUsed() {
		if (selectedSlot != null) {
			selectedSlot.UseAbilityResources (user);
		}
			
		//make sure to exit targeting mode and close the menu
		ClearTargettingMode();
		//close the move list
		CloseMoveList();
	}

	//puts the movelist into targetting mode
	//pulls back the list from the scene and disables the buttons in it
	private void EnterTargetMode() {
		//if the ability in use can't target dead ppl
		if (targetMode == TargetTypeMode.Single) {
			mouseSelector.selectDead = singleAbilityTarget.targetDead;
		} else {
			mouseSelector.selectDead = multiAbilityTarget.targetDead;
		}

		animator.Play (MOVE_OUT_HASH);

		preTargetState = state;
		state = MoveListState.Targeting;
		DisableButtons ();

		//remove the camera hints
		cameraControl.hints.keepInFrame.Remove(listFramingRect);

		//enable the exit target button
		exitTargetButton.gameObject.SetActive(true);
	}

	//method for when a category button is clicked
	public void CategoryButtonClicked(CategoryButton categoryButton) {
		//clear the movelist's current buttons and add the buttons from the category
		//along with a back button to exit out of the menu
		EnterCategoryMenu(categoryButton.Category);
	}

	//method for when the back command is recieved, can be from button or hotkey
	public void Back() {
		if (state == MoveListState.CategorySelect) {
			//go back to the start select
			EnterStartMenu();
		} else if (state == MoveListState.Targeting) {
			//go back to the category select
			//make the buttons interactable again if availiable
			DefaultEnableButtons();
			//add the camera hints back again
			cameraControl.hints.keepInFrame.Add(listFramingRect);
			//clear the selected abilities and slots
			ClearTargettingMode();
			//set the animator to move in mode
			animator.Play(MOVE_IN_HASH);
		}
	}

	//used to clear abilities and ability slots and other stuff cached as part of the target process
	//should be called when confirming a target or when backing out of targeting mode
	private void ClearTargettingMode() {
		selectedSlot = null;
		singleAbilityTarget = null;
		multiAbilityTarget = null;

		//when returning from clear target mode dont target dead units
		mouseSelector.selectDead = false;

		//clear selected and targeted units
		selectedUnit = null;
		targetedUnits.Clear ();

		exitTargetButton.gameObject.SetActive(false);

		//go back to the state before targetting mode was entered
		state = preTargetState;
	}

	//methods for each basic command
	public void Attack() {
		AbilityCommandClicked (storedPlayerBrain.basicAttack);
	}
	public void Defend() {
		AbilityCommandClicked (storedPlayerBrain.defend);
	}

	//make the buttons on the menu non interactable
	private void DisableButtons() {
		for (int i = 0; i < abilityBtn.Count; i++) {
			abilityBtn [i].Button.interactable = false;
		}
		for (int i = 0; i < categoryBtn.Count; i++) {
			categoryBtn [i].Button.interactable = false;
		}
		for (int i = 0; i < basicCommandBtn.Count; i++) {
			basicCommandBtn [i].Button.interactable = false;
		}
	}

	//restore the buttons to default interactability
	private void DefaultEnableButtons() {
		for (int i = 0; i < abilityBtn.Count; i++) {
			//in the case of the ability buttons check if it should be interactable, the button is interactable if the ability is marked as usable
			abilityBtn [i].Button.interactable = abilityBtn[i].Ability.usable;
		}
		for (int i = 0; i < categoryBtn.Count; i++) {
			categoryBtn [i].Button.interactable = true;
		}
		for (int i = 0; i < basicCommandBtn.Count; i++) {
			basicCommandBtn [i].Button.interactable = true;
		}
	}

	//method to clear all held buttons and store them in the pools
	private void ClearHeldButtons() {
		//iterate backwards so that the loop is not disrupted by removing end elements
		for (int i = abilityBtn.Count - 1; i >= 0; i--) {
			AbilityButton button = abilityBtn [i];
			//set the ability to null
			button.SetAbility(null);
			//clear any stored click events
			button.Button.onClick.RemoveAllListeners();
			//remove it from the current parent and attach to the pool
			button.transform.SetParent(poolHolder.transform);
			abilityBtnPool.Add (button);
			//finally remove it from the ability button list
			abilityBtn.RemoveAt(i);
		}
		for (int i = categoryBtn.Count - 1; i >= 0; i--) {
			CategoryButton button = categoryBtn [i];
			//clear the category

			button.Button.onClick.RemoveAllListeners();
			button.transform.SetParent(poolHolder.transform);
			categoryBtnPool.Add (button);
			categoryBtn.RemoveAt(i);
		}
		for (int i = basicCommandBtn.Count - 1; i >= 0; i--) {
			BasicCommandButton button = basicCommandBtn [i];
			button.Button.onClick.RemoveAllListeners();
			button.transform.SetParent(poolHolder.transform);
			basicCommandPool.Add (button);
			basicCommandBtn.RemoveAt(i);
		}
	}

	//method to get a button from the pool and return it for use
	//will create a new one if none are in the pool
	//places the button into the active list so it can be recoolected on menu clear
	private AbilityButton GetAbilityButton() {
		AbilityButton toReturn;

		if (abilityBtnPool.Count > 0) {
			toReturn = abilityBtnPool [0];
			abilityBtnPool.RemoveAt (0);
		} else {
			//create a new button as a child of the poolable object and return it
			toReturn = Instantiate(abilityButtonTemplate, poolHolder.transform);
		}

		abilityBtn.Add (toReturn);
		return toReturn;
	}
	private CategoryButton GetCategoryButton() {
		CategoryButton toReturn;

		if (categoryBtnPool.Count > 0) {
			toReturn = categoryBtnPool [0];
			categoryBtnPool.RemoveAt (0);
		} else {
			//create a new button as a child of the poolable object and return it
			toReturn = Instantiate(categoryButtonTemplate, poolHolder.transform);
		}

		categoryBtn.Add (toReturn);
		return toReturn;
	}
	private BasicCommandButton GetBasicButton() {
		BasicCommandButton toReturn;

		if (basicCommandPool.Count > 0) {
			toReturn = basicCommandPool [0];
			basicCommandPool.RemoveAt (0);
		} else {
			//create a new button as a child of the poolable object and return it
			toReturn = Instantiate(basicButtonTemplate, poolHolder.transform);
		}

		basicCommandBtn.Add (toReturn);
		return toReturn;
	}
}
