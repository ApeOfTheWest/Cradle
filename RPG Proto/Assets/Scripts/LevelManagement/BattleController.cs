using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {
	[HideInInspector]
	public LevelController levelControl;

	//the controller to use for the turn to turn part of the battle
	private ActiveBattleController activeBattle;
	public ActiveBattleController ActiveBattle {
		get {
			return activeBattle;
		}
	}

	//the camera being used to render the battlefield this will be a parallax enabled camera
	private Camera battleCamera;
	public Camera BattleCamera {
		get {
			return battleCamera;
		}
	}

	//the script that controls how the camera changes its position for the battle
	//also provides callbacks for certain ui elements which need onpre cull
	private BattleCameraControl cameraControl;
	public BattleCameraControl CameraControl {
		get {
			return cameraControl;
		}
	}

	//the scenery currently being used for all battles
	//should be given a default set of scenery in case no scenery is given to it
	[SerializeField]
	private BattleScenery scenery;
	public BattleScenery Scenery {
		get { return scenery; }
	}

	//the music to play in the middle of a battle
	//this is reset to the music of the attached battle sceney at the end of each battle but can be overidden before a battle if desired
	public AudioClip battleMusic;

	//an object that can be used to temporarily store units that aren't to be used yet
	private TempUnitStorage tempUnitHolder;
	//the place to hold the scenery in, cleans up after a scenery change by destroying the old scnery
	private SceneryHolder sceneryHolder;

	//a place that holds objects being used by a battle intro
	private IntroHolder introHolder;

	//the current win condition and loss condition
	//if both are null then just check if all allies or enemies are dead

	//bool that shows whether unit intros have started being played already, used to ensure they arent played multiple times
	private bool unitIntrosPlayed = false;

	//bool that shows whether the battle intro is finished or not
	private bool introFinished = false;

	//list of active units that have yet to finish their intro
	//both the battle intro and unit intros need to be played before the battle can begin
	private List<UnitController> unitsPlayingIntro = new List<UnitController>();

	//enum to be passed in when a battle is started, tells the controller who has initiative
	public enum InitiativeState {
		LeftAdvantage, RightAdvantage, Neutral
	};

	void Awake() {
		activeBattle = GetComponentInChildren<ActiveBattleController> ();

		tempUnitHolder = GetComponentInChildren<TempUnitStorage> ();
		sceneryHolder = activeBattle.gameObject.GetComponentInChildren<SceneryHolder> ();
		introHolder = GetComponentInChildren<IntroHolder> ();

		battleCamera = GetComponentInChildren<Camera> ();
		cameraControl = battleCamera.GetComponent<BattleCameraControl> ();
		cameraControl.battle = this;

		//deactive the storage so that stored units don't update or render
		tempUnitHolder.gameObject.SetActive(false);

		//get the default scenery from the holder
		//call set scenery with the given default scenery
		SetUpScenery(scenery);

		//tell the active battle controller that this is the controller to use
		activeBattle.SetBattle(this);
	}
		
	//use this to enable / disable the entire battle section of the level
	public void SetBattleActive(bool active) {
		gameObject.SetActive (active);
	}

	//use to setup a new piece of scenery in the battle controller
	//this scenery should have its game object added to the scenery holder in the battle controller heirchy
	//when the scenery is changed out the old scenery will be removed
	//and unless told to not destroy itself when this happens, will be destroyed
	public void SetUpScenery(BattleScenery newScenery) {
		//remove any current childs of the scenery holder
		sceneryHolder.ClearScenery();

		scenery = newScenery;

		//add the new scenery object to the scenery holder
		scenery.transform.parent = sceneryHolder.transform;
		//ensure it is enabled
		scenery.gameObject.SetActive(true);

		//tell the scenery that it is being used
		scenery.InitialSetup(levelControl);

		//set the battle music based on the scenery
		battleMusic = scenery.battleMusic;
	}

	//call this to start a battle
	public void StartBattle(UnitController[] leftUnits, UnitController[] rightUnits,
		UnitController[] leftReserves, UnitController[] rightReserves,
		List<WinCondition> leftWin, List<WinCondition> rightWin,
		VictoryMethod leftVictory, VictoryMethod rightVictory,
		BattleReward initialReward, BattleController.InitiativeState initiativeState, InitiativeStrike initiativeStrike, BattleIntro intro = null) {

		//play the battle music
		LevelController.soundSystemInstance.MusicSource.clip = battleMusic;
		LevelController.soundSystemInstance.MusicSource.Play ();

		//tell the ui controller to set itself, enable it for the duration of this
		//set the ui up before setting up the units so that units can enable / disable ui elements after
		activeBattle.battleUI.PreIntroSetup(this);

		//set the left and right win conditions and victory methods
		activeBattle.leftWinConditions = leftWin;
		activeBattle.rightWinConditions = rightWin;
		activeBattle.leftVictory = leftVictory;
		activeBattle.rightVictory = rightVictory;

		//reset the left and right rally resources before giving the new units references to them
		activeBattle.LeftRally.ResetToDefault();
		activeBattle.RightRally.ResetToDefault ();

		//add the active and reserve units to the battle
		//do this using the unit holder's functions to automatically add units in the right position
		//initialise each unit before adding it to the battle, as adding it will call activation functions
		for (int i = 0; i < leftUnits.Length; i++) {
			//NOTE: some intialise battle scripts refer to the current rally meter, so set the rally first
			leftUnits [i].Rally = activeBattle.LeftRally;
			leftUnits [i].InitialiseBattle (this);
			activeBattle.ActiveUnits.AddLeftUnit (leftUnits [i], scenery);
		}
		for (int i = 0; i < rightUnits.Length; i++) {
			rightUnits [i].Rally = activeBattle.RightRally;
			rightUnits [i].InitialiseBattle (this);
			activeBattle.ActiveUnits.AddRightUnit (rightUnits [i], scenery);
		}
		for (int i = 0; i < leftReserves.Length; i++) {
			leftReserves [i].Rally = activeBattle.LeftRally;
			leftReserves [i].InitialiseBattle (this);
			activeBattle.ReserveUnits.AddLeftUnit (leftReserves [i]);
		}
		for (int i = 0; i < rightReserves.Length; i++) {
			rightReserves [i].Rally = activeBattle.RightRally;
			rightReserves [i].InitialiseBattle (this);
			activeBattle.ReserveUnits.AddRightUnit (rightReserves [i]);
		}

		//tell the controller that the unit intros havent been played yet
		unitIntrosPlayed = false;
		//and same for the battle intro
		introFinished = false;

		//tell the scenery to set itself up, this will add camera hints
		scenery.PreBattleSetup(this);
		//tell the camera to snap to the position of the hints initially, afterwards any changes in hints will produce smooth movement towards it
		CameraControl.StartOfBattle();

		//if the intro is non null then play it first, and make sure the active battle scene is disabled
		if (intro != null) {
			activeBattle.gameObject.SetActive (false);

			//play the intro, pass in the intro holder so that any necessary game objects can be added to it and then removed
			intro.StartIntro(this, introHolder);
		} else {
			//call the batle intro finished function right away
			BattleIntroFinished();
		}
	}

	//end the battle, delete all temp data and return to overworld
	public void EndBattle() {
		tempUnitHolder.DeleteTempUnits ();

		//reset the battle control camera
		CameraControl.Reset();

		//set the battle music based on the scenery at the end of each battle for the next one
		battleMusic = scenery.battleMusic;

		//return to overworld
		levelControl.SwitchToOverworld();
	}

	//call this to make all active units play their intro animations
	//can be called as part of an introduction or automatically, will only be called once per battle
	public void PlayUnitIntros() {
		//make sure intros only get played once
		if (unitIntrosPlayed == false) {
			unitIntrosPlayed = true;

			unitsPlayingIntro.Clear ();

			//add all units to the unit intro list
			//tell all units to play their intros
			List<UnitController> leftUnits = activeBattle.ActiveUnits.LeftUnits;
			List<UnitController> rightUnits = activeBattle.ActiveUnits.RightUnits;

			for (int i = 0; i < leftUnits.Count; i++) {
				unitsPlayingIntro.Add (leftUnits [i]);
				leftUnits [i].PlayUnitIntro ();
			}
			for (int i = 0; i < rightUnits.Count; i++) {
				unitsPlayingIntro.Add (rightUnits [i]);
				rightUnits [i].PlayUnitIntro ();
			}

			//wait until intros are finished playing before starting the battle
		}
	}

	//call this to make the battle ui play its intro animation and initialise itself
	//this must be called after any intro or initiative strike is played, in case it alters the ui components (like by changing turn order)
	private void PlayUIintro() {
		//dont enable stuff that depends on it being the player's turn this will be enabled when their turn starts
		activeBattle.battleUI.UIfadeIn(this);
	}

	//call this when the battle intro is finished
	public void BattleIntroFinished() {
		//ensure that any remains of the intro are cleaned up
		introHolder.CleanupIntro();

		//ensure the active battle objects are enabled if they werent as part of the intro
		activeBattle.gameObject.SetActive (true);

		//also ensure that the unit intros are played if they werent already
		PlayUnitIntros();

		//tell the controller that the battle intro is finished
		introFinished = true;

		//in case the unit intros have already finished check whether to start the active battle
		CheckStartActiveBattle();
	}

	//call this when a unit intro is finished
	public void UnitIntroFinished(UnitController unitFinished) {
		//see if the corresponding unit is in the list, remove it if it is
		unitsPlayingIntro.Remove (unitFinished);

		//check if all necessary intros were finished
		CheckStartActiveBattle();
	}

	//call this to check whether to start the active battle, start the battle if true
	private void CheckStartActiveBattle() {
		//if the battle intro is finished and no units are still playing their intros then start the battle
		if (introFinished == true && unitsPlayingIntro.Count == 0) {
			//play the ui intro as the battle is starting
			PlayUIintro ();

			activeBattle.BattleStart ();
		}
	}

	//call this when a battle is won or lost


	//get the temp unit holder, often used to place initialised units before a battle starts to prevent them clogging up the heirchy
	public GameObject GetTempUnitHolder() {
		return tempUnitHolder.gameObject;
	}

	//tell the battle controller to take control again after a unit's turn has ended
	//this will start the next turn after calculating whose turn it is
	public void EndCurrentTurn() {
		//call the related method in the active controller
		activeBattle.EndCurrentTurn();
	}
}
