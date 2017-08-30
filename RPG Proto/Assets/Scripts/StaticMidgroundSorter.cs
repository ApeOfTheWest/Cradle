using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMidgroundSorter : MidgroundOrderer {

	// Use this for initialization
	void Start () {
		//this only happens once for a static object but can be called manually
		UpdateParentZ();
	}

}
