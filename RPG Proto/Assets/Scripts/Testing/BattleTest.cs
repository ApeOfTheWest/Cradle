using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class will automatically start a battle between the two given parties
//both the win and lose conditions will just restart the scene but display different messages
public class BattleTest : MonoBehaviour {
	public UnitController[] leftUnits;
	public UnitController[] rightUnits;
	public UnitController[] leftReserves;
	public UnitController[] rightReserves;
	public List<WinCondition> leftWin;
	public List<WinCondition> rightWin;

	public VictoryMethod win;
	public VictoryMethod lose;

	//in the update code disable this so another update wont be made and then start the battle
	void Update () {
		LevelController.levelControllerInstance.SwitchToBattle (leftUnits, rightUnits, leftReserves, rightReserves,
			leftWin, rightWin,
			//what to do on winning
			win, lose,
			null, BattleController.InitiativeState.Neutral, null);

		this.enabled = false;
	}
}
