using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityRequirement : MonoBehaviour {
	private Text requirementText;

	//grab the customisable text on awake
	void Awake () {
		requirementText = GetComponentInChildren<Text> ();
	}

	//set the requirement text to display the given string
	public void SetRequirementText(string text) {
		requirementText.text = text;
	}
}
