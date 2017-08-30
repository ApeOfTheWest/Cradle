using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//all non key items can be dropped and trigger an action when dropped
//takes an int on the function which gives the number of these items being dropped
public abstract class NonKeyItem : Item {

	public abstract void DropItem(uint dropNumber);
}
