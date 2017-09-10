using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the scan on awake option scans before some colliders are initialised, do the first scan here
public class InitialScanner : MonoBehaviour {

	//do the intial scan on the first update (after all colliders are set up)
	void Update() {
		this.enabled = false;
		AstarPath.active.Scan ();
	}
}
