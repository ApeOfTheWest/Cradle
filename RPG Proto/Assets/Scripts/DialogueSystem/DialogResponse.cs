using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogResponse {
	//the index of the node to link to when this response is pressed, if -1 this ends the dialog
	public int linkNodeIndex = -1;

	//the text to label the response as
	public string responseLabel = "";

	//the flag to given when the response is picked
	public int responseFlag = -1;
}
