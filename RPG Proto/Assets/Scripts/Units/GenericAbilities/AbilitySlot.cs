using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilitySlot {
	//holds a reference to an ability along with some required metadata, such as any cooldowns needed for the ability and any costs (in mana or health)
	//think of this as the interface between a shared ability and the unit using it, this is what balances and manages the costs
	//while the ability describes what happens

	//the ability
	public BattleAbility ability;

	//various possible costs and requirements of an ability

	//the remaining cooldown in turns
	//DO NOT serialize remaining cooldown, it shouldn't last between sessions
	[System.NonSerialized] 
	private int cooldownLeft = 0;
	public int CooldownLeft {
		get {
			return cooldownLeft;
		}
	}
	//the cooldown to place on the ability (can be 0)
	public int abilityCooldown = 0;

	//the minimum rally level requried to use an ability
	public int rallyRequired = 0;
	//the rally level expended when the ability is used, -1 means all of the rally is expended
	public int rallyExpended = 0;

	//the cost of an ability in magick
	public int magickCost = 0;
	//the cost of an ability in health (if magick isn't availiable an additional health cost may be taken)
	public int healthCost = 0;

	//a tag used to set whether an ability slot is to be used or not, this should be set based on the check availiability method to ensure theres enough resources
	//but it also should be based on whether there are suitable targets availiable (check targets method)
	//unusable abilities should then be disabled from ai and player ability pickong, and alternatives chosen
	public bool usable = true;

	//cooldown the ability by the given amount
	public void Cooldown(int cooldownAmount) {
		cooldownLeft -= cooldownAmount;

		if (cooldownLeft < 0) {
			cooldownLeft = 0;
		}
	}

	//remove the resources to use the ability, doesn't check availiability so this must be done before hand
	//this must be called or else resources wont be used when using the ability
	//doesnt actually call the ability as this must be used by the unitcontroller with the appropriate targets selected
	public void UseAbilityResources(UnitController unitUsing) {
		//dont call change methods if the change is 0, as this may trigger some resource displays reduntantly
		if (rallyExpended > 0) {
			unitUsing.Rally.ChangeRallyLevel (-rallyExpended);
		}
		if (magickCost > 0) {
			unitUsing.Magick.ChangeMagick (-magickCost);
		}
		if (healthCost > 0) {
			unitUsing.Health.ChangeHealth (-healthCost);
		}
		//add the cooldown of the ability to the slot, plus 1 as the cooldown will tick down next turn
		cooldownLeft = abilityCooldown + 1;
	}

	//get whether the ability is availiable or not, take in a set of unit resources to do this with so that the costs can be properly evaluated
	public bool CheckAvailiable(UnitController unitUsing) {
		if (cooldownLeft <= 0 && unitUsing.Rally.RallyLevel >= rallyRequired
			&& unitUsing.Magick.CurrentMagick >= magickCost && 
			//ignore health cost if health cost is 0
			//this is to stop ai from locking up in wierd edge cases where their hp is 0 but they can still move
			(unitUsing.Health.CurrentHealth > healthCost || healthCost == 0)) {
			return true;
		} else {
			return false;
		}
	}
}
