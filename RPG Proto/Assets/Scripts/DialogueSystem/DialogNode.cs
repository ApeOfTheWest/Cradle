using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogNode {
	//the possible responses, can be 0 for a regular string of text
	//responses can return integers, floats or strings rather than just text
	//if one of these special responses are detected then all other responses will be disregarded
	//these special responses will be go to the next node by default, to do anything special based on input the event must be intercepted
	public List<DialogResponse> responses = new List <DialogResponse>();
	//if there are no responses then the tree will just go to the next node in the index by default
	//if this is the last node then the dialogue will end

	//the actual meat of the node, shows what potraits to display on the left, right and what text should be displayed
	public Sprite leftPortrait;
	public Sprite rightPortrait;

	//the name of the speaker, if blank no nametag will be displayed
	public string speakerName = "";
	public string nodeText = "";

	//whether to end the dialog after this node (ignored if node has responses)
	public bool endDialog = false;

	//flag used to identify this node for event purposes
	public int nodeFlag = -1;
}