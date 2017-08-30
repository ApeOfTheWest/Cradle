using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderPortrait : MonoBehaviour {
	//get the image component from the child
	private Image portraitImage;
	public Image PortraitImage {
		get {
			return portraitImage;
		}
	}

	//portrait width in pixels
	private const int PORTRAIT_WIDTH = 95;

	//get the width of the portrait image in pixels, used for alignment
	public int GetPortraitWidth() {
		return PORTRAIT_WIDTH;
	}

	//the unit being represented by this portrait
	private UnitController unit;

	//whether the portrait is highlighted or not
	private bool highlighted = false;
	public bool Highlighted {
		get {
			return highlighted;
		}
		set {
			//if disabling highlight then reset the colour of the image to white
			highlighted = value;

			if (highlighted == false) {
				portraitImage.color = Color.white;
			}
		}
	}

	//method to set the unit that the portrait refers to, set the portrait sprite using the unit
	public UnitController Unit {
		get {
			return unit;
		}
		set {
			unit = value;

			//set the portrait image
			if (unit != null) {
				portraitImage.sprite = unit.turnOrderPortrait;
			} else {
				portraitImage.sprite = null;
			}

		}
	}

	// Use this for initialization
	void Awake () {
		//use get components in children to skip the parent component
		portraitImage = GetComponentsInChildren<Image>()[1];

	}

	//make the portrait picture flash if the subject is highlighted 
	//will be called from the holder of this portrait
	public void UpdateHighlightColour(Color highlightColour) {
		if (highlighted == true) {
			portraitImage.color = highlightColour;
		}
	}
}
