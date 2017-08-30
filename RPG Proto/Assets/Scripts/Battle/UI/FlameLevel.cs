using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameLevel : MonoBehaviour, RallyLevel {
	//the last known level, held for transition purposes
	private int lastLevel = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	//called every update, value may not have changed so check against last cached value before doing a transition
	public void UpdateLevel(int newLevel) {
		//transition between the colour of this level and the next
	}

	//called to set the display rather than update it, should avoid transitioning, usually called at start of batlle
	public void SetLevel(int newLevel) {
		//set the colour and cache the level

	}

	//call when resetting the level display
	public void ResetDisplay() {
		//set the last level back to 0
		SetLevel(0);
	}
}
