using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLostDistance : TargetLostCondition {
	//a form of target loss that ocurrs at a certain distance

	private float targetLostDistance = 4f;
	//a squared distance is cached for ease and efficiency
	private float distanceSquared = Mathf.Pow(4f, 2f);

	public float lossDistance {
		get {
			return targetLostDistance;
		}
		set {
			targetLostDistance = value;
			distanceSquared = Mathf.Pow(targetLostDistance, 2f);
		}
	}


	//the game object to measure the target distance by
	public GameObject targetMeasure = null;

	public override bool CheckTargetLost() {
		//catch for if no target exists
		if (target != null) {
			//check the distance between the target and the target measure
			Vector2 distanceVector = target.transform.position - targetMeasure.transform.position;
			//use the distance squared as the calculation is faster
			if (distanceVector.sqrMagnitude < distanceSquared) {
				return false;
			} else {
				//the target is lost
				target = null;
				return true;
			}
		} else {
			return true;
		}
	}
}
