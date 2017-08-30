using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the fight challenger called when a player's body is hit, doesn't take initiative
public class PlayerContactChallenger : FightChallenger {
	private LevelController control;

	private ActiveParty playerParty;

	//the victory method to use if this party wins, should be a victory screen that loots items and exp
	public VictoryMethod playerVictoryMethod;

	public override void GetChallengerConditions (out LevelController controlRef, out UnitController[] activeUnits, out UnitController[] reserveUnits, 
		out VictoryMethod victoryMethod, out InitiativeStrike strike)
	{
		controlRef = control;
		activeUnits = playerParty.CollateActiveUnits();
		reserveUnits = playerParty.CollateReserveUnits();
		strike = null;
		victoryMethod = playerVictoryMethod;
	}

	//set the battle controller reference
	public void SetControl(LevelController controlRef) {
		control = controlRef;
	}

	//set the player party reference
	public void SetActiveParty(ActiveParty party) {
		playerParty = party;
	}
}
