using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebuff : StatusEffect {
	//buffs may be curable / incurable
	//incurable buffs cant be dispelled
	//incurable debuffs cant be treated
	bool curable = true;

	//each buff / debuff status effect has a list of status modifiers, if this list is empty then this will be considered to not affect modifers and they wont be recalculated on adding
	//or taking away of this
	//if modifiers are present then they will be added automatically when this status effect is
	public List<AttributeModification> modiferChanges = new List<AttributeModification>();

	//if only the modifiers are wanted then this class can leave the other methods empty
	public override void StatusAdded(UnitController unit) {}
	public override void StatusRemoved(UnitController unit) {}
	public override void UnitTurnStarted(UnitController unit) {}
	public override void UnitTurnEnded(UnitController unit) {}
}
