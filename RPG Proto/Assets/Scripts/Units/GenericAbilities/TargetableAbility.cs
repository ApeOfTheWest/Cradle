using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the targetable ability is just any ability that has a choice of targets
//this only excludes the self ability which can only be cast on oneself
public abstract class TargetableAbility : BattleAbility {
	//the battle ability must return an intended target to guide targeting systems
	//this says whether an ability should be aimed at an enemy, ally or both
	public enum AbilityTarget {
		Enemy, Ally, Both
	}

	//whether the ability can target dead people or not
	//should usually be false except for revival spells
	public bool targetDead = false;

	//preferred target should be set in the inspector and left at runtime, so make it serialized and private
	//NOTE: preferred target does not limit targetting options but if an ai cant find its preffered target it should take no move (or risk hitting own team)
	[SerializeField]
	private AbilityTarget preferredTarget = AbilityTarget.Enemy;

	//ensure that each ability can tell ai and players alike who the intended target is
	//(not mandotory target, hostile spells can be cast on allys and vice versa)
	//this can be used to auto target the right side when the player selects an ability
	//or for ai to tell it what to prefer targetting (ally or enemy)
	//both is valid but unhelpful for this purpose, as it will make the ability choose between friend and foe at random (for certain abilities)
	public AbilityTarget PreferredTarget {
		get {
			return preferredTarget;
		}
	}

	//the valid targets of the ability
	//this can be used to limit the ability to target enemies, allies or have no limit on targets
	[SerializeField]
	private AbilityTarget validTarget = AbilityTarget.Enemy;

	public AbilityTarget ValidTarget {
		get {
			return validTarget;
		}
	}

	//by default the check valid target function only checks against the valid target enum and nothing else
	public override bool CheckValidTarget(UnitController user, UnitController target) {
		//if target is dead and target cant target the dead then return false
		if (target.Health.CurrentHealth > 0 || targetDead == true) {
			//if targets are valid then return true
			if (validTarget == AbilityTarget.Both) {
				return true;
			} else if (validTarget == AbilityTarget.Enemy) {
				//check if target is an enemy
				if (user.LeftSide != target.LeftSide) {
					return true;
				} else {
					return false;
				}
			} else if (validTarget == AbilityTarget.Ally) {
				if (user.LeftSide == target.LeftSide) {
					return true;
				} else {
					return false;
				}
			}
		}

		//return false if no case exists
		return false;
	}


	//if the valid target of an ability is known then only certain sets of the units may need to be checked
	public override bool CheckSuitableTargets(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits) {
		//if both targets are availiable then use the base method
		if (validTarget == AbilityTarget.Both) {
			return base.CheckSuitableTargets (user, leftUnits, rightUnits);
		} else if (validTarget == AbilityTarget.Enemy || validTarget == AbilityTarget.Ally) {
			//whether to check the left or right units, based on the valid target setting and the side of the user
			bool checkLeft = true;

			if (validTarget == AbilityTarget.Enemy) {
				//find which side the user is on and check the opposite one
				checkLeft = !user.LeftSide;
			} else {
				//and vice versa
				checkLeft = user.LeftSide;
			}

			if (checkLeft) {
				for (int i = 0; i < leftUnits.Length; i++) {
					if (CheckValidTarget(user, leftUnits[i])) {
						return true;
					}
				}
			} else {
				for (int i = 0; i < rightUnits.Length; i++) {
					if (CheckValidTarget(user, rightUnits[i])) {
						return true;
					}
				}
			}
		}
			
		//return false if no valid target was found 
		return false;
	}
}
