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

	// Use this for initialization
	void Awake () {
		dialog = GetComponentInChildren<DialogUI> (true);
		dialog.gameObject.SetActive (false);
	}

}
