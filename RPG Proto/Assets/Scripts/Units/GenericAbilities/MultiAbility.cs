using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MultiAbility : TargetableAbility {

	//pass in the unit using this ability, and the multiple targets
	public abstract void UseAbility(UnitController user, UnitController[] targets);

	//as multi target abilities may be required to target things differently they need to also include an enum that decides what targeting mode is allowed
	public enum MultiTargetMode {
		//one group is allowed to be targeted
		//either the enemies or the allies
		GROUP, 
		//all active units must be targeted
		ALL, 
		//up to a selected number can be targeted from either side of the field
		//the number allowed will be returned in the target number if applicable
		SETNUMBER
	}

	//set the target mode in serialization but make it private
	[SerializeField]
	private MultiTargetMode multiTargetType = MultiTargetMode.GROUP; 

	//the target mode of the ability
	public MultiTargetMode MultiTargetType {
		get {
			return multiTargetType;
		}
	}

	//the number of targets allowed (only used in set number mode)
	[SerializeField]
	private uint targetNumber = 0;

	//the number of targets allowed if in setNumber mode (only overidden if applicable)
	public virtual uint TargetNumber {
		get {
			return targetNumber;
		}
	}

	public override bool AutoAimUse(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits) {
		List<UnitController> suitableTargets = new List<UnitController>();

		//check the preferred targets and select targets based on that
		//if the multitarget mode ALL is active then both targets must be selected by default


		//eliminate any additional restrictions using the check target function on each suitable target
		//iterate backwards so elements can be safely removed
		for (int i = suitableTargets.Count - 1; i >= 0; i--) {
			if (CheckValidTarget(user, suitableTargets[i]) == false) {
				//if target is rejected then remove it
				suitableTargets.RemoveAt(i);
			}
		}




		return false;
	}
}
