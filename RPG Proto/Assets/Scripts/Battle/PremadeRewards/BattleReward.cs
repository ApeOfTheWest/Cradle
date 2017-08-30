using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//represents all that may be gained after a battle
//a flat reward is given for winning each battle, and then an additional reward based on the units killed
[CreateAssetMenu (menuName = "BattleReward")]
public class BattleReward : ScriptableObject {
	//experience rewarded
	public float experience = 0;
	//money rewarded
	public uint money = 0;
	//items rewarded
	public List<ItemSlot> items = new List<ItemSlot>();

	//method to add experience
	public void addExperience(float newExperience) {
		experience += newExperience;
	}

	public void addMoney(uint addMoney) {
		money += addMoney;
	}

	//method to add an item to the battle rewards, attempts to stack together stackable objects
	public void addItems(ItemSlot newItem) {
		if (newItem.getItem ().isStackable() == true) {
			//search for a similar item that this can be added to
			ItemSlot match = null;

			Item compareItem = newItem.getItem ();

			for (int i = 0; i < items.Count; i++) {
				//check for match
				Item currItem = items[i].getItem();

				if (currItem.isStackable() == true && currItem.GetType () == compareItem.GetType ()) {
					match = items[i];

					break;
				}
			}

			//if a stackable match was found then combine it
			if (match != null) {
				//combine and throw away old slot
				match.CombineSlot (newItem);
			} else {
				items.Add (newItem);
			}

		} else {
			//just add the new item to the list
			items.Add(newItem);
		}
	}
}
