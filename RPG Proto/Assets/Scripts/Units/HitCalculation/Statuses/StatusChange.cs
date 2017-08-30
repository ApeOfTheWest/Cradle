using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class represents a change in the status of a unit, it is added to an abiliti's hit packet to cause a status effect change
//this can be a cure as well as an affliction
//any changes in status will be added after the damage step
public abstract class StatusChange : ScriptableObject {

	//performs the changing of status on the unit, this do anything, but usually takes the form of adding an affliction like poison
	//the source is required as well as the target, in case the status calculation depends on the source
	public abstract void ChangeStatus(UnitController target, UnitController source);
}
