using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the roster holds all the units availiable to the player along with their abilities
//the party shows which units are active and in which positions

//essentially the roster tracks the gameobjects and meta data related to party members while the party only tracks battle relevant stuff
public class PlayerRoster : MonoBehaviour {
	public ActiveParty selectedParty = new ActiveParty();

	//method to add a unit to the party, childs the unit to the party unit holder and adds it to the party roster
	public void AddUnitToParty(PartyUnit unit) {
		selectedParty.AddPartyMember (unit);

		unit.transform.parent = transform;
	}
}
