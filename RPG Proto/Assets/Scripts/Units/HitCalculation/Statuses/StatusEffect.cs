using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//root class of all standard status effects and debuffs / buffs
public abstract class StatusEffect {

	//called when the status effect is added
	//use to hook up any needed event listeners
	public abstract void StatusAdded(UnitController unit);

	//called when the status effect is removed, remove this from event listener as cleanup
	public abstract void StatusRemoved(UnitController unit);

	//called at the start of a unit this is attached to's turn, in case any effects should activate at the start
	public abstract void UnitTurnStarted(UnitController unit);
	//NOTE: the turncounter for this status effect will be decremented after unitturnstarted is called
	//meaning effects will expire between calls of unit turn started and turn ended

	//called at the end of a unit this is attached to's turn, in case effect should activate at the end
	public abstract void UnitTurnEnded(UnitController unit);
}
