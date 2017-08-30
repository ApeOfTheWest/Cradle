using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attach to the same object as a trigger that can be touched to start a fight
//gives the fight some units to use as challengers and also gives a non null intiative strike if the challenger is to take the initiative
//if a null strike is given then it can be assumed that either the initiative is neutral or not in the challengers favour
public abstract class FightChallenger : MonoBehaviour {
	
	//the class must give a set of units and a battle controller when prompted, along with a strike if claiming initiative
	public abstract void GetChallengerConditions(out LevelController controlRef, out UnitController[] activeUnits, 
		out UnitController[] reserveUnits, out VictoryMethod victoryMethod, out InitiativeStrike strike);
}
