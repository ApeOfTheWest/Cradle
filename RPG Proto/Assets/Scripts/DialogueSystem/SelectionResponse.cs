using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionResponse : DialogueResponse {
	//the index of the node to link to when this response is pressed, if -1 this ends the dialog
	public int linkNodeIndex = -1;

	//the text to label the response as
	public string responseLabel = "";
}
