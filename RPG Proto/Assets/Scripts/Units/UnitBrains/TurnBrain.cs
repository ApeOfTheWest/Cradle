using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurnBrain : ScriptableObject {
	//the only thing a turn brain needs to do is come up with an action for the unit to do
	//whenever a units turn starts it will call upon its turn brain to perform any actions before handing control back to the battlecontroller
	//one game rule is that basic attacks are always availiable so that can be used in the case nothing else is availiable

	//the ability to use if no other abilities are availiable (will do nothing)
	public static SelfAbility doNothingAbility = null;

	//start the process of selecting an ability, passing in all currently active units and the user
	//this process may last longer than one frame so the function must eventually notify the user, and the user may be stored
	//this should be called after cooldowns are applied to get an accurate availiability measure
	public abstract void ChooseAndUseAbility(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits, 
		UnitController[] leftReserve, UnitController[] rightReserve);

	//abstract method that gets all of the abilities in use by the brain, must be abstract because different brains organize these differently
	public abstract AbilitySlot[] GetAbilitySlotsInUse();

	//method to cooldown all abilities by 1 turn
	public void CooldownAbilities(int cooldownAmount) {
		AbilitySlot[] abilities = GetAbilitySlotsInUse ();

		for (int i = 0; i < abilities.Length; i++) {
			abilities [i].Cooldown (cooldownAmount);
		}
	}
	//method to check which abilities are availiable, can eleminate ai branches based on this
	//must be given the user to check for required resources, and must also be given the entire active field to check for suitable targets
	//if the user is an ai then both conditions must be fulfilled for an ability to be selected
	//if the user is a player then even abilities with no valid targets can be selected, this is because the valid targets thing only accounts for suggested targets
	//the player may want to attack their own team
	public virtual void CheckAbilityAvailiability(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits) {
		AbilitySlot[] abilities = GetAbilitySlotsInUse ();

		for (int i = 0; i < abilities.Length; i++) {
			if (abilities [i].CheckAvailiable (user)
			    && abilities [i].ability.CheckSuitableTargets (user, leftUnits, rightUnits)) {
				abilities [i].usable = true;
			} else {
				abilities [i].usable = false;
			}
		}
	}
}
