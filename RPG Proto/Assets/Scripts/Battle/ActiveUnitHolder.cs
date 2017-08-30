using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveUnitHolder : MonoBehaviour {

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

	//method to add a unit to battle on the left side
	//takes battle scenery by reference so normalised positions can be calculated as absolute positions
	public void AddLeftUnit(UnitController unit, BattleScenery scenery) {
		leftUnits.Add (unit);
		unit.transform.parent = leftHolder.transform;
		unit.homePosition = scenery.LeftUnitNormToHome (unit.normHomePosition);
		unit.LeftSide = true;

		AddUnit (unit);
	}
	//method to add a unit to battle on the right side
	public void AddRightUnit(UnitController unit, BattleScenery scenery) {
		rightUnits.Add (unit);
		unit.transform.parent = rightHolder.transform;
		unit.homePosition = scenery.RightUnitNormToHome (unit.normHomePosition);
		unit.LeftSide = false;

		AddUnit (unit);
	}
	//generic unit adding method that sets up stuff that applies to right and left units
	private void AddUnit(UnitController unit) {
		unit.transform.position = unit.homePosition;
		//set the units turn cooldown to 1 by default, if this behaviour is undesired then the unit should change in when made active
		unit.TurnCooldown = 1.0f;
		//tell the unit that it was made active
		unit.UnitMadeActive();
	}
}
