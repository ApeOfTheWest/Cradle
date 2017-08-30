using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Units/Blueprints/FromPrefab")]
public class PrefabBlueprint : UnitBlueprint {
	//unit blueprint that just creates a unit from a prefab of a unit and gets the unitcontroller component from it
	public UnitController prefab;

	//the relative position to place it at
	public Vector2 normalisedHomePosition = new Vector2();

	public override UnitController MakeUnit(GameObject unitHolder) {
		//create an instance of the gameobject and add it to the unit holder
		UnitController unit = Instantiate(prefab, new Vector3(0,0,0), Quaternion.identity) as UnitController;

		//if theres no unit controller then send an error message and destroy the object
		if (unit == null) {
			Debug.LogError ("Object instantiated was not a unit");

			Destroy (unit);
			return null;

		} else {
			unit.transform.parent = unitHolder.transform;

			//set the normalised position
			unit.normHomePosition = normalisedHomePosition;

			return unit;
		}
			
	}
}
