using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a script to attach to something that needs to know when units are hovered over
//when units are moved away from
//and when the mouse is clicked while above a unit should report on the unit clicked on
//options for selections to be made using keys must also be included but this unit is only focussed on the mouse movements
public class MouseUnitSelector : MonoBehaviour {
	//the camera to use for the selection process
	public Camera selectionCamera = null;
	//the movelist to notify with selection messages
	public MoveList parentMoveList;

	//whether deadunits should be selected or not
	public bool selectDead = false;

	// Update is called once per frame
	void Update () {
		//check if the mouse has moved, if so look for a new selection using a raycast at its position
		if (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0) {
			Collider2D[] hitCollider = Physics2D.OverlapPointAll (selectionCamera.ScreenToWorldPoint(Input.mousePosition));

			UnitController unit = null;
			//get the lowest z collider that holds a unit and isnt dead if select dead is false
			for (int i = 0; i < hitCollider.Length; i++) {
				unit = hitCollider[i].gameObject.GetComponent<UnitController>();

				if (unit != null) {
					//if selected dead is false then check if unit is dead
					if (selectDead == false && unit.IsDead == true) {
						//clear selected unit
						unit = null;
					} else {
						//if selected dead is not true then break instantly and accept the unit
						//also do this if is dead is not true then break too
						break;
					}
				}
			}

			if (unit != null) {
				parentMoveList.SelectUnit(unit);
			} else {
				//clear the selection
				parentMoveList.DeselectUnit();
			}
		}
	}
}
