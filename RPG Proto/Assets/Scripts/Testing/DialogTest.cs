using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTest : MonoBehaviour {
	public DialogUI ui;
	public DialogTree tree;
	public DialogEventInterface eventInterface;

	// Use this for initialization
	void Start () {
		//if ui and tree are non null then start the tree
		if (ui != null && tree != null) {
			//set false on pause so no levelcontroller is req
			ui.StartConversation (tree, eventInterface, false);
		}
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.P)) {
			//play the dialog from start
			if (ui != null && tree != null) {
				//set false on pause so no levelcontroller is req
				ui.StartConversation (tree, eventInterface, false);
			}
		}
	}
}
