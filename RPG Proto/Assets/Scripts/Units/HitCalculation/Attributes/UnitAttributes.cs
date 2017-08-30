using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//holds a set of attributes required by a unit, used in combat calculations
[System.Serializable]
public class UnitAttributes {
	//the level that a resistance stat needs to be at or below for a unit to count as being weak to it
	//if a unit is weak to it then it will be broken by an attack of that type
	//this represents a 50% threshold
	public const int ELEMENT_WEAKNESS_THRESHOLD = -50;

	//resource stats
	public int maxHealth = 1;
	public int maxMagick = 0;

	//the speed of this unit, is interacted with through a property so that adjustments can be made to the base stat
	//100 is the default speed, any unit faster than that is faster than average
	//speed cannot be lower than 1 normally, as this would cause an eternally stopped unit
	public int speed = 100;

	//damage modifying stats

	//strength, this affects how much damage strength based abilities do
	public int strength = 1;
	//will, this affects how much damage magick based abilities do (usually, some magicks depend on other things)
	public int wisdom = 1;
	//dexterity, affects how much damage skill based abilities do
	public int dexterity = 1;

	//percentage based stats

	//acuuracy of the unit, any accuracy above 100 is greater than normal, any below 100 is less than normal 
	public int accuracy = 100;
	//chance of dodging an attack, the abilities accuracy has this subtracted by it before miss calculations are made
	public int dodgeChance = 0;
	//protection is the percenttage of all damage blocked after all damage resistance calcs (for obvs reasons this shouldn't reach 100)
	//this can be negative to increase damage taken
	public int protection = 0;

	//damage resistance stats

	//there are two mediums of dealing damage (physical and spritual)
	public int toughness = 0;
	public int will = 0;

	//how much rally is generated each turn
	//by default this will be 0 for most enemies, but for players this will usually be non zero
	public int rallyGeneration = 0;

	//and many elements
	//element resistance should have a true / false flag on whether the damage is absorbed or not
	//if damage is absorbed it is absorbed at a flat 100% rate and ignores the damage resistance stat
	public ElementalReistances elementalResistance = new ElementalReistances();

	//status resistance stats (poison, stun etc)
	public StatusResistances statusResistance = new StatusResistances();
}
