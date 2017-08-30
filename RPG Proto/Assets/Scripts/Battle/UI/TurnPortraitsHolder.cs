using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPortraitsHolder : MonoBehaviour {

	//list of all of the portraits in the order they appear
	private List<TurnOrderPortrait> turnPortraits = new List<TurnOrderPortrait>();
	public List<TurnOrderPortrait> TurnPortraits {
		get {
			return turnPortraits;
		}
	}

	//the portrait prefab to instantiate
	public TurnOrderPortrait refPortrait;

	//the colour to apply to the highlighted protraits, must have a public copy held here so the animator can access it
	public Color highlight = Color.white;

	//on late update apply the highlight colour to the portraits (must be late so that animator has time to update colour)
	void LateUpdate() {
		for (int i = 0; i < turnPortraits.Count; i++) {
			turnPortraits [i].UpdateHighlightColour (highlight);
		}
	}

	//use this to set the number of portraits based on the availiable space in pixels that need to be filled
	//wont remake portraits unless needed
	//take into account any scaling factor when supplying the pixel units
	public void FillPixelsWithPortraits(int pixels, float canvasScaling) {
		//the number of pixels wide a portrait is depends on the canvas scale and the pixels in the turnportrait image component

		//get the size of the portraits (unscaled) and multiply by canvas scaling
		float portraitSize = (float)refPortrait.GetPortraitWidth() * canvasScaling;

		//divide the pixels by the portrait size
		float portraitsNeeded = ((float) pixels) / portraitSize;

		//and round up to get the number of portraits needed
		int portraitsNeededInt = Mathf.CeilToInt(portraitsNeeded);

		//now make a number of portraits equal to the number needed, delete uneeded portraits, make more if needed
		if (turnPortraits.Count > portraitsNeededInt) {
			//delete excess
			int numberToDelete = turnPortraits.Count - portraitsNeededInt;

			//make sure to delete from the end first
			for (int i = 0; i < numberToDelete; i++) {
				TurnOrderPortrait toDelete = turnPortraits [turnPortraits.Count - 1];
				turnPortraits.RemoveAt (turnPortraits.Count - 1);
				Destroy (toDelete.gameObject);
			}
		} else if (turnPortraits.Count < portraitsNeededInt) {
			//make extra

			while (turnPortraits.Count < portraitsNeededInt) {
				//instantiate new portraits to the holder object
				turnPortraits.Add(Instantiate(refPortrait, gameObject.transform));
			}
		}
	}
}
