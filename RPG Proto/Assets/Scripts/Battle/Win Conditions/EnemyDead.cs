using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this condition becomes true when all the units on the opposing field are dead
public class EnemyDead : WinCondition {

	public override bool CheckCondition (BattleController battle, bool leftSide)
	{
		return true;
	}
}
