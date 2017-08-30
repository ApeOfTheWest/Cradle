using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OverlayUIVictory", menuName = "VictoryMethods/OverlayUI", order = 1)]
public class OverlayUIVictory : VictoryMethod {
	//this is a generalized victory method that just overlays a victory ui prefab instance on victory
	//can be used for game over and victory screens
	public VictoryUI templateUI;

	public override void VictoryPlay(BattleController battle, bool leftVictory) {
		//make a new instance of the template ui as a child of the overlay holder
		VictoryUI ui = Instantiate(templateUI, battle.ActiveBattle.OverlayUIHolder.transform);
		ui.SetupUI (battle, leftVictory);
	}
}
