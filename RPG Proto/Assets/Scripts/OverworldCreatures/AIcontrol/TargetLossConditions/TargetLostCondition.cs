using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the target lost condition takes a target and then when prompted will tell an ai controller if the target should be lost
//this could be a case of time passed
//of being too far a distance away
//of losing sight of the target for too long
//and many more
public abstract class TargetLostCondition {

	//the current target
	protected GameObject target = null;

	//the method to be called when a new target is found
	public void TargetFound(GameObject target) {
		this.target = target;
	}

	//method to be called to check if the target should be lost
	//if this is found to be true then this condition should set its current target variable to null
	public abstract bool CheckTargetLost();
}
