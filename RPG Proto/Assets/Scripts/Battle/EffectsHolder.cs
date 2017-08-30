using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the gameobject this is attached to is a simple holder for temporary visual effects that will last for some time and then fade
//these effects such as hitsparks, must be not be attached to a unit as they exist and move sperate to the unit that created them
//however they must be attached to some object in the battle system to make sense in the heirchy, hence the need for this holder
public class EffectsHolder : MonoBehaviour {
	//called when a battle is ended, destroys any gameobjects still attached to this
	public void DestroyRemainingEffects() {
		
		int attachedObjectnumber = transform.childCount;

		for (int i = 0; i < attachedObjectnumber; i++) {
			Destroy(transform.GetChild (0).gameObject);
		}
	}
}
