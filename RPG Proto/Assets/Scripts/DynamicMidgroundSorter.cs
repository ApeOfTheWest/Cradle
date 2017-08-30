using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sets the z-position of an object every time late update happens, should be used for objects that may move position unpredicatable every frame only
//should be disabled if object stops moving for some reason to save cpu
public class DynamicMidgroundSorter : MidgroundOrderer {
	
	// lateUpdate is called once per frame after regular update
	void LateUpdate () {
		//set the z position of the object this is attached to using the y position of this orderer
		UpdateParentZ();
	}
}
