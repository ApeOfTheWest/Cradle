using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//put on camera that renders first, update all cameras in construct
public class ParallaxCamPreCuller : MonoBehaviour {
	public ParallaxCamControl camControl;

	void OnPreCull() {
		camControl.UpdateAttachedCameras ();
	}
}
