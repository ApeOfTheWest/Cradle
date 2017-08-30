using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//win condition takes the current state of the battle along with the side that the condition is for (left or right)
//it then returns a bool stating whether the condition is fulfilled or not
public abstract class WinCondition : ScriptableObject {

	public abstract bool CheckCondition(BattleController battle, bool leftSide);
}
