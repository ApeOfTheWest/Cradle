using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Magick {
	//maximum magick can be 0 for units that dont use it
	//serialize the max so it can be modified in inspector (note: modifying it directly may cause the current resouce to exceed the max until updated)
	[SerializeField]
	private int maxMagick = 0;
	public int MaxMagick {
		get {
			return maxMagick;
		}
		set {
			maxMagick = value;

			//if current magick is more than the new max then lower it
			if (currentMagick > maxMagick) {
				currentMagick = maxMagick;
			}
		}
	}

	[SerializeField]
	private int currentMagick = 0;
	public int CurrentMagick {
		get {
			return currentMagick;
		}
		set {
			currentMagick = value;

			//if the value is greater than the max then clamp to max
			if (currentMagick > maxMagick) {
				currentMagick = maxMagick;
			} else if (currentMagick < 0) {
				currentMagick = 0;
			}
		}
	}

	//constructor should take a value for the max magic, the system will then start with a maxed out magick resource
	public Magick() {

	}
	public Magick(int max) {
		MaxMagick = max;

		MaxOut ();
	}

	//max out the magick bar
	public void MaxOut() {
		CurrentMagick = maxMagick;
	}

	//change the health by the given amount
	public void ChangeMagick(int amount) {
		CurrentMagick += amount;
	}
}
