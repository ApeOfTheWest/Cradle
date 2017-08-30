using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreenUI : VictoryUI {
	private BattleController battleRef;

	public override void SetupUI (BattleController battle, bool leftVictory) {
		battleRef = battle;
	}

	//on each update check for a key press, end the battle if true
	void Update() {
		if (Input.anyKey) {
			//on any key go back to start menu
			//just reload current scene
			int scene = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene (scene, LoadSceneMode.Single);
			//battleRef.ActiveBattle.BattleEnd();
		}
	}
}
