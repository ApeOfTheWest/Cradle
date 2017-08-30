using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ElementalReistances {
	//a list of all the elemental resistances, these are listed in a bool, int pair
	//the bool represents whether the element is absorbed, the int represents the percent resistance (or weakness) if no absorbtion takes place
	public ElementResistance fire = new ElementResistance();
	
}
