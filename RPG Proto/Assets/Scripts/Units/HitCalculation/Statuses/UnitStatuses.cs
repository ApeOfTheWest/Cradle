using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//holding class for all the current status effects on a unit
//has a list of buffs and debuffs
public class UnitStatuses {
	//slots for all the standard generic abilities


	//list of buffs
	public List<StatusEffect> buffs = new List<StatusEffect>();

	//list of debuffs
	public List<StatusEffect> debuffs = new List<StatusEffect>();

	//events called when buffs / debuffs are added or taken away
	//include a bool to show if it's a buff or a debuff
	public delegate void StatusChangeEvent(StatusEffect status, bool buff);

	public event StatusChangeEvent AddStatus;
	public event StatusChangeEvent RemoveStatus;

	//try to add the given buff
	public void AddBuff(StatusEffect status) {

	}

	//try to remove the buff at the given index (only works if it is curable)
}
