using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//responsible for playing back a dialog and notifying the event system of any events (if one is attached)
//also must handle requests from a dialog event system to display custom ui if requested and clean up nicely once the dialog is finished
public class DialogPlayer {
	//the dialog ui to update and recieve commands from
	public DialogUI ui;

	//the node that will currently be switched to after the current one is done
	private int nextNode = -1;

	//the interface being used with the currently playing dialog
	//can be null
	private DialogEventInterface eventInterface = null;
	//the conversation being played
	private DialogTree conversation = null;

	//the current node
	private DialogNode currNode = null;

	public void StartConversation(DialogTree conversation, DialogEventInterface eventInterface) {
		this.conversation = conversation;
		this.eventInterface = eventInterface;

		if (eventInterface != null) {
			eventInterface.DialogStart (this);
		}

		//make next node the root node and call goto next
		QueNextNode(conversation.rootNodeIndex);
		GotoNextNode ();
	}

	//the next node isn't directly changed to, rather the next node to change to is qued and will be changed to when the current node exits
	public void QueNextNode(int nextNode) {
		this.nextNode = nextNode;
	}

	//call to force a change to the next node qued, used by custom dialog ui code and for interuptions
	public void GotoNextNode() {
		//first display the next node (or exit if it doesn't exist)

		if (conversation.nodes.Count > nextNode && nextNode >= 0) {
			currNode = conversation.nodes [nextNode];

			//retrieve node data if needed
			ui.DisplayNodeContents(RetireveNodeData(currNode));

			if (currNode.endDialog == false) {
				//if not ending dialog set the next node to be equal to 1 more than the current index by default
				QueNextNode (nextNode + 1);
			} else {
				//if ending dialog then que -1
				QueNextNode(-1);
			}

			//ntify interface
			if (eventInterface != null) {
				eventInterface.EnterNode (nextNode, currNode.nodeFlag);
			}
		} else {
			//otherwise end the conversation
			EndConversation();
		}

	}

	//selects the given response index, will send an event to the interface and go to the chosen node
	public void SelectResponse(int index) {
		if (index >= 0 && index < currNode.responses.Count) {
			DialogResponse response = currNode.responses [index];
			if (eventInterface != null) {
				eventInterface.ResponseSelected (index, response.responseFlag);
			}

			QueNextNode(response.linkNodeIndex);
			GotoNextNode ();
		}
	}

	//takes a node, retrieves necessary data from the interface and passes it into a copy (dont change the origional)
	private DialogNode RetireveNodeData(DialogNode toRetrieve) {
		//return origional 4 now
		return toRetrieve;
	}

	//call when ending a conversation, unpause the world if it was paused
	//clear refs
	public void EndConversation() {
		//tell the imterface the dialog is ending
		if (eventInterface != null) {
			eventInterface.DialogExit();
		}

		eventInterface = null;
		conversation = null;
		currNode = null;
					
		ui.EndConversation ();

	}
}