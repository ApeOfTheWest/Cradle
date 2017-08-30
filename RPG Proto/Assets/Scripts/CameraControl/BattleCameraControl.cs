using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the battle camera control provides some rendering callbacks to the ui system so that worldpace ui can be placed properly
//it also zooms and pans to keep essential elements of the battle system in view
public class BattleCameraControl : MonoBehaviour {
	//the battle, this should be set on awake by the battle component
	public BattleController battle;

	//the camera this is attached to, used to get the aspect ratio
	private Camera attachedCamera;
	public Camera AttachedCamera {
		get {
			return attachedCamera;
		}
	}
	//parralax camera control this is attached to, used to set audio listener depth
	private ParallaxCamControl cameraControl;

	//the framing hints in use by this controller
	//these hints are usually set by the scenery on startup and rarely include elements from elsewhere
	public CameraFramingHints hints = new CameraFramingHints();

	//the smoothing to apply when moving the camera
	public float smoothing = 5f;

	//the desired depth of the battle camera's audio listener, should be the middle of the units
	public float desiredAudioZ = 5f;

	void Awake() {
		attachedCamera = GetComponent<Camera> ();
		cameraControl = GetComponent<ParallaxCamControl> ();
	}

	//called just before a battle is to start being rendered, will snap the camera system to a starting position based on the essential rectangles
	public void StartOfBattle() {
		//snap camera to target position
		Vector3 cameraPos;
		float cameraSize;

		hints.CalculateTargetCameraConfig (attachedCamera.aspect, out cameraSize, out cameraPos);

		//set the camera size and position 
		attachedCamera.orthographicSize = cameraSize;
		attachedCamera.transform.position = cameraPos;
	}

	//called to reset the control at the end of a battle
	public void Reset() {
		hints.ClearHints ();
	}

	//on lateupdate change the camera position based on the hints
	void LateUpdate() {
		Vector3 cameraPos;
		float cameraSize;

		hints.CalculateTargetCameraConfig (attachedCamera.aspect, out cameraSize, out cameraPos);

		//interpolate from current position to target
		//lerp between the two
		Vector3 intPos = Vector3.Lerp(attachedCamera.transform.position, cameraPos, smoothing * Time.deltaTime);
		float intSize = Mathf.Lerp (attachedCamera.orthographicSize, cameraSize, smoothing * Time.deltaTime); ;

		//set the camera size and position 
		attachedCamera.orthographicSize = intSize;
		attachedCamera.transform.position = intPos;

		//change the depth of the audio listener to match the required depth
		cameraControl.SetAudioListenerZ(desiredAudioZ);
	}

	void OnPreCull() {
		//just before culling the ui that relates to a point in worldspace must be updated to position itself correctly
		//this cant be done in lateupdate because the camera moves in late update
		battle.ActiveBattle.battleUI.BattleCameraPreCull();
	}
}
