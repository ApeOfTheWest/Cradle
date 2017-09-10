using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionRadius : TargetDetection {
	//a vision radius looks for any colliders in a radius from a point and then tests to see if they are obscured
	//on every fixedupdate (because it uses the physics system) the radius will fire off events with the colliders it finds

	//the circle collider to use as the vision radius, this should be a trigger
	//if a sort of layer masking is disired to choose what to detect then it should be done by changing the layer mask of this collider
	private CircleCollider2D vision;

	public override bool Enabled {
		get {
			return vision.enabled;
		}
		set {
			vision.enabled = value;
		}
	}

	// Use this for initialization
	void Awake () {
		//get the circle collider from the attached object
		vision = gameObject.GetComponent<CircleCollider2D>();
	}

	virtual public void OnTriggerStay2D(Collider2D other) {
		//trigger an event using the game object of the colliders found by the vision radius
		//first check using raycast to target to see if its obscured
		if (CheckObscured (other.gameObject) == false) {
			TargetDetected (other.gameObject);
		}
	}

	//method to check if a potential target is obscured
	protected bool CheckObscured(GameObject potentialTarget) {
		//get the vector from this to the target
		Vector2 vectorToTarget = (potentialTarget.transform.position - transform.position);
		//get the distance
		float distance = vectorToTarget.magnitude;

		//raycast from the centre of this vision radius to the target, and store the result
		RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorToTarget, distance, LayerMaskID.GetObscureMask());

		if (hit.collider == null) {
			return false;
		} else {
			return true;
		}
	}
}
