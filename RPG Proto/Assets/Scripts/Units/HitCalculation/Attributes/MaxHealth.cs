using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attribute", menuName = "Attributes/MaxHealth", order = 1)]
public class MaxHealth : Attribute {
	
	//adds to the modifier that correlates to this attribute
	public override void AddModifier(UnitAttributeModifiers attributeModifier, int percentageModifier, int rawModifier) {
		attributeModifier.maxHealth.percentChange += percentageModifier;
		attributeModifier.maxHealth.rawChange += rawModifier;
	}

	//returns the value of this attribute from a list of them
	public override int GetValue(UnitAttributes attributes) {
		return attributes.maxHealth;
	}

	//sets the value of this attribute from a group of them
	public override void SetValue(UnitAttributes attributes, int value) {
		//the value being set must be at least 1
		if (value > 0) {
			attributes.maxHealth = value;
		} else {
			attributes.maxHealth = 1;
		}
	}
}
