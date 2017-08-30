using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIconHolder : MonoBehaviour {
	//the selection arrows, place above the current selection

	//the secondary selection arrors, place above anyone who would be targetted as a result of the current selection

	//target lock icon, place on a currently stored target of an ability

	//current gameobjects holding the icons for a unit, paired with units
	//sort these according to the z position of each unit to ensure proper rendering

	private class IconUnit {
		public IconUnit(UnitController unit, GameObject iconHolder) {

		}

		//the z position of the unit at the time this was made
		private float zPosition = 0f;
	}

	//place a selection icon on the given unit, if a selection icon already exists then move it to the new unit
}
