using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//an element is a representation for the type of element an ability is
//it includes methods for finding the relevant attributes in the hit calculation step, this way users of an element don't need to know where to look for relevant stats
public abstract class Element : ScriptableObject {
	//when given a unit controller find the relevant elemental resistance information
	//both the numberical resistance and whether the unit absorbs that element, for this reson out must be used
	public abstract void GetUnitResistanceToElement(UnitController unit, out bool absorbElement, out int elementalResistance);
}
