using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a cone is just a vision radius with an angular restriction
public class VisionCone : VisionRadius {

	//the field of view to use for the target detection
	//measured in angles
	public float fieldOfView = 90f;

	//the current direction vector of the vision cone
	public Vector2 coneDirection = new Vector2(0, -1);

	override public void OnTriggerStay2D(Collider2D other) {
		//check if target is within the cone before checking if it is obscured (cheaper check)

		if (CheckInCone (other.gameObject) == true && CheckObscured (other.gameObject) == false) {
			TargetDetected (other.gameObject);
		}
	}

	private bool CheckInCone(GameObject potentialTarget) {
		//get the direction from the cone start to the target
		Vector2 direction = potentialTarget.transform.position - transform.position;
		//get the angle between the cone direction and the direction to target
		float angle = Vector2.Angle(direction, coneDirection);

		//check if the angle from the target to the centre of the field of view is greater than half of the total field of view
		//if so then the target must be out of the vision cone
		if (angle * 2f > fieldOfView) {
			return false;
		} else {
			return true;
		}
	}
}
