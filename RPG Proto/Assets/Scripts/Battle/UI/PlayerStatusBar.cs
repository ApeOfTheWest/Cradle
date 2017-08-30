using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is an abstract class as there are different types of status bar that display different resources
public abstract class PlayerStatusBar : MonoBehaviour {
	//used to highlight the status bar, must be called through a property so graphics can be updated
	protected bool highlighted = false;
	public virtual bool Highlighted {
		get {
			return highlighted;
		}
		set {
			highlighted = value;
		}
	}

	//the player being represented by this status bar
	protected PartyUnit playerDisplay = null;
	//get the player
	public PartyUnit PlayerDisplay {
		get {
			return playerDisplay;
		}
	}

	//used to assign a player to a status bar
	public virtual void AssignPlayer(PartyUnit player) {
		//unhighlight when a new player is assigned
		Highlighted = false;
		//set the player
		playerDisplay = player;
	}
}
