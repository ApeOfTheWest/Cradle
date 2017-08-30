using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveHolder : MonoBehaviour {

	private GameObject leftHolder;
	private GameObject rightHolder;

	//list to hold the left units
	private List<UnitController> leftUnits = new List<UnitController>();
	public List<UnitController> LeftUnits {
		get {
			return leftUnits;
		}
	}
	//list to hold the right units
	private List<UnitController> rightUnits = new List<UnitController>();
	public List<UnitController> RightUnits {
		get {
			return rightUnits;
		}
	}

	//when awake get references to the left and right unit holder game objects
	void Awake() {
		leftHolder = transform.Find ("Left").gameObject;
		rightHolder = transform.Find ("Right").gameObject;
	}

	//method to add a unit to reserve on the left side
	public void AddLeftUnit(UnitController unit) {
		leftUnits.Add (unit);
		unit.transform.parent = leftHolder.transform;
		unit.LeftSide = true;
	}
	//method to add a unit to reserve on the right side
	public void AddRightUnit(UnitController unit) {
		rightUnits.Add (unit);
		unit.transform.parent = rightHolder.transform;
		unit.LeftSide = false;
	}

}
