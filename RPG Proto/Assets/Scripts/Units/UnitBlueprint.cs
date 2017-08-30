using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the unit itself is a blueprint that returns an instance of a unit, made from a prefab
public abstract class UnitBlueprint : ScriptableObject {

	//create an instance of the battle unit, return the controller and add the game object that represents it as a child to the given gameobject
	public abstract UnitController MakeUnit(GameObject unitHolder);
}
