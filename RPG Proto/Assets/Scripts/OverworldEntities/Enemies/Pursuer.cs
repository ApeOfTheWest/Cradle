using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attach a pursuer script to any creature that should chase after something when spotted and then return to a given home position when they move far enough out of range
//the pursuer script is designed to act on a creature and so should be on the same object as a creature script
//the purser will automatically find targets using a given target detection component but can also manually be given a target if desired
//pursuer script doesn't tell the pursuer how to move and so some sort of callback is required
public class Pursuer : MonoBehaviour {
	//the creature to make pursue a target
	//this must not be null, if it is null this component will return errors
	private Creature pursuer;

	//the target detection being used
	private TargetDetection targetDetection = null;
	public TargetDetection TargetDetection {
		get {
			return targetDetection;
		}
		set {


			targetDetection = value;
		}
	}

	//the target currently being chased
	//set to null to manually lose the target
	private GameObject currentTarget = null;
	public GameObject CurrentTarget {
		get {
			return currentTarget;
		}
		set {
			//if the value is null then disable the target detection script
			//and current value is non null enable the detection script
			if (value == null && currentTarget != null) {
				//dispatch target lost event
				if (targetLost != null) {
					targetLost (currentTarget);
				}

				//enable detection at the end
				if (targetDetection != null) {
					targetDetection.Enabled = true;
				}
			} else if (value != null && currentTarget == null) {
				//dispatch target detected event
				if (targetDetected != null) {
					targetDetected (value);
				}

				//enable detection again at the end
				if (targetDetection != null) {
					targetDetection.Enabled = false;
				}

				//tell the creature to chase
				pursuer.MoveTowardsPoint(value.transform.position);
			}

			currentTarget = value;
		}
	}

	//whether to lose targets after a certain distance or not
	public bool loseTargets = true;
	//how far the pursuer must get from its target before losing it
	public float pursuitMaxDistance = 4f;
	//how close the pursuer must get to its target before counting as having reached it
	public float targetReachedDistance = 2f;

	//the home position of the pusuer, to return to after losing the target
	//can be set by the creature to a point along a patrol route
	public Vector2 homePosition;

	//events that the pursuer can dispatch
	public delegate void TargetDelegate(GameObject target);
	public delegate void BasicDelegate();
	//target detected event, called when target is first spotted, not called again until target is lost
	public event TargetDelegate targetDetected;
	//target lost event
	public event TargetDelegate targetLost;
	//target reached event (within certain threshold)
	//called every frame where a target is counted as reached
	public event TargetDelegate targetReached;
	//home reached event
	public event BasicDelegate homeReached;

	//bool showing if the pursuer is going home or not
	private bool homebound;
	private bool Homebound {
		get {
			return homebound;
		}
		set {
			if (homebound == false && value == true) {
				//add a listener to the pursuer
				pursuer.movementComplete += CreatureReachedDestination;
				//tell the pursuer to go home
				pursuer.MoveTowardsPoint (homePosition);
			} else if (homebound == true && value == false) {
				//clear up listener
				pursuer.movementComplete -= CreatureReachedDestination;
			}

			homebound = value;
		}
	}

	//method called by creature when homeposition is reached
	public void CreatureReachedDestination(bool movementStopped, Vector2 targetPositionReached) {
		//if the position given is the home position then count the creature as having reached home, do this even if movement was stopped (can blocked path)
		if (targetPositionReached == homePosition) {
			Homebound = false;

			if (homeReached != null) {
				homeReached ();
			}
		} else {
			//otherwise tell the creature to go home
			pursuer.MoveTowardsPoint(homePosition);
		}
	}

	// Use this for initialization
	void Awake () {
		//get the creature component from this gameobject
		pursuer = GetComponent<Creature> ();

		//by default the home position is the starting position of this
		homePosition = transform.position;
	}

	//enable listeners in onEnable
	void OnEnable() {
		if (targetDetection != null) {
			targetDetection.targetDetected += TargetDetected;
		}

		//if creature is homebound then listen out for a movement complete event
		if (Homebound) {
			pursuer.movementComplete += CreatureReachedDestination;
		}

		//buriedVision.targetDetected += TargetDetectedBuried;
		//unburiedVision.targetDetected += TargetDetectedUnburied;
	}
	//disable listeners in onDisable (or else face memory leaks from held references)
	void OnDisable() {
		if (targetDetection != null) {
			targetDetection.targetDetected -= TargetDetected;
		}

		pursuer.movementComplete -= CreatureReachedDestination;
		//buriedVision.targetDetected -= TargetDetectedBuried;
		//unburiedVision.targetDetected -= TargetDetectedUnburied;
	}

	//called when a target is detected
	public void TargetDetected (TargetDetection detector, GameObject target) {
		//set the target
		CurrentTarget = target;
	}

	//check if the target should be lost or if it is reached
	//check this every frame
	void LateUpdate() {
		//update the position of the current target
		//if current target is not null then update the move position to follow it
		if (CurrentTarget != null) {
			pursuer.MoveTowardsPoint (CurrentTarget.transform.position);
		} else if (Homebound == true) {
			//if homebound update the position
			pursuer.MoveTowardsPoint(homePosition);
		}

		//only do something here if a target is being pursued, and then only do it if a pursuit can be lost or theres lsiteners for a target reached event
		if (CurrentTarget != null && (loseTargets || targetReached != null)) {
			//first calculate the squared distance from this to the target
			Vector2 distanceVector = CurrentTarget.transform.position - transform.position;
			float distSqr = distanceVector.sqrMagnitude;

			//if lose targets is enabled check to lose the target
			if (loseTargets && distSqr >= pursuitMaxDistance * pursuitMaxDistance) {
				CurrentTarget = null;
				Homebound = true;
				//break out now, don't check for target reached if target is being lost
				return;
			}
			if (targetReached != null && distSqr <= targetReachedDistance * targetReachedDistance) {
				targetReached (CurrentTarget);
			}
		}
	}
}
