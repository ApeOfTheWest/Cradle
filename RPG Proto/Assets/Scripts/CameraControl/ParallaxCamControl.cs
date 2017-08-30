using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the parallax cam control ensures that all 3 camera views being used sync up so that an object in the plane z=0 moves at the same speed in all views
//this needs to be updated every time the size of the orthographic camera this is attached to has its size changed
public class ParallaxCamControl : MonoBehaviour {
	//references to the two perspective cameras and the orthographic ones
	private Camera orthoCamera;
	public Camera OrthoCamera {
		get {
			return orthoCamera;
		}
	}
	private Camera nearParallax;
	private Camera farParallax;
	private Camera backgroundOrtho;

	//the audio listener, this is held in its own gameobject because it will often need to be on the same z plane as the player to hear sounds blended properly
	private AudioListener audioListener;

	private float lastKnownZoom = 0f;
	private float lastKnownZaxis = 0f;

	void Awake () {
		orthoCamera = gameObject.GetComponent<Camera>();
		audioListener = GetComponentInChildren<AudioListener> ();
		nearParallax = transform.Find("Parallax Near").GetComponent<Camera>();
		farParallax = transform.Find ("Parallax Far").GetComponent<Camera> ();
		backgroundOrtho = transform.Find ("Background Orthographic").GetComponent<Camera> ();

		//set an event listener to notify this script whenever the size of the orthographic camera changes
		UpdatePerspectiveViewsZoom();
		UpdatePerspectiveViewsMoveZ ();
		//store the camera zoom
		lastKnownZoom = orthoCamera.orthographicSize;
		lastKnownZaxis = orthoCamera.transform.position.z;

		//Debug.Log ("Camera views updated");
	}

	//method to set the z of the audio listener component, used to get accurate blending by putting the z on the same plane as the object of interest
	//the x and y should stay attached to the camera
	public void SetAudioListenerZ(float z) {
		Vector3 audioPos = transform.position;
		audioPos.z = z;
		audioListener.transform.position = audioPos;
	}

	//camera moving and resizing often happens late update
	//but if done in onprecull then any size changes wont register till next frame
	//the onprecull must be called from the first camera to undergo furstrum culling or else change made here wont matter

	/*void OnPreCull() {
		//check if zoom has changed and update perspective fields if it has
		if (lastKnownZoom != orthoCamera.orthographicSize) {
			UpdatePerspectiveViewsZoom ();
		}
		//similarly check if z position has changed
		if (lastKnownZaxis != lastKnownZaxis) {
			UpdatePerspectiveViewsMoveZ ();
		}
	}*/

	//update the cameras attached to this one, must be called from on precull of camera with lowest depth
	public void UpdateAttachedCameras() {
		//check if zoom has changed and update perspective fields if it has
		if (lastKnownZoom != orthoCamera.orthographicSize) {
			//change last known vars so this doesnt update repeatedly
			lastKnownZoom = orthoCamera.orthographicSize;
			UpdatePerspectiveViewsZoom ();
		}
		//similarly check if z position has changed
		if (lastKnownZaxis != orthoCamera.transform.position.z) {
			lastKnownZaxis = orthoCamera.transform.position.z;
			UpdatePerspectiveViewsMoveZ ();
		}
	}



	//called just before camera culling takes place, if the camera has moved in a way that requires the perspective cameras to be updated this will update it
	/*void OnPreCull() {

		if (cameraDirty == true) {
			//update perspectives
			UpdatePerspectiveViewsZoom();

			cameraDirty = false;
			Debug.Log ("Camera views updated");
		}
	}*/

	//updates the perspective field of view after zooming the camera
	public void UpdatePerspectiveViewsZoom() {
		float orthoSize = orthoCamera.orthographicSize;
		//ensure that both orthographic cameras are of the same size
		backgroundOrtho.orthographicSize = orthoSize;

		//the cameras are updated assuming that the 'effective depth' of the orthographic plane is at z = 0
		//this method fixes it so that things rendered in a parallel perspective at z=0 will appear identical to the orthographic view

		//do the near camera first
		float distanceFromOrigin = nearParallax.transform.position.z;
		float fieldOfView = GetFieldOfView (orthoSize, distanceFromOrigin);

		nearParallax.fieldOfView = fieldOfView;

		distanceFromOrigin = farParallax.transform.position.z;
		fieldOfView = GetFieldOfView (orthoSize, distanceFromOrigin);

		farParallax.fieldOfView = fieldOfView;
	}

	//updates the perspective field of view after moving cameras in the z-axis

	//moving the camera further away from the origin can increase the range of near parralax objects and give the appearence of zooming out from the parralax enabled objects
	//to suport this feeling further the orthographic camera should be zoomed out
	public void UpdatePerspectiveViewsMoveZ() {
		//distanceFromOrigin
		float b = Mathf.Abs(orthoCamera.transform.position.z);

		//change clipping planes based on main camera z-position
		farParallax.nearClipPlane = b;
		farParallax.farClipPlane = orthoCamera.farClipPlane;

		nearParallax.farClipPlane = b;
		nearParallax.nearClipPlane = orthoCamera.nearClipPlane;
	}

	//get the field of view for a perspective camera
	public static float GetFieldOfView(float orthoSize, float distanceFromOrigin)
	{
		// orthoSize
		float a = orthoSize;
		// distanceFromOrigin
		float b = Mathf.Abs(distanceFromOrigin);

		float fieldOfView = Mathf.Atan(a / b)  * Mathf.Rad2Deg * 2f;
		return fieldOfView;
	}
}
