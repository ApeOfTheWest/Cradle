using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//holds a collection of all the player status bars, and provides methods for highlighting one of them
public class PlayerStatusBars : MonoBehaviour, BattleUIComponent {
	//prefabs of the types of status bars to instantiate

	//a status bar with health and magick
	public PlayerHealthManaBar healthMagickBar;
	//status bar with health only
	public PlayerHealthBar healthBar;

	//the status bars being held in this class, paired with their priority and sorted so that new bars can be placed next to appropriate priorities
	//the list must be kept sorted manually
	//the list should be sorted from highest priority to lowest priority
	private List<KeyValuePair<PlayerStatusBar, int>> sortedStatusBars = new List<KeyValuePair<PlayerStatusBar, int>>();

	//the currently highlighted status bar (if any)
	private PlayerStatusBar highlighted = null;

	//the animator to use for the bar display animation
	private Animator animator;

	private static int FADE_IN_HASH = Animator.StringToHash ("FadeIn");

	//the canvas group to use to make the bars invisible
	private CanvasGroup canvasGroup;

	void Awake() {
		canvasGroup = GetComponent<CanvasGroup> ();
		animator = GetComponent<Animator> ();
	}

	//animate the bars in
	public void DisplayBars() {
		canvasGroup.alpha = 1f;

		//start the animator
		animator.Play(FADE_IN_HASH, 0, 0f);
		animator.Update (0f);
	}

	//hide the bars from view
	//do this by setting the alpha on the canvas group to 0
	public void HideBars() {
		canvasGroup.alpha = 0f;
	}

	//add a new status bar with a certain priority
	//add a diiferent status bar depending on the player's resources
	public void AddStatusBar(PartyUnit unitToDisplay, int priority) {
		//first find a position for the new bar based on the bars currently in the list and the priority to add it at
		//add at index 0 by default
		int indexToAddAt = 0;

		//boolean to show if a position was found or not
		bool positionFound = false;

		//iterate through list until a bar with a lower priority is found
		for (int i = 0; i < sortedStatusBars.Count; i++) {
			//check if this status bar has a lower priority
			if (sortedStatusBars [i].Value < priority) {
				//if so then assign the index to add just before this current index (leave index as is)
				indexToAddAt = i;
				//show that a position has been found
				positionFound = true;
				//and break from the loop
				break;
			}
		}

		//var to hold the added bar
		PlayerStatusBar addedBar;
		//instantiate a different bar depending on the resources used by the player
		if (unitToDisplay.Magick.MaxMagick > 0) {
			addedBar = Instantiate(healthMagickBar, transform) as PlayerStatusBar;
		} else {
			addedBar = Instantiate(healthBar, transform) as PlayerStatusBar;
		}

		//assign the status bar to the given player
		addedBar.AssignPlayer(unitToDisplay);

		//check if a position was found for the bar
		if (positionFound == true) {
			//add the new bar at the specified position in the list and the heirchy
			//set its position in the heirchy
			addedBar.transform.SetSiblingIndex(indexToAddAt);
			//add it to the list in the specified position
			sortedStatusBars.Insert(indexToAddAt ,new KeyValuePair<PlayerStatusBar, int>(addedBar, priority));
		} else {
			//if not the list is empty or the new bar has the lowest priority
			//either way add it onto the end
			sortedStatusBars.Add(new KeyValuePair<PlayerStatusBar, int>(addedBar, priority));
		}
	}

	public void SetupFromBattle(BattleController battle) {
		ClearStatusBars ();

		//also hide the bars to start with
		HideBars();
	}

	//clear all existing status bars
	public void ClearStatusBars() {
		ClearHighlights ();

		//delete each status bar from the list and destroy it
		while (sortedStatusBars.Count > 0) {
			PlayerStatusBar barToRemove = sortedStatusBars [0].Key;

			sortedStatusBars.RemoveAt (0);

			Destroy (barToRemove);
		}
	}

	//remove an existing status bar
	public void RemoveStatusBar(PartyUnit unitToRemove) {
		//search the list for a match
		for (int i = 0; i < sortedStatusBars.Count; i++) {
			if (sortedStatusBars [i].Key.PlayerDisplay == unitToRemove) {
				PlayerStatusBar barToRemove = sortedStatusBars [i].Key;

				//if the bar being removed is currently highlighted then clear all the highlights
				if (barToRemove == highlighted) {
					ClearHighlights ();
				}

				//remove the bar from the list before destroying it
				sortedStatusBars.RemoveAt(i);

				Destroy (barToRemove);

				//break out of the loop, the unit is found
				break;
			}
		}
	}

	//highlight a specific party member, will clear any previous highlight
	public void HighlightStatusBar (UnitController toHighlight) {
		//clear any current highlight, even if no new highlight is found
		ClearHighlights();

		//search the list for a match
		for (int i = 0; i < sortedStatusBars.Count; i++) {
			if (sortedStatusBars [i].Key.PlayerDisplay == toHighlight) {
				//cache the highlighted bar for later reference
				highlighted = sortedStatusBars [i].Key;

				sortedStatusBars [i].Key.Highlighted = true;

				//break out of the loop, the unit is found
				break;
			}
		}
	}

	//clear all highlights
	public void ClearHighlights() {
		if (highlighted != null) {
			highlighted.Highlighted = false;

			highlighted = null;
		}
	}
}
