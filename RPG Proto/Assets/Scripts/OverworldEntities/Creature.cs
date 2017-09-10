using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//a generic class for the controlling of a creature in the overworld
//each creature is just a 'thinking' entity
public class Creature : Entity {
	//variables common to all creatures

	//creatures should be animated, so use an animator
	protected Animator animator;
	public Animator Animator {
		get {
			return animator;
		}
	}
	//each creature has an audiosource to play sounds on locally 
	protected AudioSource audioSource;
	public AudioSource AudioSource {
		get {
			return audioSource;
		}
	}
	//each creature has a selection of footstep sounds to play while walking
	//this can be set by the floor the creature is walking on if desired
	public AudioClip[] footstepSounds = null;
	//speed of the creature when moving, creature moves this far per second when moving is set to true
	public float speed = 1f;

	//the movement direction (used to set the direction the creature moves in), start moving to the right
	protected Vector2 moveDirection = new Vector2(1f, 0);
	public virtual Vector2 MoveDirection {
		get {
			return moveDirection;
		}
		set {
			moveDirection = value;
			//normalize direction when storing it
			moveDirection.Normalize ();
			//if moving then update the velocity
			if (moving == true) {
				RigidBody.velocity = moveDirection * speed;
			}

			//by default set the lookdirection to be the same as the move direction (can be different if this is overidden)
			LookDirection = moveDirection;
		}
	}
	//the looking direction (used to set direction vars in the animator), start looking towards the camera
	protected Vector2 lookDirection = new Vector2(0, -1f);
	//make this virtual as some creatures may have vision cones that depend on this
	public virtual Vector2 LookDirection {
		get {
			return lookDirection;
		}
		set {
			lookDirection = value;

			animator.SetFloat (XDirHash, moveDirection.x);
			animator.SetFloat (YDirHash, moveDirection.y);
		}
	}

	//whether or not the creature is moving, starts off not moving
	protected bool moving = false;
	//make virtual in case creature wants to move using a different method
	//any characteristic moving (leaping, bursts of speed, acceleration) should be put here, 
	//this will get called by the pathfinding system if any point movement is needed
	public virtual bool Moving {
		get {
			return moving;
		}
		set {
			moving = value;
			//tell the animator this is moving or not
			animator.SetBool (MovingHash, moving);

			//set the velocity to zero or the movement speed based on whether creature is moving or not
			if (moving == false) { 
				RigidBody.velocity = Vector2.zero;
			} else {
				RigidBody.velocity = moveDirection * speed;
			}
		}
	}

	//animation hashes common to all creatures
	//must be static rather than const for hashing method, provide as properties
	private static int MOVING_HASH = Animator.StringToHash("Moving");
	public int MovingHash {
		get {
			return MOVING_HASH;
		}
	}
	private static int X_DIR_HASH = Animator.StringToHash("xDirection");
	public int XDirHash {
		get {
			return X_DIR_HASH;
		}
	}
	private static int Y_DIR_HASH = Animator.StringToHash("yDirection");
	public int YDirHash {
		get {
			return Y_DIR_HASH;
		}
	}

	//pathfinding variables
	//the seeker component attached
	private Seeker seeker;
	//the ienumerator of the current movetowards ability, can use to cancel a previous pathfinding attempt
	private Coroutine pointMoveCoroutine = null;
	//the current target position that the creature is trying to move to
	private Vector2 targetPosition;
	//the currently generated path that the creature is using
	private Path movementPath = null;
	//the waypoint currently being moved towards
	private int currentWaypoint = 0;
	//the distance from a waypoint a creature must reach before moving onto the next one
	public float nextWaypointDistance = 0.5f;
	//the time to wait between path refreshes
	//may need to be lower if in a fast changing environment
	//this time will be waited on even if the target position changes
	public float pointMoveRepathRate = 0.5f;
	//the cooldown until the next repath is needed
	private float timeUntilNextRepath = 0f;

	//pathfinding events and delegates
	public delegate void MovementComplete(bool movementStopped, Vector2 targetPositionReached);
	//event fired when a movement command is completed
	//a bool shows if the destination was reached or if the movement was cancelled
	//the targetposition shows the position that was aimed for (useful to check that the unit has reached the right destination instead of being redirected)
	public event MovementComplete movementComplete;

	// Use this for initialization
	new protected void Awake () {
		//first initialise the entity part
		base.Awake();

		//next get creature components
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource> ();
		seeker = GetComponent<Seeker> ();
		#if UNITY_EDITOR
		if (animator == null) {
			Debug.Log("Warning: No animator found on creature object");
		}
		if (audioSource == null) {
			Debug.Log("Warning: No audio source found on creature object");
		}
		if (seeker == null) {
			Debug.Log("Warning: No seeker found on creature object");
		}
		#endif

		//by default set move direction and vision direction so that additional setup is called through property methods
		MoveDirection = moveDirection;
		LookDirection = lookDirection;
	}

	//set the physics label to creature
	protected override void SetLayer() {
		//set the layer of this to the entity layer
		gameObject.layer = GlobalTweakedConstants.CREATURE_LAYER;
	}
		
