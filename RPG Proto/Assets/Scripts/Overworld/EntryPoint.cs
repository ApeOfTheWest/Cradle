using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntryPoint : MonoBehaviour {

	//called when the entry point is activated
	//takes the current instance of the player as an argument so that they can be placed on the scene properly
	public abstract void Enter (PlayerController player);
}
