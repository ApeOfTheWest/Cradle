using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEntryPoint : EntryPoint {
	//the simple entry point just sets the position of the player, tells the camera to follow it, and sets the direction the player is facing
	//the object to parent the player to must also be included so it can be added to the heirchy

	public Vector2 facingDirection = new Vector2();

	public GameObject parent = null;

	public override void Enter(PlayerController player) {
		//child the player to the given parent
		player.transform.parent = parent.transform;
		//set the position of the player to equal this position
		player.transform.position =  transform.position;


		//tell the camera to follow the player
		OverworldController.CameraFollow.toFollow = player.transform;

		//enable the player
		player.gameObject.SetActive(true);

		//tell the player to face a specific direction
		//note: do this after activating the player so the animator can update
		player.MoveDirection = facingDirection;
	}
}
