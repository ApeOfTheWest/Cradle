using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a type of ability that runs up to the enemy, and triggers the attack animation
//once it recieves a callback indicating the frame to do damage on it does damage to the enemy and usually produces a hitspark at the impact point
[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/GenericAbilities/Melee", order = 1)]
public class MeleeAttack : SingleAbility {
	//static parameters common to all melee attacks
	public const string defaultMeleeName = "MeleeAttack";

	//the string of the run and melee variables, yet to be hashed
	//this is kept so that a human can edit these variables
	public string meleeName = defaultMeleeName;

	//the offset to make the attack by (this is how far the unit should be from a target on the right, flip it if not the case)
	public Vector2 meleeOffset = new Vector2();

	//the audio clip to play on hit
	public AudioClip hitSound;

	//parameters to be tweaked on a case by case basis
	//the animation to set to while attacking (must include 2 animation hit effects to work)
	private int meleeHash = 0;
	//the damage packet (contains damage, accuracy and all other possible effects)
	public AbilityHitPacket hitPacket = new AbilityHitPacket();
	//the hitspark to create when the attack connects
	public Hitspark hitSpark = null;
	//the rotation of the hitspark (0 is usually facing to the right, the hitspark will be reflected along the horizontal axis if the user is facing to the left)
	//to reflect the rotation just set the euler angle to its negative
	public float hitSparkAngle = 0f;

	//place to cache the user and target between callbacks
	//set to null when finished to prevent leaks
	private UnitController currUser = null;
	private UnitController currTarget = null;

	//whether the attack has been used in this current ability cycle
	private bool attackUsed = false;

	void OnEnable() {
		//hash the animation methods on waking
		meleeHash = Animator.StringToHash (meleeName);
	}

	//the method that controls how the ability is used
	public override void UseAbility(UnitController user, UnitController target) {
		//store the user and target for later reference
		currUser = user;
		currTarget = target;

		//tell the unit controller's main animator to use the runcycle animation
		//call a manual update first to ensure the animation gets qued at the start even if the current state is the running animation
		user.Animator.Update(0f);
		user.SetRunning();
		//show that the attack has yet to be used
		attackUsed = false;

		//now start a coroutine that will move the unit towards its target before attacking
		user.StartCoroutine(MoveAndAttack());
	}

	//coroutine that moves the player and then finally calls the meleeAttack anim
	public IEnumerator MoveAndAttack() {
		//move the user towards its target but offset it by a little bit based on the given melee offset
		//(switch the sides of the melee offset based on the unit's side in the battle)
		Vector2 meleePoint = currTarget.MeleePoint;

		Vector2 meleePosition = meleePoint;
		//offset the point by the offset to get the position, offset horizontally differently depending on which side the target is on
		meleePosition.y += meleeOffset.y;
		if (currTarget.LeftSide == false) {
			meleePosition.x += meleeOffset.x;
		} else {
			meleePosition.x -= meleeOffset.x;
		}

		//move a bit each frame until at the target
		while (currUser.MoveToPosition (meleePosition) == false) {
			//end until the next frame
			yield return null;
		}

		//make sure the user is facing its target
		if (meleePosition.x < meleePoint.x) {
			currUser.RightFacing = true;
		} else {
			currUser.RightFacing = false;
		}

		//after the move is complete listen for the ability hit function
		currUser.abilityHitEvent += AbilityHit;
		//and play the melee animation
		currUser.Animator.Play(meleeHash);
	}

	//function ability called on hit
	//on the first hit damage should be dealt, on the second hit the ability ended
	public void AbilityHit() {
		if (attackUsed == false) {
			//deal the damage packet to the unit, pass in the user as the source of this
			//store the bool saying if the hit connected or not
			bool hit = currTarget.ApplyHit(hitPacket, currUser);

			//if the audio component exists then play a hit sound
			if (hitSound != null) {
				currUser.AudioSource.PlayOneShot (hitSound);
			}

			//if the hit connected then instantiate a hitspark at the melee point (but a bit closer to the camera z wise)
			if (hit == true && hitSpark != null) {
				float adjustedAngle = hitSparkAngle;
				if (currUser.RightFacing == false) {
					adjustedAngle *= -1f;
				}

				//get the effects holder and istantiate a hitspark on it just in front of the melee point
				Vector3 meleePoint = currTarget.MeleePoint;
				meleePoint.z -= GlobalTweakedConstants.VISUAL_Z_OFFSET;

				Instantiate (hitSpark, meleePoint, Quaternion.Euler (new Vector3 (0, 0, adjustedAngle)),
					currUser.Battle.ActiveBattle.EffectsHolder.transform);
			}

			//show that the attack has been used
			attackUsed = true;
		} else {
			//attack has been used, end the ability
			UnitController holdUser = currUser;
			//clearup
			currUser = null;
			currTarget = null;
			holdUser.abilityHitEvent -= AbilityHit;

			holdUser.SetRunning();
			//tell the user to return home and stop its current ability after it returns
			holdUser.ReturnHomeAndEndAbility();
		}
	}

	//remove event callbacks to prevent leaks
	void OnDisable() {
		if (currUser != null) {
			currUser.abilityHitEvent -= AbilityHit;
		}
		currUser = null;
		currTarget = null;
	}
}
