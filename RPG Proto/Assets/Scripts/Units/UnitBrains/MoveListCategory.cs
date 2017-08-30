using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//each move list cotegory holds a movelist node and the name of the category
[System.Serializable]
public class MoveListCategory {
	public string categoryName = "Category";

	public List<AbilitySlot> moves = new List<AbilitySlot>();
}
