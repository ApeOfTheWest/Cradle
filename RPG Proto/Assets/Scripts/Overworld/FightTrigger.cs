using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fight trigger object, attach to the same gameobject as a bunch of trigger colliders
//when a trigger event is detected between this and an object that can fight it then the linked fight will be dispatched
//the initiative will be decided between this and the challenger
public class FightTrigger : MonoBehaviour {
	public Fight fightToTrigger;

	//whether this trigger should give advantage to the fighters being triggered if the enemy doesn't have initiative
	public bool homeInitiative = false;

	//often times the monster that starts the fight should be destroyed when a fight is started
	//set a game object here to delete it when a fight is started (or leave blank)
	public GameObject deleteOnFight = null;

	void OnTriggerEnter2D(Collider2D other) {
		//if the other collider has a fight challenger then start the fight
		FightChallenger check = other.gameObject.GetComponent<FightChallenger> ();

		if (check != null) {
			//start a fight
			LevelController controlRef;
			UnitController[] awayUnits;
			UnitController[] awayReserve;
			InitiativeStrike toStrike;
			VictoryMethod awayVictoryMethod;

			check.GetChallengerConditions (out controlRef, out awayUnits, out awayReserve, out awayVictoryMethod, out toStrike);

			BattleController.InitiativeState initiative = BattleController.InitiativeState.Neutral;

			if (homeInitiative == true) {
				initiative = BattleController.InitiativeState.RightAdvantage;
			}

			//if initiative strike is given then take that to mean the challenger has initiative
			if (toStrike != null) {
				initiative = BattleController.InitiativeState.LeftAdvantage;
			}

			//call the fight
			fightToTrigger.ChallengeFight (controlRef, awayUnits, awayReserve, awayVictoryMethod, initiative, toStrike);

			//delete the set object
			if (deleteOnFight != null) {
				Destroy (deleteOnFight);
			}
		}

	}
}
