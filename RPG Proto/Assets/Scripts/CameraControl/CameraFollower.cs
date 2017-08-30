using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {
	public Transform toFollow;

	//get the parralax component this is attached to so the audio listener can be moved
	private ParallaxCamControl camControl;

	void Awake() {
		camControl = GetComponent<ParallaxCamControl> ();
	}

	//use lateupdate to move camera after objects are done moving themselves
	void LateUpdate () {
		gameObject.transform.position = new Vector3 (toFollow.position.x, toFollow.position.y, gameObject.transform.position.z);
		camControl.SetAudioListenerZ (toFollow.transform.position.z);
	}
}
