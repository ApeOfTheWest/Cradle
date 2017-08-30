using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//scriptable object designed to be used as a enum, will automatically find and modify the targeted attribute in an attribute modifier
//or attribute collection when prompted
public abstract class Attribute : ScriptableObject {
	//adds to the modifier that correlates to this attribute
	public abstract void AddModifier(UnitAttributeModifiers attributeModifier, int percentageModifier, int rawModifier);

	//returns the value of this attribute from a list of them
	public abstract int GetValue(UnitAttributes attributes);

	//sets the value of this attribute from a group of them
	public abstract void SetValue(UnitAttributes attributes, int value);
}
