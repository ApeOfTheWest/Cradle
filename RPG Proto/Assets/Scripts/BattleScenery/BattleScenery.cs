using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class representing a set of battle scenery, battle scenery is held in memory and then passed to the battle controller at the start of a battle
//scenery should be held in a scriptableObject and instantiated via a prefab so that many objects can hold a reference to it

//scenery is instantiated when the battlescenery is switched to rather than when the battle starts, as such changes in scenery from one battle can carry over
//[CreateAssetMenu (menuName = "BattleScenery/Base")]
public abstract class BattleScenery : MonoBehaviour {
	//use this to set up the scenery using the required prefab and then control it
	//certain sceneries may have different things which happen based on the units in them or the events that ocurr

	//each set of battle scenery has a rectangle area in the scenery that left and right side units can stand in
	//each unit describes their position in terms of the normalized co-ords relative to these rectangles
	[SerializeField]
	private Rect leftField;
	public Rect LeftField {
		//return unmodifiable copy
		get {
			return new Rect (leftField);
		}
	}
	[SerializeField]
	private Rect rightField;
	public Rect RightField {
		//return unmodifiable copy
		get {
			return new Rect (rightField);
		}
	}

	//default z position of the camera, affects parralax effects but not orthographic rendering
	[SerializeField]
	private float cameraZposition = -10f;
	//the default half width of the camera, to account for different aspect ratios this must be converted in height based on ratio
	//this should be set so units are the same size as in the overworld but appear larger
	//set to 0 if only the right and left fields should be displayed with no excess
	[SerializeField]
	private float defaultCameraWidth = 0f;
	//the focal point of the scenery, the point that the camera will try to centre around if possible
	//will be given to cam system as a point of interest
	//0, 0 by default
	[SerializeField]
	private Vector2 sceneFocalPoint = new Vector2();
	//additional rectangular areas to keep in the camera's view
	//often used to ensure a cetrain piece of the horizon is viewable, for composition purposes
	[SerializeField]
	private List<Rect> viewRects = new List<Rect> ();

	//the storage object of the battle scenery, if this is non null then the scenery will return itself to this once it is removed from the battle controller
	//if it is null then the scenery object will delete itself
	public GameObject sceneryStorage = null;

	//the music to play during the battle
	public AudioClip battleMusic;

	//called when the scenery is first set up for use in a battleController, may only be called once during the entire lifetime of the object
	//takes the state of the levelcontroller, as relevant info may be held there for how to set up the scenery
	//also gives the scenery a chance to set up event listeners
	public abstract void InitialSetup(LevelController levelState);

	//called before each battle starts, gives the scenery a look at the battle controller which allows changes to be made based on the units in the battle
	//the position of the camera should be set here if needed
	public virtual void PreBattleSetup(BattleController battleState) {
		//get the camera control and set the scene
		BattleCameraControl camControl = battleState.CameraControl;

		//by default set the camera to display the left field and right field
		camControl.hints.keepInFrame.Add(leftField);
		camControl.hints.keepInFrame.Add(rightField);
		for (int i = 0; i < viewRects.Count; i++) {
			camControl.hints.keepInFrame.Add(viewRects[i]);
		}

		camControl.hints.interestPoints.Add (new CameraFramingHints.CameraInterestPoint (sceneFocalPoint, 1f));

		//also set the intial z and default camera size from the variables
		//convert the half width into half height using aspect ratio
		float halfHeightCam = defaultCameraWidth / 2f / camControl.AttachedCamera.aspect;
		camControl.hints.defaultCameraSize = halfHeightCam;
		camControl.hints.defaultZ = cameraZposition;
	}

	//get the camera size

	//called when the scenery is removed from the battle controller and replaced with a new set of scenery, unless otherwise required to be stored and re-used the scenery should destroy itself
	public virtual void SceneryRemoved() {
		//check the status of the storage object
		if (sceneryStorage != null) {
			//deactivate this object
			gameObject.SetActive(false);

			//add this object back to the storage as a child
			transform.parent = sceneryStorage.transform;
		} else {
			//destroy this object
			Destroy(gameObject);
		}
	}

	//converts the home position of a left field unit for normalised co-ordinates to absolute co-ords
	public Vector2 LeftUnitNormToHome(Vector2 normalizedPosition) {
		Vector2 leftHomePosition = normalizedPosition;

		leftHomePosition.x = leftField.position.x + (leftField.width * leftHomePosition.x);
		leftHomePosition.y = leftField.position.y + (leftField.height * leftHomePosition.y);

		return leftHomePosition;
	}

	public Vector2 RightUnitNormToHome(Vector2 normalizedPosition) {
		Vector2 rightHomePosition = normalizedPosition;

		//normalized positions are from the edge of the screen so flip the x position for right hand positions
		rightHomePosition.x = 1f - rightHomePosition.x;

		rightHomePosition.x = rightField.position.x + (rightField.width * rightHomePosition.x);
		rightHomePosition.y = rightField.position.y + (rightField.height * rightHomePosition.y);

		return rightHomePosition;
	}

	//editor methods
	void OnDrawGizmosSelected() {
		//draw rectangles representing the scenery's left and right field areas for easy editing
		Gizmos.color = Color.blue;
		//get the z position of the scenery
		float zPos = transform.position.z;

		//set the rectangles centre and size based on the left and right fields
		Vector3 centreFrame = new Vector3();
		centreFrame.z = zPos;
		Vector3 size = new Vector3 ();
		size.z = 0f;

		centreFrame = leftField.center;
		size = leftField.size;
		Gizmos.DrawWireCube (centreFrame, size);
		centreFrame = rightField.center;
		size = rightField.size;
		Gizmos.DrawWireCube (centreFrame, size);

		//draw each of the viewing rects a different colour
		Gizmos.color = Color.magenta;

		for (int i = 0; i < viewRects.Count; i++) {
			centreFrame = viewRects[i].center;
			size = viewRects[i].size;

			Gizmos.DrawWireCube (centreFrame, size);
		}

		centreFrame = sceneFocalPoint;
		//draw the focal point as a small circle
		Gizmos.DrawIcon(centreFrame, "targetIcon");

	}
}
