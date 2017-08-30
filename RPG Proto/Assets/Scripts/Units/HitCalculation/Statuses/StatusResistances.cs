using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusResistances {
	//lists of resistances to common status ailments
	public StatusResistancePair poison = new StatusResistancePair();
	public StatusResistancePair death = new StatusResistancePair();

	//resistance to uncommon status ailments, these are any programmed status that doesn't fit the standard types
	//known generically as debuffs
	public StatusResistancePair debuff = new StatusResistancePair();
	//as debuffs can be anything most monsters will have no debuff resistance, and players should have very limited resistance if availiable
	//a few bosses may be resistant
}
