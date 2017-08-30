using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPointHolder : MonoBehaviour {
	//keeps track of all the entry points currently in use in a room
	//provides a way to search the active entry points by name

	private EntryPoint[] entryPoints;

	public EntryPoint[] EntryPoints {
		get {
			return entryPoints;
		}
	}

	// Use this for initialization
	void Awake () {
		//get a list of all the entry points in the heirchy
		entryPoints = GetComponentsInChildren<EntryPoint>();
	}


}
