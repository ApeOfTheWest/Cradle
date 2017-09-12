using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldUI : MonoBehaviour {
	//get some dialog ui from the children
	private DialogUI dialog;
	public DialogUI Dialog {
		get {
			return dialog;
		}
	}

	//get the pause menu from the children
	private PauseMenu pauseMenu;
	public PauseMenu PauseMenu {
		get {
			return pauseMenu;
		}
	}

	// Use this for initialization
	void Awake () {
		dialog = GetComponentInChildren<DialogUI> (true);
		dialog.gameObject.SetActive (false);
		pauseMenu = GetComponentInChildren<PauseMenu> (true);
		pauseMenu.gameObject.SetActive (false);
	}

}
