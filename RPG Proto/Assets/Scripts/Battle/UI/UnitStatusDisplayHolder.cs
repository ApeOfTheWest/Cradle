using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatusDisplayHolder : MonoBehaviour {
	//the unit status display holder is used to contain all the unit status bars that pop up when a unit is hit with something or when they are being targeted
	//there will be one display for each unit and it will position itself above each unit's head (based on where the given camera is looking)
	//the displays will automatically enable themselves when the unit's status changes and then disable themselves after some time

	//the prefab to initialise them from
	[SerializeField]
	private UnitStatusDisplay templateDisplay;

	//the camera that is being used to render the units (needed to layer ui over worldspace)
	public Camera unitRenderCamera;

	//the list of currently active displays
	private List<UnitStatusDisplay> activeDisplays = new List<UnitStatusDisplay>();

	public void BattleCameraPreCull () {
		//notify the status displays to update their positions
		for (int i = 0; i < activeDisplays.Count; i++) {
			activeDisplays [i].CameraPreCull ();
		}
	}

	public void AddUnitStatusDisplay(UnitController unit) {
		//add and initialise a new unit display as a child of this object
		UnitStatusDisplay newDisplay = Instantiate(templateDisplay, transform);
		newDisplay.AttachToUnit (unit);
		newDisplay.displayRenderCamera = unitRenderCamera;

		activeDisplays.Add (newDisplay);

		//when first added the display should be inactive
		newDisplay.gameObject.SetActive(false);
	}
	public void RemoveUnitStatusDisplay(UnitController unit) {
		//iterate through the list and remove all instances of the given unit
		for (int i = 0; i < activeDisplays.Count; i++) {
			if (activeDisplays [i].AttachedUnit == unit) {
				//remove the indexed unit
				activeDisplays.RemoveAt(i);
				//and then decrement the counter by 1 to represent the removal so duplicates can be searched for
				i--;
			}
		}
	}
}