	//used to tell the creature to a location, this is virtual to account for alternate methods of locomotion and pathfinding
	//pathfinding must occurr here to prevent the creature from getting stuck
	//useful for moving creatures in cutscenes while maintaining any movement quirks and pathfinding required
	//return true if the creature is considered to be at the point
	/*public virtual bool MoveTowardsPoint2(Vector2 target) {
		//if this is less than one physics frame from the target position then use move position and set moving to false
		Vector2 vectorToTarget = target;
		vectorToTarget.x -= gameObject.transform.position.x;
		vectorToTarget.y -= gameObject.transform.position.y;
		float frameDistance = Time.fixedDeltaTime * speed;

		if (vectorToTarget.sqrMagnitude < (frameDistance * frameDistance)) {
			RigidBody.MovePosition (target);
			Moving = false;
			return true;
		} else {
			MoveDirection = vectorToTarget;
			Moving = true;
			return false;
		}
	}*/

	//called by animator whenever footstep sound effect is to be played
	//virtual in case other things should happen (particle effects etc)
	public virtual void FootStep() {
		//make sure the footstep clip array is not null or 0
		if (footstepSounds != null && footstepSounds.Length > 0) {
			//choose a random sound from the array
			int clip = Random.Range(0, footstepSounds.Length);
			//use play oneshot
			audioSource.PlayOneShot (footstepSounds[clip]);
		}
	}

	//used to tell the creature to move to a point, this will start a coroutine that is dedicated to moving towards the given point
	//if the coroutine is already running then the target position will be changed and on the next repath will be updated
	public void MoveTowardsPoint(Vector2 target) {
		//set the target position
		targetPosition = target;

		//if the coroutine isn't active then start a new one
		if (pointMoveCoroutine == null) {
			pointMoveCoroutine = StartCoroutine(MoveToPoint());
		}
	}
	//the coroutine for moving towards a point
	private IEnumerator MoveToPoint() {
		//request a new path
		seeker.StartPath(transform.position, targetPosition, MovePointPathGenerated);
		timeUntilNextRepath = pointMoveRepathRate;

		//infinite loop until point is reached or found unreachable
		while (true) {
			//only do something if the path is non-null (may take a bit to be returned from the seeker)
			if (movementPath != null) {
				//follow the path waypoint by waypoint

				//get the vector to next waypoint
				Vector2 direction = movementPath.vectorPath[currentWaypoint] - transform.position;
				//if the next waypoint is the last one then check if the creature would reach it in the next fixedupdate
				if (currentWaypoint >= movementPath.vectorPath.Count - 1) {
					float frameDistSqr = speed * Time.fixedDeltaTime;
					frameDistSqr *= frameDistSqr;

					if (frameDistSqr >= direction.sqrMagnitude) {
						//set the creature position to the destination and break from the loop
						RigidBody.MovePosition (movementPath.vectorPath [currentWaypoint]);
						Moving = false;
						break;
					}
				} else if (direction.sqrMagnitude <= nextWaypointDistance) {
					//if not on the last waypoint test whether to move onto the next one
					currentWaypoint++;
					//update direction to match the new waypoint
					direction = movementPath.vectorPath[currentWaypoint] - transform.position;
				}
				//now move toward the target waypoint
				MoveDirection = direction;
				Moving = true;

				//lastly, if the repath time is ready and the seeker is done with the last one fetch a new path
				if (timeUntilNextRepath <= 0f && seeker.IsDone ()) {
					//request a new path
					seeker.StartPath(transform.position, targetPosition, MovePointPathGenerated);
					//set the time to the rate
					timeUntilNextRepath = pointMoveRepathRate;
				}
			}

			//yield until next fixed update frame (movement will usually be calculated on physics step)
			//this also ensures ai movement is fairly frame independent
			yield return new WaitForFixedUpdate();

			//lower the repath time after the frame has passed
			timeUntilNextRepath -= Time.fixedDeltaTime;
		}

		//cleanup after the pathing mechanism
		pointMoveCoroutine = null;
		movementPath = null;

		//and activate point reached events
		if (movementComplete != null) {
			movementComplete (false, targetPosition);
		}
	}

	//method called when a new movement path is generated
	public void MovePointPathGenerated(Path p) {
		//check for an error on the returned path
		if (!p.error) {
			//store the new path
			movementPath = p;
			//reset the waypoint
			currentWaypoint = 0;
		} else {
			//if an error has ocurred then stop the movement code, this may mean the point is unreachable
			StopPointMoving();
		}
	}

	//used to cancel any current move towards a point commands a creature may have
	public virtual void StopPointMoving() {
		//check a movement coroutine is active
		if (pointMoveCoroutine != null) {
			//stop the coroutine
			StopCoroutine (pointMoveCoroutine);
			pointMoveCoroutine = null;
			movementPath = null;
			//call the move cancelled event on any listeners
			if (movementComplete != null) {
				movementComplete (true, targetPosition);
			}
		}
	}
}
