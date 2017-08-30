using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a party member brain stores a nested set of abilities which will be displayed to the player for selection when choosing how to spend their turn
[CreateAssetMenu(fileName = "PartyMemberBrain", menuName = "TurnBrains/PartyMember", order = 1)]
public class PartyMemberBrain : TurnBrain {
	//the ability to list as the basic attack, any costs for this attack will be ignored because it is meant to be free
	public BattleAbility basicAttack;
	//the ability to list as the defend option, again costs are ignored
	public BattleAbility defend;

	public List<MoveListCategory> abilityCategories = new List<MoveListCategory>();

	public override void ChooseAndUseAbility(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits, 
		UnitController[] leftReserve, UnitController[] rightReserve) {
		//before anything else do a check on which abilities are availiable to use
		CheckAbilityAvailiability(user, leftUnits, rightUnits);

		//then display the movelist element of the battle ui so the player can pick which one to use, from here on the ui can handle selection and the brains job is done
		//do this using the user for a reference to the battle ui
		user.Battle.ActiveBattle.battleUI.ShowPlayerMoveList(this, user, leftUnits, rightUnits, leftReserve, rightReserve);
	}

	public override AbilitySlot[] GetAbilitySlotsInUse() {
		//make a list of all the abilities by iterating through the categories
		List<AbilitySlot> abilities = new List<AbilitySlot>();

		for (int i = 0; i < abilityCategories.Count; i++) {
			for (int j = 0; j < abilityCategories [i].moves.Count; j++) {
				abilities.Add (abilityCategories [i].moves [j]);
			}
		}

		//finally return it as an array
		return abilities.ToArray();
	}

	//the party member version of the check ability function should only check if resources are availiable
	//and not whether the targets are suggested
	public override void CheckAbilityAvailiability(UnitController user, UnitController[] leftUnits, UnitController[] rightUnits) {
		AbilitySlot[] abilities = GetAbilitySlotsInUse ();

		for (int i = 0; i < abilities.Length; i++) {
			if (abilities [i].CheckAvailiable (user)) {
				abilities [i].usable = true;
			} else {
				abilities [i].usable = false;
			}
		}
	}

	//
}
