using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatResponse : DialogueResponse {
	//the minimum value (make it 0 if negatives are not allowed)
	public float minValue = 0;
	//whether to use the minimum value or not
	public bool useMinimum = true;
	//boolean for checking whether to request this min value from the interface
	public bool getMinimum = false;
	//the default value (response dialog starts on this assuming its within the min / max boundaries)
	public float defaultValue = 0;
	public bool getDefault = false;
	//the max value
	public float maxValue = 0;
	public bool useMaximum = false;
	public bool getMaximum = false;
}
