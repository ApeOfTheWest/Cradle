using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//overall controller class for game levels
//holds references to the overworld controller and the battle controller, and they hold references to this
//the level controller also maintains static references to this instance and the battle / overworld controller instances in a singleton like way
public class LevelController : MonoBehaviour {
	private static LevelController levelController;
	private static BattleController battleController;
	private static OverworldController overworldController;
	private static SoundSystem soundSystem;

	private static PersistentData persistentData;

	//the instance of the player currently being used
	//will need to be made from a prefab when the scene is first entered and then can persist throughout the scene
	private static PlayerController player;

	//the prefab to create the player from
	public PlayerController playerBlueprint = null;

	//the saved time rate of world before pausing
	private float cachedTickRate;
	//whether the world is paused at the moment, if the world is paused for the dialog it should be unpaused at the end of it
	private bool worldPaused = false;
	public bool WorldPaused {
		get {
			return worldPaused;
		}
		set {
			if (worldPaused == false && value == true) {
				cachedTickRate = Time.timeScale;
				Time.timeScale = 0;
			} else if (worldPaused == true && value == false) {
				Time.timeScale = cachedTickRate;
			}

			worldPaused = value;
		}
	}

	//events called when the level is first entered and when the level is about to be exited
	public delegate void LevelEvent();
	//event with all the listeners registered
	public event LevelEvent levelEntered;
	public event LevelEvent levelExited;

	void Awake() {
		//set the level controller instance to this
		levelController = this;

		//get the reference to the persistent data
		persistentData = FindObjectOfType<PersistentData>();

		//get references to the overworld and battle controller
		//they should both be children of this
		battleController = GetComponentInChildren<BattleController> (true);
		overworldController = GetComponentInChildren<OverworldController> (true);
		soundSystem = GetComponentInChildren<SoundSystem> (true);

		//ensure battle controller is disabled and overworld is enabled to start with
		//enable and disable each controller in turn so that awake functions are called before other methods
		//end with the overworld being active
		//enable and then disable the controller so that it can initialise itself before methods are called
		/*battleController.gameObject.SetActive(true);
		battleController.gameObject.SetActive(false);

		overworldController.gameObject.SetActive (true);*/

		//set the references to this in each child controller
		battleController.levelControl = this;
		overworldController.levelControl = this;

		//create the player instance from a prefab, start the player off disabled until it is added to a room
		if (playerBlueprint != null) {
			player = Instantiate (playerBlueprint, new Vector3 (), Quaternion.identity);
			player.gameObject.SetActive (false);
		}
	}

	void Start() {
		//set overworld and battle controller to active / inactive here to avoid wierd edge cases where they are deactivated before awake functs r called
		battleController.gameObject.SetActive(true);
		battleController.gameObject.SetActive(false);

		overworldController.gameObject.SetActive (true);

		//don't set controller to inactive until its awake function is called
		//start the battle controller and all children of it disabled
		battleController.SetBattleActive (false);

		//start the scene by entering using the entry point saved in the persistent data
		OverworldController.roomHolderInstance.ChangeRoom(persistentData.entryRoom, persistentData.entryPoint);

		//call any level enter events
		EnterLevel ();
	}

	//call an event when entering a level for the first time, should be called after the overworld controller has been enabled
	private void EnterLevel() {
		LevelEvent evnt = levelEntered;
		if (evnt != null) {
			evnt ();
		}
	}
	//call an event just before exiting a level
	private void ExitLevel() {
		LevelEvent evnt = levelExited;
		if (evnt != null) {
			evnt ();
		}
	}

	//switch to the given scene, make sure to notify any exit level events
	public void SwitchScene(string levelName) {
		ExitLevel ();

		//save the current set of sceneflags for this scene before switching


		//switch
		SceneManager.LoadScene(levelName);
	}

	//switches the view to the battleview using the given intro, background and units

	//it should be noted that the units given can be completely arbitrary for both sides, 
	//two sets of enemy units could even be given in which case the player would have no control and just have to watch
	//in this sense enemy units may have the same central 'brain' or act independantly

	//if no intro is given then the default intro will be used
	//also tell the controller which side has initiative if anyone

	//the initiative strike must be non null if the state is not neutral, if neutral the strike wont usually be used but can be
	//if the battle intro is null then the default can be used
	public void SwitchToBattle(UnitController[] leftUnits, UnitController[] rightUnits,
		UnitController[] leftReserves, UnitController[] rightReserves,
		List<WinCondition> leftWin, List<WinCondition> rightWin,
		VictoryMethod leftVictory, VictoryMethod rightVictory,
		BattleReward initialReward, BattleController.InitiativeState initiativeState, InitiativeStrike initiativeStrike, BattleIntro intro = null) {

		//disable the overworld and enable the battle
		overworldController.SetOverworldActive(false);

		//save the current music position, before the battle system plays its own music
		soundSystem.SaveMusicState();

		battleController.SetBattleActive (true);

		battleController.StartBattle (leftUnits, rightUnits, leftReserves, rightReserves, 
			leftWin, rightWin, leftVictory, rightVictory,
			initialReward, initiativeState, initiativeStrike, intro);
	}

	//used to switch back to the overworld view
	public void SwitchToOverworld() {
		//switch the music back
		soundSystem.RevertMusicState();

		battleController.SetBattleActive (false);

		overworldController.SetOverworldActive(true);
	}

	//get a temporary place to store units while initialising them but before battling them
	public GameObject GetTempUnitStorage() {
		return battleController.GetTempUnitHolder ();
	}

	//get the controllers
	public BattleController GetBattle() {
		return battleController;
	}
	public OverworldController GetOverworld() {
		return overworldController;
	}

	//get the persistent data
	public PersistentData GetPersistentData() {
		return persistentData;
	}

	//method to get the static singleton like data stored in this controller
	public static LevelController levelControllerInstance {
		get {
			return levelController;
		}
	}
	public static BattleController battleControllerInstance {
		get {
			return battleController;
		}
	}
	public static OverworldController overworldControllerInstance {
		get {
			return overworldController;
		}
	}
	public static PersistentData persistentDataInstance {
		get {
			return persistentData;
		}
	}
	public static PlayerController playerInstance {
		get {
			return player;
		}
	}
	public static SoundSystem soundSystemInstance {
		get {
			return soundSystem;
		}
	}
}
