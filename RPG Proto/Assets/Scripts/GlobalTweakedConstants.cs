using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class holds a variety of values used throughout the program that may need to be tweaked a little
public class GlobalTweakedConstants {

	//the value to offset something by to place it just above (or below) another object on the z axis
	//must be large enough to prevent z fighting
	//often used in battle abilities to place an object above or below another object
	public const float VISUAL_Z_OFFSET = 0.01f;

	//physics layers identifiers
	public const int ENTITY_LAYER = 16;
	public const int CREATURE_LAYER = 15;
	public const int PLAYER_LAYER = 14;
}
