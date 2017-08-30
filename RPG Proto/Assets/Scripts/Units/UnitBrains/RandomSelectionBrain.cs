using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the random selection brain does nothing but select from all availiable abilities equally randomly and is the laziest way to set up a turn brain
//this shouldn't be used beyond prototype due to the awkwardness of transfering abilities from one brain to another
//instead some kind of decision making ai brain should be used
[CreateAssetMenu(fileName = "RandomSelectionBrain", menuName = "TurnBrains/RandomSelection", order = 1)]
public class RandomSelectionBrain : TurnBrain {
	//each turn brain has a list of all the abilities availiable to it, this must be cooled down each turn and checked for availiability against the unit using it
	[SerializeField]
	protected List<AbilitySlot> abilities = new List<AbilitySlot>();
	//provide way to get the abilities so that they can be examined
	public List<AbilitySlot> Abilities {
		get {
			return abilities;
		}
	}

	public override AbilitySlot[] GetAbilitySlotsInUse() {
		return abilities.ToArray ();
	}

	public override void ChooseAndUseAbility(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits, 
		UnitController[] leftReserve, UnitController[] rightReserve) {
		//before anything else do a check on which abilities are availiable to use
		CheckAbilityAvailiability(user, leftUnits, rightUnits);

		//now collect together all the usable abilities
		List<AbilitySlot> usableAbilities = new List<AbilitySlot>();
		for (int i = 0; i < abilities.Count; i++) {
			if (abilities [i].usable == true) {
				usableAbilities.Add (abilities [i]);
			}
		}

		AbilitySlot chosenAbility;

		//choose the ability with a randomiser
		//consider the case where no abilities are availiable
		if (usableAbilities.Count > 0) {
			//and choose a random number to select which one to use
			int abilityToUse = Random.Range (0, usableAbilities.Count - 1);
			chosenAbility = usableAbilities [abilityToUse];

			//look at which type of ability is being used and use that to decide who to target
			//do this using the built in method
			//test if it failed
			if (chosenAbility.ability.AutoAimUse (user, leftUnits, rightUnits) == false) {
				if (doNothingAbility != null) {
					//use the ability that shows no other abilities are availiable
					doNothingAbility.UseAbility (user);
				} else {
					//if no ability is availiable then just tell the unit that the ability use has stopped
					user.AbilityStopped ();
				}
			} else {
				//if it worked take away the associated resources
				chosenAbility.UseAbilityResources(user);
			}
		} else if (doNothingAbility != null) {
			//use the ability that shows no other abilities are availiable
			doNothingAbility.UseAbility (user);
		} else {
			//if no ability is availiable then just tell the unit that the ability use has stopped
			user.AbilityStopped();
		}
	}
}
