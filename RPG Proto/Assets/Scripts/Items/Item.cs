using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the base class of all items that can drop off of monsters and be held in a characters inventory
//(including key items)
public abstract class Item : ScriptableObject {
	//each item has a sprite associated with it for displat in the inventory
	private Sprite itemIcon;

	//shows whether the item is stackable or not
	//note: being non stackable doesn't mean being unique, it may just mean that different instances have different stats
	private bool stackable = true;

	//shows whether item is key or not, if it is key then the player will not be allowed to drop it
	private bool keyItem = false;

	//each item has a name
	private string itemName = "";

	//each item has a list of menu options that can be called upon when clicked on in the menu
	public abstract void GetMenuActions();

	public bool isStackable() { 
		return stackable; 
	}

}
