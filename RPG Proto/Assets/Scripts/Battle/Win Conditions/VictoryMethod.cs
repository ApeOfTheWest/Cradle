using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//victory methods show what should be done when a team wins a battle
public abstract class VictoryMethod : ScriptableObject {

	//play out a certain sequence of events based on the victory
	//pass in the battle so it can be used, this object will need to call a battle end when necessary
	public abstract void VictoryPlay(BattleController battle, bool leftVictory);
}
