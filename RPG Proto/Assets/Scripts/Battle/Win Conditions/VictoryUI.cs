using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a type of class that attaches to a victory ui object, takes in the state of the battle system so certain things can be set
public abstract class VictoryUI : MonoBehaviour {

	public abstract void SetupUI (BattleController battle, bool leftVictory);
}
