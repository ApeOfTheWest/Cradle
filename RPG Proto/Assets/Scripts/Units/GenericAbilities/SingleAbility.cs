using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a type of ability that targets a single unit (of either side)
public abstract class SingleAbility : TargetableAbility {
	//pass in the unit using this ability, and the target
	public abstract void UseAbility(UnitController user, UnitController target);

	public override bool AutoAimUse(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits) {
		//for single abilities get the list of suitable targets by the preferred target
		List<UnitController> suitableTargets = new List<UnitController>();

		if (PreferredTarget == AbilityTarget.Both) {
			//both targets being suitable simplifies things
			for (int i = 0; i < leftUnits.Length; i++) {
				suitableTargets.Add (leftUnits [i]);
			}
			for (int i = 0; i < rightUnits.Length; i++) {
				suitableTargets.Add (rightUnits [i]);
			}
		} else {
			//otherwise alligeonces must be checked
			//get the side that the user is on
			bool userLeftSide = user.LeftSide;

			if (PreferredTarget == AbilityTarget.Ally) {
				//add only allies
				if (userLeftSide == true) {
					for (int i = 0; i < leftUnits.Length; i++) {
						suitableTargets.Add (leftUnits [i]);
					}
				} else {
					for (int i = 0; i < rightUnits.Length; i++) {
						suitableTargets.Add (rightUnits [i]);
					}
				}
			} else if (PreferredTarget == AbilityTarget.Enemy) {
				//add only enemies
				if (userLeftSide == true) {
					for (int i = 0; i < rightUnits.Length; i++) {
						suitableTargets.Add (rightUnits [i]);
					}
				} else {
					for (int i = 0; i < leftUnits.Length; i++) {
						suitableTargets.Add (leftUnits [i]);
					}
				}
			}

		}

		//eliminate any additional restrictions using the check target function on each suitable target
		//iterate backwards so elements can be safely removed
		for (int i = suitableTargets.Count - 1; i >= 0; i--) {
			if (CheckValidTarget(user, suitableTargets[i]) == false) {
				//if target is rejected then remove it
				suitableTargets.RemoveAt(i);
			}
		}

		//now if no suitable targets remain then return false
		if (suitableTargets.Count == 0) {
			return false;
		} else {
			//otherwise choose a random target and attack it
			int chosenTarget = Random.Range(0, suitableTargets.Count -1);

			UseAbility (user, suitableTargets [chosenTarget]);

			return true;
		}
	}
}
