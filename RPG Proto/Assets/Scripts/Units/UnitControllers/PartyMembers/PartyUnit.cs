using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PartyUnit : UnitController {
	//the party unit is similar to a regular unit but must also provide functions for displaying a status bar and displaying a selectable move list

	//the sprite to use for the status bar portraits
	[SerializeField]
	private Sprite statusBarPortrait;

	//function that returns a sprite for use in a status bar
	public Sprite StatusBarPortrait {
		get {
			return statusBarPortrait;
		}
	}

	/*protected void Awake() {
		base.Awake();

		//party members should use a party unit turn brain
		turnBrain = ScriptableObject.CreateInstance<PartyMemberBrain>();
	}*/
		
	public override void InitialiseBattle(BattleController setBattle) {
		base.InitialiseBattle (setBattle);

		//a party member should also tell the battle to use its rally field for the rally meter
		setBattle.ActiveBattle.battleUI.EnableRallydisplay(Rally);
	}

	public override void UnitMadeActive() {
		base.UnitMadeActive ();

		//and add itself to the health bar selection, only do this when added to the battle as an active unit and not a reserve
		battle.ActiveBattle.battleUI.AddStatusBar(this, 0);
	}
	public override void UnitMadeInactive() {
		base.UnitMadeInactive ();

		battle.ActiveBattle.battleUI.RemoveStatusBar(this);
	}
}
