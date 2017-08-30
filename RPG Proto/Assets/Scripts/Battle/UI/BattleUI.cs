using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour {
	//the rally meter
	private RallyMeter rallyMeter;

	//the turn order display
	private TurnOrderDisplay turnDisplay;

	//show whether the intro animation has played yet, used to hold of components animating in before theyre meant to
	private bool introPlayed = false;

	//shows whether certain ui components are meant to be enabled or disabled
	//only components which must be visible at all times must be cached, as any turn dependant components will auto fade in each turn
	private bool rallyMeterEnabled = false;
	private bool healthBarsEnabled = false;

	//the player stat bars
	private PlayerStatusBars playerBars;

	//the place to hold unit status displays
	private UnitStatusDisplayHolder statusDisplayHolder;

	//the place to hold the move list that is displayed during a players turn
	private MoveList playerMoveList;

	//the current state of whether the player turn specific ui is enabled or not
	private bool playerTurnUI = false;
	public bool PlayerTurnUI {
		get {
			return playerTurnUI;
		}
	}

	//the gameobject to hold any overlay ui elements (often used as part of cutscenes, game over screens, etc)


	// Use this for initialization
	void Awake () {
		rallyMeter = GetComponentInChildren<RallyMeter> ();
		turnDisplay = GetComponentInChildren<TurnOrderDisplay> ();
		playerBars = GetComponentInChildren<PlayerStatusBars> ();
		statusDisplayHolder = GetComponentInChildren<UnitStatusDisplayHolder> (true);
		playerMoveList = GetComponentInChildren<MoveList> (true);
		playerMoveList.battleUI = this;
	}

	//method called after the camera is moved and everthing else is updated
	//used to position certain ui elements over a position in worldspace, while still being on a canvas
	public void BattleCameraPreCull () {
		//notify the status displays
		statusDisplayHolder.BattleCameraPreCull();
	}

	//method to be called when a party member wants to select its ability through the movelist ai
	//pass in the party member brain along with the user and all active units
	public void ShowPlayerMoveList(PartyMemberBrain playerBrain, UnitController user, UnitController[] leftUnits, UnitController[] rightUnits,
		UnitController[] leftReserve, UnitController[] rightReserve) {
		playerMoveList.gameObject.SetActive(true);
		playerMoveList.StartupMoveList (playerBrain, user, leftUnits, rightUnits, leftReserve, rightReserve);
	}

	//add a unit status display, to the given unit
	public void AddUnitStatusDisplay(UnitController unit) {
		statusDisplayHolder.AddUnitStatusDisplay (unit);
	}
	public void RemoveUnitStatusDisplay(UnitController unit) {
		statusDisplayHolder.RemoveUnitStatusDisplay (unit);
	}

	//called to enable the rally meter component, will animate the meter in when uifadein is called or animate it straight away otherwise
	public void EnableRallydisplay(Rally toDisplay) {
		rallyMeterEnabled = true;
		rallyMeter.SetDisplayRally (toDisplay);

		if (introPlayed == true) {
			rallyMeter.gameObject.SetActive(true);
			//animate the meter in
			rallyMeter.DisplayMeter();
		}
	}
	//don't bother animating, just switch the display off, make sure to set its rally reference to null
	public void DisableRallyDisplay() {
		rallyMeterEnabled = false;

		rallyMeter.gameObject.SetActive(false);
	}

	//add a player health bar to the health bars in a certain position priority (will displace existing bars to fit priority)
	//just pass in a suitable unit controller, let the ui system create the ui component based on the resources that must be displayed
	//higher priority goes to the left of the screen
	public void AddStatusBar(PartyUnit unitToDisplay, int priority) {
		healthBarsEnabled = true;
		playerBars.AddStatusBar (unitToDisplay, priority);
	}

	//remove a player health bar from the health bars, pass in the matching unit controller and it will be searched for
	public void RemoveStatusBar(PartyUnit unitToRemove) {
		playerBars.RemoveStatusBar (unitToRemove);
	}

	//notify the ui that a players turn has started, animate in turn speific ui, including the movelist holder of the player
	//pass in the player whose turn is starting so that they may be highlighted and their specific move list box may be used
	//the upcoming unitturns must also be passed so that the turndisplay can be updated with them
	public void PlayerTurnStart(PartyUnit player) {//, UnitController[] unitTurns) {
		playerTurnUI = true;

		//tell the turn display to animate in
		//turnDisplay.DisplayAppear();

		//also do a few things common to playerturn stay
		PlayerTurnStay(player);//, unitTurns);
	}

	//animate out the player turn ui
	public void PlayerTurnEnd() {
		playerTurnUI = false;

		//turnDisplay.DisplayHide ();
		//clear any highlights
		playerBars.ClearHighlights();
	}

	//tell the ui that the turn is changing to a different players turn but it will still be a player (so don't try to fade in / out player specific ui)
	public void PlayerTurnStay(PartyUnit newPlayer) {//, UnitController[] unitTurns) {
		//highligh the status bar of the player whose turn it is
		playerBars.HighlightStatusBar(newPlayer);

		//set the turn portraits using the given unitturns
		//turnDisplay.DisplayTurns(unitTurns);
	}

	//method for getting the number of unit turns that can be displayed by this ui, this will be used to find the specified number of unit turns to plass in to playerTurnStart
	public uint GetTurnDisplayMax() {
		return turnDisplay.DisplayNumber;
	}

	//used when a battle is first started and before intros are played and turn order and unit composition is solidified, 
	//used to set up resolution dependent stuff and reset components
	public void PreIntroSetup(BattleController battle) {
		//enable this object if not enabled yet
		//do this before calling ui fade in so that animation components can be set
		gameObject.SetActive(true);

		//give the move list a lin to cam control so it can be adjusted
		playerMoveList.cameraControl = battle.CameraControl;
		//also tell the movelist where the left units are
		playerMoveList.leftField = battle.Scenery.LeftField;
		//set up the left buffer zone and pool some buttons
		playerMoveList.Initialise();

		//give the status display holder a link to the battle camera
		SetBattleCamera(battle.BattleCamera);

		//show that the intro hasnt been played yet
		introPlayed = false;

		//make sure the player turn display is disabled by default
		playerMoveList.gameObject.SetActive(false);

		//disable necessary components, until player units call for them to be enabled
		rallyMeterEnabled = false;
		healthBarsEnabled = false;
		playerTurnUI = false;

		//disable the rally meter if no unit in the battle is using it
		rallyMeter.gameObject.SetActive(false);

		//reset all ui components
		rallyMeter.SetupFromBattle(battle);
		turnDisplay.SetupFromBattle (battle);
		playerBars.SetupFromBattle (battle);
	}

	//used when the ui is first set to be visible, start playing intro animations for components and stuff
	public void UIfadeIn(BattleController battle) {
		//enable the rally meter
		//EnableRallydisplay(new Rally());

		if (healthBarsEnabled == true) {
			playerBars.DisplayBars ();
		}

		//if the rally meter is to be enabled then animate it in now
		if (rallyMeterEnabled == true) {
			rallyMeter.gameObject.SetActive(true);
			//animate the meter in
			rallyMeter.DisplayMeter();
		}

		//show that the intro has now been played
		introPlayed = true;
	}

	//called by a movelist when displayed, enables all ui elements that should be visible while picking a move
	//pass in the player unit whose turn it is so they can be highlighted
	public void DisplayMoveEssentialUI(UnitController playerMove) {
		//tell the turn display to animate in
		turnDisplay.DisplayAppear();

		//highligh the status bar of the player whose turn it is
		playerBars.HighlightStatusBar(playerMove);

		//predict the next turns coming up, predict enough to fill out the ui
		UnitController[] turnOrder = playerMove.Battle.ActiveBattle.PredictFutureTurns (playerMove, GetTurnDisplayMax ());
		//set the turn portraits using the given unitturns
		turnDisplay.DisplayTurns(turnOrder);
	}
	//the inverse to the above
	public void HideMoveEssentialUI() {
		turnDisplay.DisplayHide ();
	}

	//multiple parts of the battle ui use the battle camera for something
	//use this to set them all
	public void SetBattleCamera(Camera camera) {
		//tell the status bars which one to use 
		statusDisplayHolder.unitRenderCamera = camera;
		playerMoveList.GetComponent<MouseUnitSelector> ().selectionCamera = camera;
	}
}
