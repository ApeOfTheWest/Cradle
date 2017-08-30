using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a class that holds all the data needed for a single event of dialogue
public class DialogueTree : ScriptableObject {
	//all dialogue nodes that are a part of the tree, each node can link into other nodes, either through a chosen response or automatically
	//these nodes are not serializable due to their structure, a serializable version must be used instead
	public List<DialogueNode> nodes = new List<DialogueNode>();

	//the index of the node to start with, if -1 end convo straight away
	public int rootNodeIndex = 0;

	//use to remove a node, all references to the node should be nulled before the node is removed
	//and also any references to nodes above the deletion point will have to be shifted by one downwards
	public void RemoveNode(int index) {
		//make sure the node exists
		if (index >= 0 && index < nodes.Count) {
			//delete the node
			nodes.RemoveAt(index);

			//iterate through remaining nodes, null any direct refs
			//decrement any references to indexes larger than the deleted one
			for (int i = 0; i < nodes.Count; i++) {
				for (int j = 0; j < nodes [i].responses.Count; j++) {

				}
			}
		}
	}

	//use to add a node
	public void AddNode(DialogueNode toAdd) {
		nodes.Add (toAdd);
	}

	//method to adjust the index reference to a node when one had been deleted, changes the index based on the index of deletion
	public void AdjustIndexDeleted(ref int toAdjust, int indexDeleted) {
		if (toAdjust == indexDeleted) {
			//set to null
			toAdjust = -1;
		} else if (toAdjust > indexDeleted) {
			//decrement by one to bring it in line
			toAdjust -= 1;
		}
	}

	//get the node related to the index or null if the index given is out of bounds
	//used extensively by the dialog system and custom editor window
	public DialogueNode GetNode(int index) {
		if (index >= 0 && index < nodes.Count) {
			return nodes [index];
		} else {
			return null;
		}
	}
}
