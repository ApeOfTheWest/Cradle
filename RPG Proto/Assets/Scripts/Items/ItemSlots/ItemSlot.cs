using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//each item is held in an itemSlot in the players inventory
//the itemslot holds the type of item, responds to requests to use it and holds savable data relating to that item, such as how many there are or what state the item is in
public class ItemSlot {
	private Item item;

	//method for combining an item slot with this slot
	//if the two slots can't be combined then nothing will happen, if they can be combined then this may change one item using the other or may stack similar items together
	public void CombineSlot(ItemSlot sourceSlot) {
		//first check if the item is stackable, and then check if the items are the same class
		if (item.isStackable() == true && item.GetType() == sourceSlot.getItem().GetType()) {
			//add the sourceslot number of items to the stack

		}
	}

	//gets the item to modify
	public Item getItem() {
		return item;
	}
}
