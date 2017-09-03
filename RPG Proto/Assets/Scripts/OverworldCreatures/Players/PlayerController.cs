using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Rigidbody2D creatureBody;
	private Animator creatureAnimator;
	public float speed = 2f;

	//the audio source to play all sounds of the player from
	private AudioSource audioSource;

	//the sound clips to play a footstep sound from
	public AudioClip[] footstepSounds = null;

	//controls how battles start if the player's body is hit
	private PlayerContactChallenger contactFightbox;

	//keeps track of the current party members
	private PlayerRoster roster;

	//animator control string hashes
	private static int MOVING_HASH = Animator.StringToHash("Moving");
	private static int X_DIR_HASH = Animator.StringToHash("xDirection");
	private static int Y_DIR_HASH = Animator.StringToHash("yDirection");

	//direction the creature is moving in, starts looking towards camera
	private Vector2 moveDirection = new Vector2(0, -1);
	//whether or not the creature is moving, starts off not moving
	private bool moving = false;

	public Vector2 MoveDirection {
		get {
			return moveDirection;
		}
		set {
			//normalise the direction when storing it
			moveDirection = value;
			moveDirection.Normalize ();

			creatureAnimator.SetFloat (X_DIR_HASH, moveDirection.x);
			creatureAnimator.SetFloat (Y_DIR_HASH, moveDirection.y);

			//if moving then update the velocity
			if (moving == true) {
				creatureBody.velocity = moveDirection * speed;
			}
		}
	}
	public bool Moving {
		get {
			return moving;
		}
	}

	// Use this for initialization
	void Awake () {
		creatureBody = GetComponent<Rigidbody2D> ();
		creatureAnimator = GetComponentInChildren<Animator> ();
		contactFightbox = gameObject.GetComponentInChildren<PlayerContactChallenger> ();
		audioSource = GetComponent<AudioSource> ();

		//set references for the fight challenger
		contactFightbox.SetControl(LevelController.levelControllerInstance);
	}

	//set party in start as it depends on the roster's awake function being called
	void Start() {
		//get the roster from the persistent data store and set it to this player
		roster = LevelController.persistentDataInstance.roster;

		contactFightbox.SetActiveParty (roster.selectedParty);
	}
	
	// Update is called once per frame
	void Update () {
		//check the system isn't paused before getting input
		if (LevelController.levelControllerInstance.WorldPaused == false) {

		Vector2 movementVector = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		if (movementVector != Vector2.zero) {
			creatureAnimator.SetBool (MOVING_HASH, true);
			creatureAnimator.SetFloat (X_DIR_HASH, movementVector.x);
			creatureAnimator.SetFloat (Y_DIR_HASH, movementVector.y);

			//set the velocity of the player based on the normalized movemnt vector times the speed
			movementVector.Normalize();
			movementVector *= speed;
		} else {
			creatureAnimator.SetBool (MOVING_HASH, false);
		}

		creatureBody.velocity = movementVector;
		//creatureBody.MovePosition (creatureBody.position + movementVector * Time.deltaTime * speed);
		}
	}

	//called by animator whenever footstep sound effect is to be played
	public void FootStep() {
		//make sure the footstep clip array is not null or 0
		if (footstepSounds != null && footstepSounds.Length > 0) {
			//choose a random sound from the array
			int clip = Random.Range(0, footstepSounds.Length);
			//use play oneshot
			audioSource.clip = footstepSounds[clip];
			audioSource.Play();
		}
	}
}
