using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabUnitController : UnitController {
	private static int BATTLE_INTRO_HASH = Animator.StringToHash("Intro");

	new void Awake() {
		base.Awake ();
	}

	public override void PlayUnitIntro() {
		//play the intro animation and wait for it to finish
		animator.Play(BATTLE_INTRO_HASH);
	}
}
