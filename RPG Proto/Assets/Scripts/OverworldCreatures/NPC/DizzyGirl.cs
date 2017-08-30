using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the dizzy girl controller and interaction provider
public class DizzyGirl : Interactable {
	//the dialogue to activate when talking to this girl
	[SerializeField]
	private DialogueTree dialogue;
	//the dialogue interface to give the system to

	public override void Selected(PlayerController playerSelecting) {

	}

	//method to call when deselected
	public override void Deselected() {
		
	}

	//method to call when used
	public override void UseInteractable(PlayerController playerInteracting) {
		//start a dialogue with the player
	}
}
