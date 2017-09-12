using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//each pause menu content object can take some data to initialise itself with
public abstract class PauseMenuContent : MonoBehaviour {

	public abstract void Initialise (PersistentData data);
	//and cleanup to discar references with
	public abstract void Cleanup();
}
