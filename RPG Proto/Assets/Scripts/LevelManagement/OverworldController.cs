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

	// Use this for initialization
	void Awake () {
		roomHolder = GetComponentInChildren<RoomHolder> ();
		overworldCamera = GetComponentInChildren<ParallaxCamControl> ();
		cameraFollow = overworldCamera.gameObject.GetComponent<CameraFollower> ();
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
