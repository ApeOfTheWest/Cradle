using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//responsible for playing back a dialog and notifying the event system of any events (if one is attached)
//also must handle requests from a dialog event system to display custom ui if requested and clean up nicely once the dialog is finished
public class DialogPlayer {
	//the node that will currently be switched to after the current one is done
	private int nextNode = -1;

	//lock the dialog system, prevents transitioning to the next node while locked
	//used while custom ui or animation is being played to prevent skipping ahead
	private bool locked;
	public bool Locked {
		get {
			return locked;
		}
		set {
			locked = value;
		}
	}

	//the next node isn't directly changed to, rather the next node to change to is qued and will be changed to when the current node exits
	public void QueNextNode(int nextNode) {
		this.nextNode = nextNode;
	}

	//call to force a change to the next node qued, used by custom dialog ui code and for interuptions
	public void GotoNextNode() {

	}
}