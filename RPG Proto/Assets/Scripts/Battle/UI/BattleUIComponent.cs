using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//every battle UI component must derive from this interface, provides a method for setting itself up that must be called at the start of each fight
public interface BattleUIComponent {

	void SetupFromBattle(BattleController battle);
}
