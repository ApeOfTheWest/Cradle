using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberedRallyLevel : MonoBehaviour, RallyLevel {
	//the image component to hold the numbered sprite
	private Image numberImage;

	//the number sprites, should be added in numerical order starting at 0
	public Sprite[] numberSprites;

	// Use this for initialization
	void Awake () {
		numberImage = GetComponent<Image>();
	}
	
	//called every update, value may not have changed so check against last cached value before doing a transition
	public void UpdateLevel(int newLevel) {
		//for the numerical display updates should be handled the same way as setting the level
		SetLevel(newLevel);
	}

	//called to set the display rather than update it, should avoid transitioning, usually called at start of batlle
	public void SetLevel(int newLevel) {
		//change the sprite based on the number, clamp if the number is passed the availiable sprites
		if (newLevel > numberSprites.Length - 1) {
			newLevel = numberSprites.Length - 1;
		}

		numberImage.sprite = numberSprites [newLevel];
	}

	//call when resetting the level display
	public void ResetDisplay() {
		//set the last level back to 0
		SetLevel(0);
	}
}
