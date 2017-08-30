using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidgroundOrderer : MonoBehaviour {
	//the generic midground orderer class, this component can be attached to a game object to automatically update its z-position for render ordering purposes
	//the bounds of this midground plane must be known and as such public static variables will be held that allow the user to store the min y-positions and y-range (max minus min) of the current room
	//the starting z and size of the z range availiable must also be set

	//it should be noted that the transform of a midground orderer will often be taken as the y coordinate to set the z-position with

	public static float minYposition = -2000f;
	public static float yRange = 4000f;

	public static float startingZposition = 0f;
	public static float zRange = 10f;

	//takes in a y value and calculate the appropriate z-value for it
	public static float calcZposition(float yPosition) {
		float zPosition = startingZposition;

		//get the percentage of the y range being taken up
		float yRangePercent = (yPosition - minYposition) / yRange;
		//clamp the percent
		if (yRangePercent < 0f) {
			yRangePercent = 0f;
		} else if (yRangePercent > 1f) {
			yRangePercent = 1f;
		}

		//and transform it into a point in the valid zRange
		zPosition += yRangePercent * zRange;

		return zPosition;
	}

	//method to set the z-position of the parent of the gameobject holding this component based on the y position of this orderer
	public void UpdateParentZ() {
		//set the z position of the object this is attached to using the y position of this orderer

		float newZ = calcZposition(transform.position.y);
		gameObject.transform.parent.transform.localPosition = new Vector3 (gameObject.transform.parent.transform.localPosition.x,
			gameObject.transform.parent.transform.localPosition.y,
			newZ);

	}
}
