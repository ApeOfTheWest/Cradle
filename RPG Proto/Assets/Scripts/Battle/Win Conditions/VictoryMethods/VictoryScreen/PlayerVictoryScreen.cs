using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVictoryScreen : VictoryUI {
	private BattleController battleRef;

	public override void SetupUI (BattleController battle, bool leftVictory) {
		battleRef = battle;
	}

	//on each update check for a key press, end the battle if true
	void Update() {
		if (Input.anyKey) {
			//on any key end the battle
			battleRef.ActiveBattle.BattleEnd();
		}
	}
}
