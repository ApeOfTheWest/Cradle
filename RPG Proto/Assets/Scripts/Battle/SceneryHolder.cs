using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the object this is attached to is used to hold battlescenery
public class SceneryHolder : MonoBehaviour {

	//method for clearing out all the childs attached to this scenery holder, automatically tells the scenery that it has been removed
	public void ClearScenery() {
		int children = transform.childCount;

		for (int i = children - 1; i >= 0; i--) {
			//cache the child
			GameObject child = transform.GetChild (0).gameObject;

			//orphan the child
			child.transform.parent = null;

			//try to get a battle scenery component from the child
			BattleScenery battle = child.GetComponent<BattleScenery>();
			//tell the scenery that it has been removed
			battle.SceneryRemoved();
		}
	}
}
