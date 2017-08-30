using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//persistent data holds all the data that must persist between levels
public class PersistentData : MonoBehaviour {
	//the current player roster
	public PlayerRoster roster;
	//the player's inventory

	//the flags that persist through levels

	//the players settings

	public PartyUnit lotte;

	//the name of the room to enter on scene transition
	public string entryRoom = "";
	//the name of the entry point to use on scene transition
	public string entryPoint = "";

	void Awake() {
		//set up the player roster
		roster = GetComponentInChildren<PlayerRoster>(true);

		//and populate it with lotte
		roster.AddUnitToParty (Instantiate(lotte));
	}
}
