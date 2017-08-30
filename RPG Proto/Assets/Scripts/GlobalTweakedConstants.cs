using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class holds a variety of values used throughout the program that may need to be tweaked a little
public class GlobalTweakedConstants {

	//the value to offset something by to place it just above (or below) another object on the z axis
	//must be large enough to prevent z fighting
	public const float VISUAL_Z_OFFSET = 0.01f;
}
