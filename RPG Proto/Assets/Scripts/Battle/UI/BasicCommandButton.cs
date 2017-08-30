using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicCommandButton : MonoBehaviour {
	//the button ui component
	private Button button;
	public Button Button {
		get {
			return button;
		}
	}

	//the text to display
	private Text text;

	// Use this for initialization
	void Awake () {
		button = GetComponent<Button> ();
		text = GetComponentInChildren<Text> ();
	}

	//set the text of the button
	public void SetText(string setText) {
		text.text = setText;
	}
}
