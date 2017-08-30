using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class looks for interactables in front of the player and allows the player to select them and use them
//the object this is attached to should have a collider trigger attached to look for interactables
public class PlayerObjectInteraction : MonoBehaviour {
	//the player that is to interact with any object used
	public PlayerController interactingPlayer;

	//the currently selected interactable
	private Interactable selectedInteractable;
	//set using a property to auto call select and deselect events
	private Interactable SelectedInteractable {
		set {
			//if the current value is non null and is different to the current value call deselect
			//first check that the new value is different
			if (selectedInteractable != value) {
				//now if the current value is non null call deselect on it
				if (selectedInteractable != null) {
					selectedInteractable.Deselected ();
				} else if (value != null) {
					//otherwise if the new value is non null then call select on it
					value.Selected(interactingPlayer);
				}

				//if the new value is null then disable this script to save update cycles
				if (value == null) {
					this.enabled = false;
					//Debug.Log ("Interactable deselected");
				} else {
					this.enabled = true;
					//Debug.Log ("Interactable selected" + value.ToString ());
				}

			}

			selectedInteractable = value;
		}
	}

	//the list of interactables that are currently in range (this is stored so that if one is moved away from the other will be selected instead)
	private List<Interactable> interactablesInRange = new List<Interactable>();

	//look at on trigger stay each turn to check out new interactables that may
	void OnTriggerEnter2D(Collider2D collider) {
		//if a new interactable is in range then add it to the list
		Interactable interactable = collider.gameObject.GetComponent<Interactable>();
		if (interactable != null) {
			interactablesInRange.Add (interactable);
			//also test if it is closer than the current selection
			SelectedInteractable = GetClosestInteractable();
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		//if a interactable is leaving then remove it 
		Interactable interactable = collider.gameObject.GetComponent<Interactable>();
		if (interactable != null) {
			//remove from the list
			interactablesInRange.Remove (interactable);

			//if it is the current selection then try to replace it with a different interactable object
			if (selectedInteractable == interactable) {
				//get the closest interactable
				SelectedInteractable = GetClosestInteractable();
			}
		}
	}

	//helper method that returns the closes interactable in range
	private Interactable GetClosestInteractable() {
		//the closest interactable
		Interactable closest = null;
		//get the distance to the current closest
		float closestDistance = 0f;

		//start off by assigning the first interactable if availiable
		if (interactablesInRange.Count > 0) {
			closest = interactablesInRange [0];
			closestDistance = GetDistanceSqrToInteractable(interactablesInRange [0]);
		}

		//check each cached interactable for a closer one
		for (int i = 1; i < interactablesInRange.Count; i++) {
			float newDistance = GetDistanceSqrToInteractable(interactablesInRange [i]);

			if (newDistance < closestDistance) {
				closest = interactablesInRange [i];
				closestDistance = newDistance;
			}
		}

		//return null if none found
		return closest;
	}

	//get the distance squared for comparison, its less computing intensive
	private float GetDistanceSqrToInteractable(Interactable toMeasure) {
		//get vector2 from the player to the interactable
		Vector2 vectorToPlayer = interactingPlayer.transform.position - toMeasure.transform.position;

		return vectorToPlayer.sqrMagnitude;
	}

	//every update check to see if the interact button is pressed
	//also check which interactable should be selected based on the distance between the interactable objects, this must be done in lateupdate, after positions r moved
	//disable this script when no selections are made to prevent superfluous checking
	void LateUpdate() {
		//check what the closest interactable is and set accordingly
		SelectedInteractable = GetClosestInteractable();

		//check if the interaction button is pressed
		if (selectedInteractable != null && Input.GetKeyDown("e")) {
			//press it
			selectedInteractable.UseInteractable(interactingPlayer);
		}

	}
}
