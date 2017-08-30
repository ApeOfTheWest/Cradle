using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface RallyBar {

	//called every update, value may not have changed so check against last cached value before doing a transition
	void UpdateRallyPercent(float newLevel);

	//called to set the display rather than update it, should avoid transitioning, usually called at start of batlle
	void SetRallyPercent(float newLevel);

	//call when resetting the level display
	void ResetDisplay();
}
