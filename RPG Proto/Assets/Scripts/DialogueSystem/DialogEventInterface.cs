using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the dialog event interface is used to link a dialog event to gameplay reactions, such as displaying a custom interface in the middle of a dialog stream
//or giving the player an item
//or making a player pay money for something
//or using input from a player
//or changing dialog (or sprites) based on the gamestate
//or displaying a picture
public interface DialogEventInterface {
	//standard events called when a dialog starts, finishes, or changes node
	//the dialog player should be passed in on start so that it can be chached in the case where custom ui and other tricks must be added to it
	//called before the first node is entered
	void DialogStart(DialogPlayer player);
	//called just before the dialog exits, save any last minute values and last minute events here
	void DialogExit();
	//called on a node change, gives the index number of the node being changed to so relevant events can be called
	//also can give an integer flag if one is given (useful as index numbers can shift whenever nodes are edited)
	//this is called after sprites are set so custom sprites can be used here
	void EnterNode(int nodeIndex, int nodeFlag);

	//called when a response is picked, just before exiting the node, (so a specific node can be linked to based on the value given)
	//give the response index and the flag in
	void ResponseSelected(int responseIndex, int responseFlag);
	//called when an integer response is chosen
	void IntegerResponse(int response);
	//called when a float response is chosen
	void FloatResponse(float response);
	//called when a string response is chosen
	void StringResponse(string response);

	//called when input is requested from the interface
	//the interface will usually cache the current node and respond appropriately
	int IntegerRequested();
	float FloatRequested();
	string StringRequested();
}
