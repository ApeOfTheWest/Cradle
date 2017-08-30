using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderDisplay : MonoBehaviour, BattleUIComponent {

	//image component of the turn order, used to querey canvas size
	private Image turnOrderImage;

	//the turn portraits holder
	private TurnPortraitsHolder portraitHolder;

	//the animator that moves the turn order holder on and off the screen
	private Animator animator;
	public Animator Animator {
		get {
			return animator;
		}
	}

	private static int FADE_IN_HASH = Animator.StringToHash ("FadeIn");
	private static int FADE_OUT_HASH = Animator.StringToHash ("FadeOut");

	//the number of turns this display can handle, used to find how many turns ahead must be calculated
	public uint DisplayNumber {
		get {
			return (uint)portraitHolder.TurnPortraits.Count;
		}
	}

	// Use this for initialization
	void Awake () {
		turnOrderImage = GetComponent<Image> ();
		portraitHolder = GetComponentInChildren<TurnPortraitsHolder> ();
		animator = GetComponent<Animator> ();
	}

	public void SetupFromBattle(BattleController battle) {
		//clear all previously stored portraits (if any exist)
		//and add a number of new ones based on the current screen size, the portraits must take up at least 6/10ths of the screen width
		int pixelsNeeded = (int)((float)turnOrderImage.canvas.pixelRect.width * (6.0f / 10.0f));

		//call the portrait holder method to generate portraits based on resolution
		portraitHolder.FillPixelsWithPortraits(pixelsNeeded, turnOrderImage.canvas.scaleFactor);

		//hide the turn holder by default
		//do this by setting the animation state to the end of the fade out animation
		animator.Play(FADE_OUT_HASH, 0, 1.0f);
	}

	//tell the display to hide, called after a player has picked their turn
	public void DisplayHide() {
		animator.Play (FADE_OUT_HASH);
		animator.Update(0f);
	}

	//tell the display to appear, called at the start of a players turn
	public void DisplayAppear() {
		animator.Play (FADE_IN_HASH);
		//manually update animator to make sure it starts offscreen (or else 1 frame flashes can happen)
		animator.Update(0f);
	}

	//give the display a list indicating the next set of unit controllers that have a turn, in order
	//the portraits to use for the turn order can be gotten from the unit
	//the number of units given should be at least equal to the display number
	public void DisplayTurns(UnitController[] unitTurns) {
		//set as many units to a display as there are displays, if there are less turns given than displays set the remaining displays to blank

		//first clear all highlights
		ClearHighlights ();

		List<TurnOrderPortrait> turnPortraits = portraitHolder.TurnPortraits;

		//then assign as many units as can fit
		int fittingUnits = unitTurns.Length;
		if (fittingUnits > turnPortraits.Count) {
			fittingUnits = turnPortraits.Count;
		}

		for (int i = 0; i < fittingUnits; i++) {
			turnPortraits [i].Unit = unitTurns [i];
		}

		//and set to null any excess units
		for (int i = fittingUnits; i < turnPortraits.Count; i++) {
			turnPortraits [i].Unit = null;
		}
	}

	//highlight a single unit in the turn order display
	//make sure to clear previous highlights
	public void HightlightUnit(UnitController toHighlight) {
		ClearHighlights ();

		AddHighlight (toHighlight);
	}

	//highlight multiple units in the display
	public void HighlightUnit(UnitController[] toHighlight) {
		ClearHighlights ();

		for (int i = 0; i < toHighlight.Length; i++) {
			AddHighlight (toHighlight [i]);
		}
	}

	//method to add a highlight to the order display, doesnt clear previous highlights
	private void AddHighlight(UnitController toHighlight) {
		//iterate through portraits looking for a match, highlight if match is found
		List<TurnOrderPortrait> portraits = portraitHolder.TurnPortraits;

		for (int i = 0; i < portraits.Count; i++) {
			if (portraits [i].Unit == toHighlight) {
				portraits [i].Highlighted = true;
			}
		}
	}

	//clear any previous highlights
	public void ClearHighlights() {
		List<TurnOrderPortrait> portraits = portraitHolder.TurnPortraits;

		for (int i = 0; i < portraits.Count; i++) {
			portraits [i].Highlighted = false;
		}
	}
}
