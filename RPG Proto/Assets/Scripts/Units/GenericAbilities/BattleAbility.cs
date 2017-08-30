using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for all battle abilities that can be used by units in battle
//different battle abilities will have different methods of targeting (single, multi, self) etc and so must inherit different versions of the use ability method
public abstract class BattleAbility : ScriptableObject {
	//the name of the ability
	public string abilityName = "";

	//used as a final check to ensure that the ability can target the given unit, usually this just ensures friend / enemy specific abilities only target those specifics
	//but it can also hold more complex conditions (for abilities that can target only one specific monster)
	//takes the user of the ability so friend / foe / self stuff can be checked
	//also takes the target
	public abstract bool CheckValidTarget(UnitController user, UnitController target);

	//method used to check and ensure that there are suitable targets for a selected ability before an ai decides to try and use it
	//this is needed to eliminate branches in decision making
	//takes in the unit using the ability and the active left and right units
	//as this is ai use only it should take into account suggested rather than intended targets
	//this method is virtual as it checks every single unit and certain abilities wont target every unit, and so a more efficient check can be used
	public virtual bool CheckSuitableTargets(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits) {
		//iterate through all targets using the check valid target function
		//if there is one suitable target then this function can instantly return
		for (int i = 0; i < leftUnits.Length; i++) {
			if (CheckValidTarget(user, leftUnits[i])) {
				return true;
			}
		}

		for (int i = 0; i < rightUnits.Length; i++) {
			if (CheckValidTarget(user, rightUnits[i])) {
				return true;
			}
		}

		//return false if no valid target was found 
		return false;
	}

	//each ability type has a method for targeting randomly from the list of suitable targets, this is to help ai
	//this method requires all active unit information
	//returns false if no suitable targets are found
	public abstract bool AutoAimUse(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits);
}
