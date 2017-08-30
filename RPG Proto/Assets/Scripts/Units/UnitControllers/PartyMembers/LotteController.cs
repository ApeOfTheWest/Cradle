using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteController : PartyUnit {
	//animator to use for the body of lotte
	private static int MOVING_HASH = Animator.StringToHash("Moving");
	private static int BATTLE_INTRO_HASH = Animator.StringToHash("Intro");
	private static int INTRO_EFFECT_HASH = Animator.StringToHash("IntroEffects");
	private static int IDLE_HASH = Animator.StringToHash("Idle");

	new void Awake() {
		base.Awake ();
		//animator = GetComponent<Animator> ();
	}
		
	public override void PlayUnitIntro() {
		//play the intro animation and wait for it to finish
		//the intro animation will perform a callback on the intro finished event
		animator.Play(BATTLE_INTRO_HASH);
		animator.Play (INTRO_EFFECT_HASH);
	}

	public override void IntroAnimationFinished() {
		//change the animator state to idle
		animator.Play(IDLE_HASH);

		//callback to the controller when finished
		base.IntroAnimationFinished();
	}
}
