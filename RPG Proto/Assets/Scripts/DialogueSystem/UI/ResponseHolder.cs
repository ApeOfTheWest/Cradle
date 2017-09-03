using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseHolder : MonoBehaviour {
	//prefab to make the buttons from
	[SerializeField]
	private ResponseButton template;
	//pool to store unussed buttons in
	[SerializeField]
	private GameObject pool;

	//list of unused and diabled buttons
	private List<ResponseButton> buttonPool = new List<ResponseButton>();
	//list of currently in use buttons in order, may be disabled until text finishes
	private List<ResponseButton> activeButtons = new List<ResponseButton>();

	//instantiate a response on awake to load the asset into memory and prevent stalling later

	//whether the buttons are active or not
	private bool buttonsActive = false;
	public bool ButtonsActive {
		get {
			return buttonsActive;
		}
		set {
			buttonsActive = value;

			for (int i = 0; i < activeButtons.Count; i++) {
				activeButtons [i].gameObject.SetActive (buttonsActive);
			}
		}
	}

	//clear the current buttons in use, and return them to the pool
	public void ClearButtons() {
		for (int i = activeButtons.Count - 1; i >= 0; i--) {
			ResponseButton button = activeButtons [i];
			activeButtons.RemoveAt (i);

			button.gameObject.SetActive (false);

			button.transform.SetParent(pool.transform);
			buttonPool.Add (button);
		}
	}

	//make a new response button under the last, take the player of the dialog so events can be sent
	public void AddResponse(DialogResponse response, int responseIndex, DialogPlayer player) {
		//get a new button
		ResponseButton button = GetButton();
		button.gameObject.SetActive (ButtonsActive);
		button.transform.SetParent(transform);
		activeButtons.Add (button);

		//set up the button with references to the player, store the index of the response on each button along with the text
		button.Init(response, responseIndex, player);
	}

	//get a response button from the pool, or make one if not
	private ResponseButton GetButton() {
		ResponseButton butt;

		if (buttonPool.Count > 0) {
			butt = buttonPool [buttonPool.Count - 1];
			buttonPool.RemoveAt (buttonPool.Count - 1);
		} else {
			butt = Instantiate (template, pool.transform);
		}

		return butt;
	}
}
