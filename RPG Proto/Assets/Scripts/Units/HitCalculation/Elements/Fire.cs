using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fire", menuName = "Elements/Fire", order = 1)]
public class Fire : Element {
	public override void GetUnitResistanceToElement(UnitController unit, out bool absorbElement, out int elementalResistance) {
		ElementResistance resist = unit.Attributes.elementalResistance.fire;

		absorbElement = resist.absorb;
		elementalResistance = resist.resistance;
	}
}
