using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldController : MonoBehaviour {
	[HideInInspector]
	public LevelController levelControl;
	//the game object script that holds all the rooms
	private static RoomHolder roomHolder;

	//the script attached camera that also holds the 3 supplementary game objects
	private static ParallaxCamControl overworldCamera;
	public static ParallaxCamControl OverworldCamera {
		get {
			return overworldCamera;
		}
	}
	//the script that follows a player
	private static CameraFollower cameraFollow;
	public static CameraFollower CameraFollow {
		get {
			return cameraFollow;
		}
	}

	//get the ui ststem
	private static OverworldUI uiSystem;
	public static OverworldUI UISystem {
		get {
			return uiSystem;
		}
	}

	//whether the pause menu is enabled or not
	//it will often be disabled when in other menus or during cutscenes
	public bool pauseMenuEnabled = true;

	//display the pause menu, will only complete is pause enabled is true AND the level controller is not paused 
	//(as this indicates something else may doing something in realtime that the pause menu wouldn't stop)
	public void ShowPauseMenu() {
		//only if both conditions are satisfied should the pause menu be shown
		if (pauseMenuEnabled == true && levelControl.WorldPaused == false) {
			uiSystem.PauseMenu.OpenMenu ();
		}
	}

	// Use this for initialization
	void Awake () {
		roomHolder = GetComponentInChildren<RoomHolder> ();
		overworldCamera = GetComponentInChildren<ParallaxCamControl> ();
		cameraFollow = overworldCamera.gameObject.GetComponent<CameraFollower> ();
		uiSystem = GetComponentInChildren<OverworldUI> ();
	}

	//use this to enable / disable the entire overworld section of the level
	public void SetOverworldActive(bool active) {
		gameObject.SetActive (active);
	}
		
	//get the current instance of the roomholder
	public static RoomHolder roomHolderInstance {
		get {
			return roomHolder;
		}
	}
}
