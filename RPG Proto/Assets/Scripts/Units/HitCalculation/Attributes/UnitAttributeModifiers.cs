using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//contains two copies of unit attributes, one is the base copy and another is the modified copy
//it also contains modifiers which can be altered based on abilities / talents / buffs etc
//the modified version wont be updated automatically so it is the job of the unit to call update as required
[System.Serializable]
public class UnitAttributeModifiers {
	//base attributes, dont tamper with these unless changes should be permenant
	public UnitAttributes baseAttributes = new UnitAttributes();
	//modified attributes
	public UnitAttributes modifiedAttributes = new UnitAttributes();

	//the modifiers, these come in a pair of multipliers and flat additions
	//multipliers are always used before additions (so additions wont be multiplied)
	//multipliers should have their percentages added together rather than multiplied together when combining their effects

	//regular attributes
	public AttributeModifier maxHealth = new AttributeModifier();
	public AttributeModifier maxMagick = new AttributeModifier();
	public AttributeModifier speed = new AttributeModifier();
	public AttributeModifier strength = new AttributeModifier();
	public AttributeModifier wisdom = new AttributeModifier();
	public AttributeModifier dexterity = new AttributeModifier();
	public AttributeModifier toughness = new AttributeModifier();
	public AttributeModifier will = new AttributeModifier();
	public AttributeModifier accuracy = new AttributeModifier();
	public AttributeModifier dodgeChance = new AttributeModifier();
	public AttributeModifier protection = new AttributeModifier();

	//elemental attributes

	//status attributes

	//and rally generation
	public AttributeModifier rally = new AttributeModifier();

	//resets the modifiers to their default, should be called to nullify the modifiers before a rebuild
	public void ResetModifers() {
		maxHealth.Reset ();
		maxMagick.Reset ();
		speed.Reset ();
		strength.Reset ();
		wisdom.Reset ();
		dexterity.Reset ();
		toughness.Reset ();
		will.Reset ();
		accuracy.Reset ();
		dodgeChance.Reset ();
		protection.Reset ();

		rally.Reset ();
	}

	public void UpdateModifiedAttributes() {
		//calculate the modified stats
		modifiedAttributes.maxHealth = CalculateModifiedAttribute(baseAttributes.maxHealth, maxHealth);
		modifiedAttributes.maxMagick = CalculateModifiedAttribute(baseAttributes.maxMagick, maxMagick);
		modifiedAttributes.speed = CalculateModifiedAttribute(baseAttributes.speed, speed);
		modifiedAttributes.strength = CalculateModifiedAttribute(baseAttributes.strength, strength);
		modifiedAttributes.wisdom = CalculateModifiedAttribute(baseAttributes.wisdom, wisdom);
		modifiedAttributes.dexterity = CalculateModifiedAttribute(baseAttributes.dexterity, dexterity);
		modifiedAttributes.toughness = CalculateModifiedAttribute(baseAttributes.toughness, toughness);
		modifiedAttributes.will = CalculateModifiedAttribute(baseAttributes.will, will);
		modifiedAttributes.accuracy = CalculateModifiedAttribute(baseAttributes.accuracy, accuracy);
		modifiedAttributes.dodgeChance = CalculateModifiedAttribute(baseAttributes.dodgeChance, dodgeChance);
		modifiedAttributes.protection = CalculateModifiedAttribute(baseAttributes.protection, protection);

		modifiedAttributes.rallyGeneration = CalculateModifiedAttribute (baseAttributes.rallyGeneration, rally);
	}

	//used to calculate each modified attribute that has a percentage and raw modifier applied to it
	public int CalculateModifiedAttribute(int baseAttribute, AttributeModifier modifier) {
		//calculations should all be done as floats and then converted back into integers
		float modifiedBase = (float) baseAttribute;

		//percentage modifers are 0 by default, so to convert to a full percentage add 100 and then divide it in float form
		float percentModifier = (100f + (float)modifier.percentChange) / 100f;

		//now multiply by the modifier
		modifiedBase *= percentModifier;

		//and convert back to an int, adding the raw modifier
		int modAttrib = ((int)modifiedBase) + modifier.rawChange;

		return modAttrib;
	}
}
