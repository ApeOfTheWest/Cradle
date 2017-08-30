using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script for a room object, one should be placed on each game object that corresponds to a room
public class Room : MonoBehaviour {
	private EntryPointHolder entryPointHolder;

	//events called when the room is first entered and when the room is about to be exited
	public delegate void RoomEvent();
	//event with all the listeners registered
	public event RoomEvent roomEntered;
	public event RoomEvent roomExited;

	//default camera settings, these will be set on entering the room to give a camera that fits the room best
	//the default size of the camera
	public float defaultCameraSize = 5f;
	//the default z position of the camera
	public float defaultCameraZ = -10f;
	//the music to play by default on entering the room, if null will stop current music
	public AudioClip backgroundMusic;

	public EntryPointHolder EntryPointHolder {
		get {
			return entryPointHolder;
		}
	}

	// Use this for initialization
	void Awake () {
		entryPointHolder = GetComponentInChildren<EntryPointHolder> ();
	}

	//use this when the room is about to be exited
	//this may fire off events to some objects in the room
	public void ExitRoom() {
		RoomEvent evnt = roomExited;
		if (evnt != null) {
			evnt ();
		}
	}

	//use this when the room is about to be entered
	//may fire off events to some objects in the room
	//this is always called after the room has been enabled
	//typical use of enter room is to respawn enemies or respond to change in state
	public void EnterRoom() {
		RoomEvent evnt = roomEntered;
		if (evnt != null) {
			evnt ();
		}
	}
}
