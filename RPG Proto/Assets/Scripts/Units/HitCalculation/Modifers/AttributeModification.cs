using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeModifier {
	//the percentage change
	public int percentChange = 0;
	//the raw change
	public int rawChange = 0;

	//resets the modifiers to 0
	public void Reset() {
		percentChange = 0;
		rawChange = 0;
	}
}
