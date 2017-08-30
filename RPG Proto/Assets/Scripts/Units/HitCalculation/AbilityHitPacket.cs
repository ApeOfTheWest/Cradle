using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class contains all the data and effects that may need to be transferred as part of an ability hit
//this includes damage/healing, status effects, accuracy, and any additional buffs / debuffs
[System.Serializable]
//the ability hit packet is attached to an ability, and as such it shouldnt be modified outside of an ability, it also shouldn't be cloned
//it should just provide a way to calculate an apply a damage and other effects based on the source and target of the hit
public class AbilityHitPacket {
	public AbilityHitPacket() {
	}

	//copy constructor
	public AbilityHitPacket(AbilityHitPacket toCopy) {

	}

	//whether or not the hit should ignore resistances (good for healing and 'true' damage attacks)
	public bool ignoreResistance = false;
	//whether or not the hit should be evadable (certain support abilities a char wouldnt want to evade, but can miss)
	public bool evadable = true;
	//whether or the the hit should be missable (use to produce guaranteed hit abilities, like items)
	public bool missable = true;

	//how to calculate damage, can be based on users stats
	public DamageCalculationPacket damage = new DamageCalculationPacket();

	//the type of the hit, can be physical or magical
	public enum HitType
	{
		Physical, Magical
	}
	public HitType hitType = HitType.Physical;

	//the element of the hit, chosen from scriptable object list of elements designed to be able to find relevant resistances in a target
	//element can be null, for non elemental damage
	public Element element = null;

	//the chance of hitting as a percentage, can also be modified by a dodge chance on the enemies side
	public int accuracy = 100;

	//any possible effects to apply on top of damage
	public List<StatusChange> additionalEffects = new List<StatusChange>();
}
