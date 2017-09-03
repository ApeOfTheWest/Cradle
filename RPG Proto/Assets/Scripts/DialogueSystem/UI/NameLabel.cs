using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameLabel : MonoBehaviour {
	private Text label;

	void Awake() {
		label = GetComponentInChildren<Text> (true);
	}

	public void SetName (string name) {
		label.text = name;
	}
}
