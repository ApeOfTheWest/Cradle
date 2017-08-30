using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a common class for all interactable objects in a scene (objects that respond to the use key)
//interactable objects may display a prompt when they are ready to be selected, so this requries selection and use methods
//IMPORTANT: the collider should be on the interactable layer
public abstract class Interactable : MonoBehaviour {
	//each interactable object has a trigger to show its interactable area, but as it is the player who decides what to interact with interactions shouldn't try to call
	//their methods by themselves

	//method to call when selected
	//pass in the player selecting it
	public abstract void Selected(PlayerController playerSelecting);

	//method to call when deselected
	public abstract void Deselected();

	//method to call when used
	public abstract void UseInteractable(PlayerController playerInteracting);
}
