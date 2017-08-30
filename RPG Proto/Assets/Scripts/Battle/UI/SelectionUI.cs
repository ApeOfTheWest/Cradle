using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the selection ui displays certain elements that should appear when a unit is selected, as some elements follow units the cameras on pre cull should be used
//this also displays the larger unit view menu when a unit is examined
public class SelectionUI : MonoBehaviour {
	//the selection icon holder, manages the various icons that can be enabled and disabled on a unit
	private SelectionIconHolder iconHolder;

	// Use this for initialization
	void Awake () {
		iconHolder = GetComponentInChildren<SelectionIconHolder> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
