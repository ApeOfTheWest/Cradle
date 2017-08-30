using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManaBar : PlayerStatusBar {
	private Image portrait;

	private Image healthBar;
	private Image magickBar;

	//the dropshadow to use when the user is highlighted
	private Outline highlightOutline;

	//set the outline on or off based on whether this is highlighted or not
	public override bool Highlighted {
		set {
			highlighted = value;
			highlightOutline.enabled = highlighted;
		}
	}

	//grab references
	void Awake() {
		portrait = transform.Find ("PortraitHolder/PortraitMask/Portrait").GetComponent<Image>();

		highlightOutline = transform.Find ("Bars").GetComponent<Outline>();
		//turn the shadow off by default
		highlightOutline.enabled = false;

		healthBar = transform.Find ("Bars/HealthBar").GetComponent<Image>();
		magickBar = transform.Find ("Bars/MagickBar").GetComponent<Image>();
	}

	//use late update so that changes that happened in the last update can be applied
	void LateUpdate() {
		UpdateDisplay ();
	}

	//called every update and also when the player is first assigned, update the portrait as well as the bar values 
	//(in case the player portrait changes at health thresholds etc)
	private void UpdateDisplay() {
		portrait.sprite = playerDisplay.StatusBarPortrait;

		healthBar.fillAmount = (float)playerDisplay.Health.CurrentHealth / (float)playerDisplay.Health.MaxHealth;

		//check to make sure the player has a mana resouce
		if (playerDisplay.Magick.MaxMagick > 0) {
			magickBar.fillAmount = (float)playerDisplay.Magick.CurrentMagick / (float)playerDisplay.Magick.MaxMagick;
		} else {
			//if not then empty the magickBar
			magickBar.fillAmount = 0;
		}
	}

	public override void AssignPlayer(PartyUnit player) {
		//call base method
		base.AssignPlayer(player);

		//also set the portrait and all the bars initial state from the status of the player
		UpdateDisplay();
	}
}
