using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeModification {
	//the type of attribute to modify
	public Attribute toModify = null;

	public AttributeModifier modification = new AttributeModifier();

	//call to apply the modifier to the given set of modifiers, will do nothing if no attribute is selected
	public void ApplyModifier(UnitAttributeModifiers attributeModifiers) {
		if (toModify != null) {
			toModify.AddModifier (attributeModifiers, modification.percentChange, modification.rawChange);
		}
	}
}
