using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothing : SelfAbility {
	//ability used when no other abilities are availiable, or intentionally called to skip a turn
	//the entire ability usage can be fit into one coroutine so multiple instances are not necessary

	public override void UseAbility(UnitController user) {

	}

	public IEnumerator DisplayVisuals(UnitController user) {
		//play three dots appearing over the user's head


		return null;

		//after the visuals pass back control
		user.AbilityStopped();
	}
}
