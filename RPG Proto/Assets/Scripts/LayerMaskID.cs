using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMaskID {
	//holds a list of all the notable layer ids in use by the game system

	//the layer that represents obscuring objects
	//enemies wont be able to see through these objects when trying to detect a player
	public const int OBSCURE_LAYER = 13;

	//holds a list of commonly required layer masks for use in the physics engine and other
	private static LayerMask OBSCURE_MASK = (1 << OBSCURE_LAYER);
	public static LayerMask GetObscureMask() {
		return OBSCURE_MASK;
	}
}
