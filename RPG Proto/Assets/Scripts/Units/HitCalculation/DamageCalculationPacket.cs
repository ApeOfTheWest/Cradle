using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the damage calculation packet provides a way to calculate damage based on the attributes of the user of the ability
//it also provides a way to randomise the damage done by a certain percentage
//if damage is negative then this will heal instead
[System.Serializable]
public class DamageCalculationPacket {
	//the raw damage to do, disregarding any scaling effects
	public int baseDamage = 0;

	//the amount to scale by based on strength
	public int strengthScaling = 0;
	//and wisdom
	public int wisdomScaling = 0;
	//and dexterity
	public int dexterityScaling = 0;

	//the percentage amount to scatter the damage by
	//should be small but there for variability
	//some abilities will have wide scatter, some will have barely any scatter
	public int percentScatter = 10;

	//method to check whether the ability hit packed is designed to do damage
	//(required because even if damage is 0 after calculations the 0 should be displayed if the ability was intended to damage)
	public bool IsDamaging() {
		//check if any of the modifier in the damage calc packet are non zero
		if (baseDamage != 0 || strengthScaling != 0 || wisdomScaling != 0 || dexterityScaling != 0) {
			return true;
		} else {
			return false;
		}
	}

	//method that takes the user of this packet so that their stats can be used with the scatter percent to calculate the damage they should do
	public int CalculateDamage(UnitController user) {
		//add the base damage to start with
		float calculatedDamage = (float)baseDamage;
		//then add the scaling damages based on the user's attributes
		calculatedDamage += ((float)strengthScaling / 100f) * (float)user.Attributes.strength;
		calculatedDamage += ((float)wisdomScaling / 100f) * (float)user.Attributes.wisdom;
		calculatedDamage += ((float)dexterityScaling / 100f) * (float)user.Attributes.dexterity;

		//calculate the amount to scatter by using rng
		float floatPercentScatter = (float)percentScatter / 100f;
		float scatterMultiplier = 1f + Random.Range(-floatPercentScatter, floatPercentScatter);
		calculatedDamage *= scatterMultiplier;

		return Mathf.RoundToInt(calculatedDamage);
	}
}
