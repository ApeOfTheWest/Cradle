using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObstructor : MonoBehaviour {
	//add this to any obstruction object which may be added at runtime or moved at runtime and affects pathfinding

	//whether or not to update the pathfinding after the obstructor has been created (not needed for static spawn ins and enable / disable)
	//but needed for moving hazards
	public bool staticObstructor = true;
	//if not static then an update interval should be set to prevent excessive updating
	//(slower moving objects can have a longer interval, faster ones should be shorter)
	public float updateInterval = 0.2f;

	//the box length around the object that needs to be updated on mesh change
	public float updateBoundsSize = 3f;

	//cache the last position of this object, used to only update paths if the position is changed
	//also used to update the previous position to show obstructions may have been cleared
	private Vector3 previousPosition;

	//on enabling and disabling update the path graph
	void OnEnable() {
		//if this is static disable the script to prevent updating
		if (staticObstructor == true) {
			AstarPath.active.UpdateGraphs (new Bounds(transform.position, new Vector3(updateBoundsSize, updateBoundsSize, 0f)));
		} else {
			//cache current position
			previousPosition = transform.position;
			//start the coroutine
			StartCoroutine(UpdatePaths());
		}
	}
	void OnDisable() {
		AstarPath.active.UpdateGraphs (new Bounds(transform.position, new Vector3(updateBoundsSize, updateBoundsSize, 0f)));
	}

	private IEnumerator UpdatePaths()
	{
		//update this until the object is disabled
		while(gameObject.activeInHierarchy == true) 
		{ 
			//only update if position has changed
			if (transform.position != previousPosition) {
				AstarPath.active.UpdateGraphs (new Bounds(transform.position, new Vector3(updateBoundsSize, updateBoundsSize, 0f)));
				//update prev position as well
				AstarPath.active.UpdateGraphs (new Bounds(previousPosition, new Vector3(updateBoundsSize, updateBoundsSize, 0f)));

				previousPosition = transform.position;
			}
			yield return new WaitForSeconds(updateInterval);
		}
	}
}
