using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the intro holder may be tasked with the playing of a battle intro
//it may be tasked with the 
public class IntroHolder : MonoBehaviour {

	//called when the intro is reported to have finished, destroy any objects left in the holder
	public void CleanupIntro() {
		int children = transform.childCount;

		for (int i = children - 1; i >= 0; i--) {
			//cache the child
			GameObject child = transform.GetChild (0).gameObject;

			//orphan the child
			child.transform.parent = null;
			//and destroy it
			Destroy(child);
		}
	}

}
