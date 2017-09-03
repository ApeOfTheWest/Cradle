using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a class that holds all the data needed for a single event of dialogue
[CreateAssetMenu(fileName = "DialogTree", menuName = "DialogTree", order = 1)]
public class DialogTree : ScriptableObject {
	//all dialogue nodes that are a part of the tree, each node can link into other nodes, either through a chosen response or automatically
	//these nodes are not serializable due to their structure, a serializable version must be used instead
	public List<DialogNode> nodes = new List<DialogNode>();

	//the index of the node to start with, if -1 end convo straight away
	public int rootNodeIndex = 0;

	//use to remove a node, all references to the node should be nulled before the node is removed
	//and also any references to nodes above the deletion point will have to be shifted by one downwards
	public void RemoveNode(int index) {
		//make sure the node exists
		if (index >= 0 && index < nodes.Count) {
			//delete the node
			nodes.RemoveAt(index);

			AdjustIndexDeleted(ref rootNodeIndex, index);

			//iterate through remaining nodes, null any direct refs
			//decrement any references to indexes larger than the deleted one
			for (int i = 0; i < nodes.Count; i++) {
				for (int j = 0; j < nodes [i].responses.Count; j++) {
					AdjustIndexDeleted (ref nodes [i].responses [j].linkNodeIndex, index);
				}
			}
		}
	}

	//use to add a node
	public void AddNode(DialogNode toAdd) {
		nodes.Add (toAdd);
	}

	//use to add a node at an index
	public void AddNodeAtIndex(DialogNode toAdd, int addIndex) {
		//add the node
		AddNode(toAdd);
		//then move it
		MoveNode(nodes.Count -1, addIndex);
	}

	//use to move a node to a new index
	public void MoveNode(int nodeIndex, int newIndex) {
		//make sure both values exist
		//and are valid, skip the process if not
		//also make sure the current index is not the destination
		if (nodeIndex >= 0 && nodeIndex < nodes.Count && newIndex >= 0 && newIndex < nodes.Count && newIndex != nodeIndex) {
			//first up cache the node being moved
			DialogNode moving = nodes[nodeIndex];
			//then remove it
			nodes.RemoveAt(nodeIndex);
			//add it at the destination, and update the indexes 
			nodes.Insert(newIndex, moving);

			//adjust root node as well
			AdjustIndexMoved(ref rootNodeIndex, nodeIndex, newIndex);

			for (int i = 0; i < nodes.Count; i++) {
				for (int j = 0; j < nodes [i].responses.Count; j++) {
					AdjustIndexMoved (ref nodes [i].responses [j].linkNodeIndex, nodeIndex, newIndex);
				}
			}
		}
	}

	private void AdjustIndexMoved(ref int toAdjust, int origionalPosition, int newPosition) {
		//do nothing if index is negative as this signifies a dialog end
		if (toAdjust >= 0) {
			//if the index to adjust is equal to the origional node then set to the new pos
			if (toAdjust == origionalPosition) {
				toAdjust = newPosition;
			} else if (origionalPosition < newPosition && toAdjust >= origionalPosition && toAdjust <= newPosition) {
				//if the origional is less than the new position, and the index is in the boundary then the node index will have decremented
				toAdjust --;
			} else if (newPosition < origionalPosition && toAdjust >= newPosition && toAdjust <= origionalPosition) {
				//if new position is less than the origional then the index will have incrimented
				toAdjust ++;
			}
		}
	}

	//method to adjust the index reference to a node when one had been deleted, changes the index based on the index of deletion
	private void AdjustIndexDeleted(ref int toAdjust, int indexDeleted) {
		//do nothing if index is negative as this signifies a dialog end
		if (toAdjust >= 0) {

			if (toAdjust == indexDeleted) {
				//set to null
				toAdjust = -1;
			} else if (toAdjust > indexDeleted) {
				//decrement by one to bring it in line
				toAdjust -= 1;
			}
				
		}
	}

	//get the node related to the index or null if the index given is out of bounds
	//used extensively by the dialog system and custom editor window
	public DialogNode GetNode(int index) {
		if (index >= 0 && index < nodes.Count) {
			return nodes [index];
		} else {
			return null;
		}
	}
}
