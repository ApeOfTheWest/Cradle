using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetDetection : MonoBehaviour {
	//target detection is used by all classes that hope to target something through some means, eg an enemy spotting a player
	//it dispatches a target found event along with the targeted game object to any listeners when a target has been found
	//the target type is always a gameobject

	//events fired should include this detector as an identifier in systems with multiple detectors

	//delegate type to use for the event
	public delegate void DetectionDelegate(TargetDetection detector, GameObject target);
	//event with all the listeners registered
	public event DetectionDelegate targetDetected;

	protected void TargetDetected(GameObject target) {
		//check for null listeners, cache the event before checking to ensure thread safeness

		DetectionDelegate evnt = targetDetected;
		if (evnt != null) {
			evnt (this, target);
		}
	}

	//method to get and set the enabled state of the target detection
	public abstract bool Enabled {
		set;
		get;
	}
}
