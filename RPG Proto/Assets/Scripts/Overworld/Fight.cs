using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Fights/Basicfight")]
public class Fight : ScriptableObject {
	//the fight describes everything needed to start a battle, it is a scriptable object as one fight may be similarly described on a number of different triggers

	//the fight only describes one side of the units, the other set of units will be provided by whatever triggers the fight
	//the units are descibed by blueprints, and only initialised once a fight is actually started
	public UnitBlueprint[] activeUnits;

	//any reserve units for the fight
	public UnitBlueprint[] reserveUnits;

	//the base reward for winning the fight, this reward is always gained on a victory no matter what reward each unit holds
	public BattleReward baseReward;

	//the initiative strike to use if this side gains initiative
	public InitiativeStrike homeInitiativeStrike;

	//the intro to use for the fight (can be null, if null default will be used)
	public BattleIntro intro = null;

	//the win conditions for the home units
	public List<WinCondition> homeWin;
	//and the away units
	public List<WinCondition> awayWin;

	//the victory method of this fight
	public VictoryMethod homeVictory = null;

	//tells the fight whether the challenging units are to be placed on the left or right side
	public bool challengerLeftSide = true;

	//call when a battle is to be started using this fight as a base
	//pass in the challenging units (usually the player's units) and start the battle using them
	//also pass in the battle controller so that it can be used as a reference to start the fight

	//and say what initiative the fight has (challengers are always on the left so initiative left means the challengers have initiative)
	//if initiative is to the left then an initiative strike from the enemy must be included, otherwise use this initiative strike
	public void ChallengeFight(LevelController controlRef, 
		UnitController[] awayUnits, UnitController[] awayReserves, VictoryMethod awayVictory,
		BattleController.InitiativeState initiative, InitiativeStrike awayStrike = null) {

		//get a place to temporarily store the units from the battle controller
		GameObject tempHolder = controlRef.GetTempUnitStorage();

		//create the home units from the blueprints
		UnitController[] homeUnits = new UnitController[activeUnits.Length];

		for (int i = 0; i < homeUnits.Length; i++) {
			homeUnits [i] = activeUnits [i].MakeUnit(tempHolder);
		}

		//and same for the reserves
		UnitController[] homeReserves = new UnitController[reserveUnits.Length];

		for (int i = 0; i < homeReserves.Length; i++) {
			homeReserves [i] = reserveUnits [i].MakeUnit(tempHolder);
		}

		//find which initiative strike to use based on the initiative state
		InitiativeStrike strikeToUse = null;

		if (initiative == BattleController.InitiativeState.LeftAdvantage) {
			//use the challenger's initivativestrike
			strikeToUse = awayStrike;

			//if the challenger's side is flipped for this battle then flip the state
			if (challengerLeftSide == false) {
				initiative = BattleController.InitiativeState.RightAdvantage;
			}
		} else if (initiative == BattleController.InitiativeState.RightAdvantage) {
			//use the home strike
			strikeToUse = homeInitiativeStrike;

			if (challengerLeftSide == false) {
				initiative = BattleController.InitiativeState.LeftAdvantage;
			}
		}

		//choose which units are left and which are right based on the orientation of this battle
		UnitController[] leftActive;
		UnitController[] leftReserve;

		UnitController[] rightActive;
		UnitController[] rightReserve;

		//same with other variables
		List<WinCondition> leftWin;
		List<WinCondition> rightWin;
		VictoryMethod leftVictory;
		VictoryMethod rightVictory;

		if (challengerLeftSide == true) {
			leftActive = awayUnits;
			leftReserve = awayReserves;

			rightActive = homeUnits;
			rightReserve = homeReserves;

			leftWin = awayWin;
			leftVictory = awayVictory;

			rightWin = homeWin;
			rightVictory = homeVictory;
		} else {
			leftActive = homeUnits;
			leftReserve = homeReserves;

			rightActive = awayUnits;
			rightReserve = awayReserves;

			leftWin = homeWin;
			leftVictory = homeVictory;

			rightWin = awayWin;
			rightVictory = awayVictory;
		}

		//call the battle
		controlRef.SwitchToBattle(leftActive, rightActive, leftReserve, rightReserve, leftWin, rightWin, leftVictory, rightVictory,
			baseReward, initiative, strikeToUse, intro);
	}
}
