using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponseButton : MonoBehaviour {
	DialogPlayer player;
	int responseIndex;

	private Text label;

	public void Init(DialogResponse response, int responseIndex, DialogPlayer player) {
		label = GetComponentInChildren<Text> ();
		label.text = response.responseLabel;
		this.responseIndex = responseIndex;
		this.player = player;
	}

	//click the button
	public void Click() {
		player.SelectResponse (responseIndex);
	}
}
