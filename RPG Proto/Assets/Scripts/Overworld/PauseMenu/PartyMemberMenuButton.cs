using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//displays information for the given party member
//can be clicked on to get more info on the given party member or move the formation of them
//party members can be moved in formation to be active or in reserve
public class PartyMemberMenuButton : MonoBehaviour {
	//initialise the button with a party unit
	//also pass in whether this is an active unit or not
	//and pass in the party tab so that callbacks can be made
	public void Initialise(PartyTab tab, PartyUnit partyUnit, bool active) {

	}

	//cleanup after it
	public void Cleanup() {

	}
}
