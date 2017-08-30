using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script to be attached to a ui component that displays the current state of a rally resource
public class RallyMeter : MonoBehaviour, BattleUIComponent {
	//the rally meter has multiple different displays, displays for the level of rally
	private RallyLevel[] rallyLevelDisplay;

	//and a display for the percentage towards the next level of rally
	private RallyBar rallyBarDisplay;

	//the canvas group used to control the meter
	private CanvasGroup meterGroup;

	private Animator animator;
	private static int FADE_IN_HASH = Animator.StringToHash ("FadeIn");

	//the rally to be displayed by this meter
	private Rally toDisplay;

	// Use this for initialization
	void Awake () {
		rallyLevelDisplay = GetComponentsInChildren<RallyLevel> ();
		rallyBarDisplay = GetComponentInChildren<RallyBar> ();
		meterGroup = GetComponent<CanvasGroup> ();
		animator = GetComponent<Animator> ();
	}
	
	//use lateupdate to update the rendering of the meter, this is because the state of a rally meter may change as part of the update cycle
	void LateUpdate() {
		//notify the displays of the new rally status in case it has changed
		rallyBarDisplay.UpdateRallyPercent((float)toDisplay.LevelProgress / (float)Rally.MAX_LEVEL_PROGRESS);

		for (int i = 0; i < rallyLevelDisplay.Length; i++) {
			rallyLevelDisplay [i].UpdateLevel (toDisplay.RallyLevel);
		}
	}

	//set the rally display component of the meter
	public void SetDisplayRally(Rally rally) {
		toDisplay = rally;

		//TODO remove after testing
		//rally.LevelProgress = (int)(0.8f * (float) Rally.MAX_LEVEL_PROGRESS);

		//update the meter visually to match the current rally

	}

	//animate the meter in
	public void DisplayMeter() {
		animator.Play (FADE_IN_HASH);
	}

	//animate the meter out
	public void HideMeter() {
		meterGroup.alpha = 0;
	}

	//reset the meter each time a new battle is starting
	//don't set the rally component to measure, this will be set by the player's side if they are in battle
	//the meter will be de-activated until it is assigned a heat meter to measure
	public void SetupFromBattle(BattleController battle) {
		//reset each component
		rallyBarDisplay.ResetDisplay();

		for (int i = 0; i < rallyLevelDisplay.Length; i++) {
			rallyLevelDisplay [i].ResetDisplay ();
		}

		//initialise the group as invisible
		meterGroup.alpha = 0;
	}
}
