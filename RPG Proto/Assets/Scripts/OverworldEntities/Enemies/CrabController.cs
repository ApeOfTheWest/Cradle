using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabController : Creature {
	private GameObject target = null;

	//public Rigidbody2D creatureBody;
	//private Animator creatureAnimator;
	//the fight hitbox of the crab, is kept here so it can be toggled off and on
	public GameObject fightHitbox;

	//the audio source to play all sounds of the player from
	//private AudioSource audioSource;
	//the sound clips to play a footstep sound from
	//public AudioClip[] footstepSounds = null;

	//the vision cones of the crab, contains both buried and unburied cones
	public VisionRadius buriedVision;
	public VisionCone unburiedVision;

	//the pursuer attached to the crab, used to let the crab chase the player
	private Pursuer pursuer;

	//the distance at which the crab will stop following a target and return to its burrow
	//public float pursuitMaxDistance = 4f;

	//the current state of the crab's AI
	private enum CrabState {
		Buried, //while buried in ground, changed to digging upon the crab digging itself in or out of ground
		Digging, //digging state acuurs while crab is burying or unburing itself
		Idle, //state the crab enters upon coming out of ground, while walk around looking for a target and return home to bury if none is found
		Chasing, //state while chasing a target, will leave this state if target is lost and then return home to bury itself
		Returning //crab will return home and bury itself after walking left and right for a moment
	}

	private CrabState aiState = CrabState.Buried;

	//used to determine whether the target has been lost or not when chasing something
	//private TargetLostDistance targetLossCheck = new TargetLostDistance();

	//the time taken to finish the burying / unburying animations
	//used to show when the crab can move / check for unburying conditions again
	const float BURY_TIME = 1.02f;

	//public float speed = 5f;

	//crab starts buried
	//private bool buried = true;
	//if the crab is playing the burying / unburying animation set to true
	//prevent any bury / unbury command until complete
	//private bool playingAnim = false;

	//direction the creature is moving in, starts looking towards camera (for crab means moving to the right)
	//private Vector2 moveDirection = new Vector2(1, 0);

	//the home location of the creature, will be initialised to wherever the creature is when start is called
	private Vector2 home;

	//whether or not the creature is moving, starts off not moving
	//private bool moving = false;

	//animator control string hashes
	//static int MOVING_HASH = Animator.StringToHash("Moving");
	//static int X_DIR_HASH = Animator.StringToHash("xDirection");
	//static int Y_DIR_HASH = Animator.StringToHash("yDirection");
	static int BURY_HASH = Animator.StringToHash("Bury");
	static int UNBURY_HASH = Animator.StringToHash("Unbury");

	//the seeker component for the crab, used while in pursuit

	//called initially
	void Awake() {
		//creatureAnimator = GetComponent<Animator> ();
		//audioSource = GetComponent<AudioSource> ();
		base.Awake();

		//disable the fight hitbox while the crab is buried
		fightHitbox.SetActive(false);
		//set the direction, will automatically update vision cones and the like
		//SetMoveDirection(moveDirection);
		//tell the target loss checker to measure all targets against this
		//targetLossCheck.targetMeasure = gameObject;
		//and also tell it the distance to lose sight of the target at
		//targetLossCheck.lossDistance = pursuitMaxDistance;
		//set home to be the current position
		home = transform.position;

		pursuer = GetComponent<Pursuer> ();
		pursuer.TargetDetection = unburiedVision;
	}

	void Start() {
		//set the pursuer to use the unburied vision, and disable it while the crab is buried
		unburiedVision.Enabled = false;
	}

	//enable listeners in onEnable
	void OnEnable() {
		buriedVision.targetDetected += TargetDetectedBuried;
		//unburiedVision.targetDetected += TargetDetectedUnburied;
		pursuer.homeReached += HomeReached;
	}

	//disable listeners in onDisable (or else face memory leaks from held references)
	void OnDisable() {
		buriedVision.targetDetected -= TargetDetectedBuried;
		//unburiedVision.targetDetected -= TargetDetectedUnburied;
		pursuer.homeReached -= HomeReached;
	}

	//scripts called from the pursuer
	public void HomeReached() {
		//tell the crab to bury itself
		Bury();
	}

	//command to bury the crab
	private void Bury() {
		if (aiState != CrabState.Buried && aiState != CrabState.Digging) {
			animator.SetTrigger (BURY_HASH);
			aiState = CrabState.Digging;

			//stop the crab from moving and update the animator
			//Moving = false;
			//also tell the crab to be moving towards the right, as thats where the sprite for burying is positioned towards
			MoveDirection = new Vector2(1, 0);

			//disable the fight hitbox
			fightHitbox.SetActive(false);
			//tell the crab's rigid body to become kinematic (fixing it in place)
			RigidBody.isKinematic = true;

			//disable the uburied vision
			unburiedVision.Enabled = false;
			//unburiedVision.gameObject.SetActive(false);

			StartCoroutine (BuryAnimationWait());
		}
	}

	//wait for burying animation to finish
	private IEnumerator BuryAnimationWait() {
		yield return new WaitForSeconds (BURY_TIME);

		aiState = CrabState.Buried;
		//enable the buried vision
		buriedVision.Enabled = true;
	}

	//command to unbury the crab
	private void Unbury() {
		if (aiState == CrabState.Buried) {
			animator.SetTrigger (UNBURY_HASH);
			aiState = CrabState.Digging;

			//disable the buried vision cone
			buriedVision.Enabled = false;

			//enable the fight hitbox after the animation is done playing using coroutine
			StartCoroutine (UnburyAnimationWait());
		}
	}

	//coroutine for waiting for unburying animation to finish
	private IEnumerator UnburyAnimationWait() {
		yield return new WaitForSeconds (BURY_TIME);

		aiState = CrabState.Idle;
		//enable fight hitbox after animation is done playing
		fightHitbox.SetActive(true);
		//tell the creatures rigid body to become dynamic on unburying
		RigidBody.isKinematic = false;
		//also enable unburied vision cone
		unburiedVision.Enabled = true;
	}	

	//listener method for the target detector while buried
	public void TargetDetectedBuried (TargetDetection detector, GameObject target) {
		//dont do anything if currently burying or unburying
		if (aiState != CrabState.Digging) {
			//raise from the ground
			Unbury();
		}
	}

	//same for unburied
	/*public void TargetDetectedUnburied (TargetDetection detector, GameObject target) {
		//dont do anything if currently burying or unburying
		if (aiState != CrabState.Digging) {
				//if not buried then chase the target and disable the vision cone until the target is lost
				//don't do this multiple times in the case multiple targets are detected in the same frame
				//to avoid this ensure the current target is null before setting a new one
				if (this.target == null) {
					this.target = target;
				//also set target in the target loss detector
				targetLossCheck.TargetFound(this.target);
				//and tell the crab controller that the crab is chasing something
				aiState = CrabState.Chasing;
					unburiedVision.gameObject.SetActive (false);
				}
		}
	}*/

	// Update is called once per frame
	/*void Update () {

		//only update things if crab is not in the middle of playing an animation, and only perform actions if crab is not buried
		if (aiState != CrabState.Digging && aiState != CrabState.Buried) {

			//check the state of the crab
			if (aiState == CrabState.Chasing) {
				//check if the crab should lose sight of its target
				if (targetLossCheck.CheckTargetLost () == true) {
					target = null;
				}

				//check if crab still has a target
				if (target != null) {
					//if so then call the point move method with the transform
					MoveTowardsPoint(target.transform.position);

					//if so then set moving to true and follow the target
					//Moving = true;

					//Vector2 vectorToTarget = target.transform.position - gameObject.transform.position;
					//MoveDirection = vectorToTarget;
				} else {
					//if not then set the ai state to returning
					aiState = CrabState.Returning;
					//and when returning reEnable the vision cone
					unburiedVision.gameObject.SetActive (true);
				}
			} else if (aiState == CrabState.Returning) {
				
				//first check distance to home against arbitrary close enough distance
				Vector2 vectorToHome = home - (Vector2)transform.position;

				if (vectorToHome.sqrMagnitude <= 0.001f) {
					//if back home then bury again
					//stop moving first
					StopPointMoving();
					Bury();
				} else {
					//make the crab go back home
					//Moving = true;
					//MoveDirection = vectorToHome;
					MoveTowardsPoint(home);
				}
					
			} else if (aiState == CrabState.Idle) {

			}

		}

	}*/

	/*private void SetMoving(bool move) {
		moving = move;
		creatureAnimator.SetBool (MOVING_HASH, moving);

		if (moving == false) { 
			creatureBody.velocity = Vector2.zero;
		} else {
			creatureBody.velocity = moveDirection * speed;
		}

	}*/

	public override Vector2 LookDirection {
		get {
			return base.LookDirection;
		}
		set {
			base.LookDirection = value;

			//when setting the look direction make sure to set the unburied vision cone
			//crabs have a quirk in that their vision cone is 90 degrees clockwise from the direction they're moving in
			//unburiedVision.coneDirection = moveDirection;
			unburiedVision.coneDirection = Quaternion.Euler(0, 0, -90) * moveDirection;
		}
	}

	/*private void SetMoveDirection(Vector2 direction) {
		//normalise the direction when storing it
		moveDirection = direction;
		moveDirection.Normalize ();

		creatureAnimator.SetFloat (X_DIR_HASH, moveDirection.x);
		creatureAnimator.SetFloat (Y_DIR_HASH, moveDirection.y);

		//if moving then update the velocity
		if (moving == true) {
			creatureBody.velocity = moveDirection * speed;
		}

		//change the direction of the unburied vision cone
		//crabs have a quirk in that their vision cone is 90 degrees clockwise from the direction they're moving in
		//unburiedVision.coneDirection = moveDirection;
		unburiedVision.coneDirection = Quaternion.Euler(0, 0, -90) * moveDirection;

	}*/

	//called by animator whenever footstep sound effect is to be played
	/*public void FootStep() {
		//make sure the footstep clip array is not null or 0
		if (footstepSounds != null && footstepSounds.Length > 0) {
			//choose a random sound from the array
			int clip = Random.Range(0, footstepSounds.Length);
			//use play oneshot
			//audioSource.clip = footstepSounds[clip];
			audioSource.PlayOneShot (footstepSounds[clip]);
		}
	}*/
}
