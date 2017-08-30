using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//intened to be derived from, rally level takes changes in level of rally and displays the change
public interface RallyLevel {

	//general methods each level should have

	//called every update, value may not have changed so check against last cached value before doing a transition
	void UpdateLevel(int newLevel);

	//called to set the display rather than update it, should avoid transitioning, usually called at start of batlle
	void SetLevel(int newLevel);

	//call when resetting the level display
	void ResetDisplay();
}
