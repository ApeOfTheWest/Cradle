using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelfAbility : BattleAbility {

	//pass in the unit using this ability, no target is required as this is a self using ability
	public abstract void UseAbility(UnitController user);

	public override bool CheckValidTarget(UnitController user, UnitController target) {
		//for a self ability the only usual requirement is that the user equals the target
		if (user == target) {
			return true;
		} else {
			return false;
		}
	}

	//override the check suitable targets function to only check this unit for availiability, for most self abilities this is redundant
	//but some may have special conditions
	public override bool CheckSuitableTargets(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits) {
		return CheckValidTarget (user, user);
	}

	public override bool AutoAimUse(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits) {
		//for self abilities just use on itself
		UseAbility(user);

		return true;
	}
}
