using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode {
	//the possible responses, can be 0 for a regular string of text
	//responses can return integers, floats or strings rather than just text
	//if one of these special responses are detected then all other responses will be disregarded
	//these special responses will be go to the next node by default, to do anything special based on input the event must be intercepted
	public List<DialogueResponse> responses = new List <DialogueResponse>();
	//if there are no responses then the tree will just go to the next node in the index by default
	//if this is the last node then the dialogue will end

	//the actual meat of the node, shows what potraits to display on the left, right and what text should be displayed
	public Sprite leftPortrait;
	public Sprite rightPortrait;

	public string nodeText = "";

	//flag used to identify this node for event purposes
	public int nodeFlag = -1;
}