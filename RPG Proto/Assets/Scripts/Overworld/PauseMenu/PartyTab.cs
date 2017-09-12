using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTab : PauseMenuContent {
	private PlayerRoster roster = null;
	public PartyMemberMenuButton partyMemberButton = null;

	//the current list of buttons (in order) of active units
	private List<PartyMemberMenuButton> activeButtons = new List<PartyMemberMenuButton>();
	//current list of buttons of reserves
	private List<PartyMemberMenuButton> reserveButtons = new List<PartyMemberMenuButton>();

	public override void Initialise (PersistentData data) {
		//store referance to party member data
		roster = data.roster;

		//get the active members
		PartyUnit[] active = roster.selectedParty.CollateActiveUnits();
		//get the reserve members
		PartyUnit[] reserve = roster.selectedParty.CollateReserveUnits();

		//add to the pool as many party members as there are, add them in order as well
		for (int i = 0; i < active.Length; i++) {
			activeButtons.Add(Instantiate(partyMemberButton, transform));
			activeButtons [i].Initialise (this, active [i], true);
		}

		for (int i = 0; i < reserve.Length; i++) {
			reserveButtons.Add(Instantiate(partyMemberButton, transform));
			reserveButtons [i].Initialise (this, reserve [i], false);
		}
	}

	public override void Cleanup() {
		roster = null;

		//delete the buttons and clear the lists
		for (int i = activeButtons.Count - 1; i >= 0; i--) {
			Destroy (activeButtons [i].gameObject);
			activeButtons.RemoveAt (i);
		}
		//delete the buttons and clear the lists
		for (int i = reserveButtons.Count - 1; i >= 0; i--) {
			Destroy (reserveButtons [i].gameObject);
			reserveButtons.RemoveAt (i);
		}
	}
}
