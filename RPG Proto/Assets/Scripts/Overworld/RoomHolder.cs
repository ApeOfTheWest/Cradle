using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHolder : MonoBehaviour {
	//the list of room component that correspond to the root objects of each room
	private Room[] rooms;

	//the currently active room
	private Room activeRoom = null;

	// Use this for initialization
	void Awake () {
		//get a list of all the rooms in the heirchy, make sure to include inactive rooms
		rooms = GetComponentsInChildren<Room>(true);
		//make sure all the rooms are disabled to start with
		for (int i = 0; i < rooms.Length; i++) {
			rooms [i].gameObject.SetActive (false);
		}
	}

	//method to change the active room, requires a room and an entry point into that room
	//always change rooms using this method so that cleanup and necessary methods will be called automatically
	public void ChangeRoom(Room room, EntryPoint entry) {
		//if the active room is non null then disable the previously active room
		if (activeRoom != null) {
			//call the exit room function first
			activeRoom.ExitRoom();

			activeRoom.gameObject.SetActive (false);
		}

		//and set the new active room
		activeRoom = room;
		activeRoom.gameObject.SetActive (true);

		//set the size and z of the camera, this is usually overwitten by the entry point if something else is desired
		OverworldController.OverworldCamera.OrthoCamera.orthographicSize = room.defaultCameraSize;
		OverworldController.OverworldCamera.transform.position = new Vector3 (OverworldController.OverworldCamera.transform.position.x,
			OverworldController.OverworldCamera.transform.position.y,
			room.defaultCameraZ); 

		LevelController.soundSystemInstance.MusicSource.clip = room.backgroundMusic;
		LevelController.soundSystemInstance.MusicSource.Play ();

		//activate the entry point before calling the enter room event, this is so entry point specific flags can be set up first and the player can be added
		entry.Enter(LevelController.playerInstance);

		activeRoom.EnterRoom ();
	}

	//a version of change room that depends on the room name, usually called when transferring between scenes
	public void ChangeRoom(string roomName, string entryName) {
		//search the rooms for the first room that matches the name

		//holds the room when found, or null if not found
		Room foundRoom = null;

		for (int i = 0; i < rooms.Length; i++) {
			if (rooms [i].gameObject.name == roomName) {
				foundRoom = rooms [i];
				//break out once a room is found
				break;
			}
		}

		//the room must be enabled before looking for entry points or else the entry point holder wont have collated them
		//the current room must be disabled before this is done if applicable because it may interact with the next room in unwanted ways
		if (activeRoom != null) {
			activeRoom.gameObject.SetActive (false);
		}
		foundRoom.gameObject.SetActive(true);
		foundRoom.gameObject.SetActive(false);
		if (activeRoom != null) {
			activeRoom.gameObject.SetActive (true);
		}

		//now find the entry point
		EntryPoint foundEntry = null;

		EntryPoint[] entryPoints = foundRoom.EntryPointHolder.EntryPoints;

		for (int i = 0; i < entryPoints.Length; i++) {
			if (entryPoints [i].gameObject.name == entryName) {
				foundEntry = entryPoints [i];
				//break out once a point is found
				break;
			}
		}

		ChangeRoom (foundRoom, foundEntry);
	}
}
