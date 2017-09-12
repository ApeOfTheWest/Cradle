using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

	//the system settings (saved independantly from the the save file as these are applied on a system basis not a session basis)
	public SystemSettings systemSettings = null;

	//the audio mixer, store here temporarily
	public AudioMixer mixer;

	void Awake() {
		//try to load the stored system settings from the computer


		//if they can't be loaded then create a new set
		if (systemSettings == null) {
			systemSettings = new SystemSettings ();
			//get the current resolution
			//systemSettings.Resolution = Screen.currentResolution;
		}

		//set up the player roster
		roster = GetComponentInChildren<PlayerRoster>(true);

		//and populate it with lotte
		roster.AddUnitToParty (Instantiate(lotte));
	}

	//apply the system settings in start
	void Start() {
		//apply the system settings
		systemSettings.ApplySettings(mixer);
	}
}
