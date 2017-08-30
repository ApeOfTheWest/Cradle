using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the active battle controller is a part of the battle controller that is only tasked with controlling the battle, 
//and not concerned with how to set up before a battle or how to conclude after the battle, delegating to the main battle controller for these tasks
public class ActiveBattleController : MonoBehaviour {

	private BattleController battle;

	//the canvas for all battle ui to be placed on (ensure rendering order and allows entire sections to be disabled)
	private Canvas battleCanvas;

	//the battle ui, can be fully customised if desired
	public BattleUI battleUI;

	//the gameobject on the canvas to attach overlays to, overlays can be customly added by units and code so this is necessary for cleanup
	private GameObject overlayUIHolder;
	public GameObject OverlayUIHolder {
		get {
			return overlayUIHolder;
		}
	}

	//the rally resource to share with the left units
	//if any unit wants its own independant rally meter then it can reject this one in the setter method
	private Rally leftRally = new Rally();
	public Rally LeftRally {
		get {
			return leftRally;
		}
	}
	//the rally resource to share with the right units
	private Rally rightRally = new Rally();
	public Rally RightRally {
		get {
			return rightRally;
		}
	}

	//an object to attach all active units to in the heirchy
	private ActiveUnitHolder activeUnits;
	public ActiveUnitHolder ActiveUnits {
		get {
			return activeUnits;
		}
	}

	//an object to attach all reserve units to in the heirchy
	private ReserveHolder reserveUnits;
	public ReserveHolder ReserveUnits {
		get {
			return reserveUnits;
		}
	}

	//the script attached camera that also holds the 3 supplementary game objects
	/*private static ParallaxCamControl battleCamera;
	public static ParallaxCamControl BattleCamera {
		get {
			return battleCamera;
		}
	}
	//get the orthographic part of the camera control
	public Camera OrthographicBattleCamera {
		get {
			return battleCamera.OrthoCamera;
		}
	}
	//the script that follows a player
	private static CameraFollower cameraFollow;
	public static CameraFollower CameraFollow {
		get {
			return cameraFollow;
		}
	}*/

	//an object to store temporary visual effects on
	private EffectsHolder effectsHolder;
	public EffectsHolder EffectsHolder {
		get {
			return effectsHolder;
		}
	}

	//list of win conditions for the left side
	//even if conditions are null one side will win if all units are defeated
	public List<WinCondition> leftWinConditions = new List<WinCondition>();
	//list of win conditions for the right side
	public List<WinCondition> rightWinConditions = new List<WinCondition>();
	//method to call when the left wins (usually victory screen)
	public VictoryMethod leftVictory;
	//method to call when the right wins (usually game over screen)
	public VictoryMethod rightVictory;

	void Awake() {
		activeUnits = GetComponentInChildren<ActiveUnitHolder> ();
		reserveUnits = GetComponentInChildren<ReserveHolder> ();

		//battleCamera = GetComponentInChildren<ParallaxCamControl> ();
		//cameraFollow = battleCamera.gameObject.GetComponent<CameraFollower> ();

		battleCanvas = GetComponentInChildren<Canvas> (true);
		overlayUIHolder = battleCanvas.transform.Find ("OverlayUIHolder").gameObject;
		battleUI = battleCanvas.GetComponentInChildren<BattleUI> (true);

		effectsHolder = GetComponentInChildren<EffectsHolder> ();

		//deactive the storage so that stored units don't update or render
		reserveUnits.gameObject.SetActive(false);

		//deactivate the battleui until it is called for
		battleUI.gameObject.SetActive(false);
	}

	//sets the battle controller to use for this active battle controller
	public void SetBattle(BattleController newBattle) {
		battle = newBattle;
	}

	public void BattleStart() {
		//make sure battle ui is enabled (may have been disabled for another overlay)
		battleUI.gameObject.SetActive(true);

		//calculate the turn order and start the first turn
		StartNextTurn();
	}

	//check the current win conditions and if any are satisfied end the battle
	//return true if battle ends
	private bool CheckWinConditions() {
		//prioritise right units first so that the player (usually on left side) cant force a draw
		//check the set conditions first
		//if any of these are true then return true and use the win methods
		for (int i = 0; i < rightWinConditions.Count; i++) {
			if (rightWinConditions [i].CheckCondition (battle, false) == true) {
				//use the method
				rightVictory.VictoryPlay(battle, false);
				//return true
				return true;
			}
		}
		//next check the active left units to see if their dead
		if (CheckUnitsDead (activeUnits.LeftUnits.ToArray()) == true) {
			rightVictory.VictoryPlay(battle, false);
			return true;
		}

		for (int i = 0; i < leftWinConditions.Count; i++) {
			if (leftWinConditions [i].CheckCondition (battle, false) == true) {
				//use the method
				leftVictory.VictoryPlay(battle, true);
				//return true
				return true;
			}
		}
		//next check the active left units to see if their dead
		if (CheckUnitsDead (activeUnits.RightUnits.ToArray()) == true) {
			leftVictory.VictoryPlay(battle, true);
			return true;
		}

		return false;
	}

	//check if all the units are dead
	private bool CheckUnitsDead(UnitController[] units) {
		//check if any units are alive, return false if so
		for (int i = 0; i < units.Length; i++) {
			if (units [i].IsDead == false) {
				return false;
			}
		}

		return true;
	}

	//start the next turn
	private void StartNextTurn() {
		//before starting a turn check to see if any win conditions are satisfied
		if (CheckWinConditions () == false) {
			//only continue battle if no win conditions are satisfied

			//var to store the unit whose turn it is
			UnitController unitTurn = null;

			//find the unit whose turn is next and store the amout of time taken to reach it
			float timeTillNextTurn = FindNextUnitTurnAndTime (out unitTurn);

			//pass time on all active units equal to the time till the next turn
			PassUnitTime (timeTillNextTurn);

			//when starting a turn test if the unit whose turn it is is a player
			if (typeof(PartyUnit).IsAssignableFrom (unitTurn.GetType ())) {
				PartyUnit partyMemberTurn = (PartyUnit)unitTurn;

				//predict the next turns coming up, predict enough to fill out the ui
				//UnitController[] turnOrder = PredictFutureTurns (unitTurn, battleUI.GetTurnDisplayMax ());

				if (battleUI.PlayerTurnUI == false) {
					//if it is and the player ui is disabled fade it in
					battleUI.PlayerTurnStart (partyMemberTurn);//, turnOrder);
				} else {
					//if it is and the player ui was enabled for the last player then switch it
					battleUI.PlayerTurnStay (partyMemberTurn);//, turnOrder);
				}
			} else {
				if (battleUI.PlayerTurnUI == true) {
					//if it isn't and the ui was enabled then fade it out
					battleUI.PlayerTurnEnd ();
				}
			}

			//start the unit's turn
			unitTurn.StartTurn ();

		} else {
			//if the win condition is activated then disable the battle ui
			battleUI.gameObject.SetActive(false);
		}
	}

	//method called at the end of the current turn
	//must be public as this is called by the unit when its turn is finished
	//because this is called by the unit the unit doesn't need to be notified that its turn has ended
	public void EndCurrentTurn() {
		StartNextTurn();
	}

	//method to find the next unit whos turn it is
	//returns the time as a float and passes out the unit by reference
	private float FindNextUnitTurnAndTime(out UnitController unitTurn) {
		//work out which unit is next to go by iterating through all units and caching the unit with the lowest predicted time till next turn
		float lowestTime = 0;
		unitTurn = null;

		//iterate through left units first
		List<UnitController> leftUnits = activeUnits.LeftUnits;
		for (int i = 0; i < leftUnits.Count; i++) {
			//ignore any dead units
			if (leftUnits [i].IsDead == false) {
				//if uniturn is currently null then set this unit to be the one next one by default
				if (unitTurn == null) {
					unitTurn = leftUnits [i];
					lowestTime = leftUnits [i].TimeTillTurn ();
					//if the time till turn is infinite then reject this unit
					if (lowestTime < 0) {
						lowestTime = 0;
						unitTurn = null;
					}
				} else {
					//otherwise calculate the time of this unit and compare it with the current lowest
					float thisUnitTime = leftUnits [i].TimeTillTurn ();

					//negative time represents an infinite time, discard these results
					if (thisUnitTime < lowestTime && thisUnitTime >= 0) {
						lowestTime = thisUnitTime;
						unitTurn = leftUnits [i];
					}
				}
			}
		}

		//then iterate through right units
		List<UnitController> rightUnits = activeUnits.RightUnits;
		for (int i = 0; i < rightUnits.Count; i++) {
			//ignore any dead units
			if (rightUnits [i].IsDead == false) {
				if (unitTurn == null) {
					unitTurn = rightUnits [i];
					lowestTime = rightUnits [i].TimeTillTurn ();
					//if the time till turn is infinite then reject this unit
					if (lowestTime < 0) {
						lowestTime = 0;
						unitTurn = null;
					}
				} else {
					float thisUnitTime = rightUnits [i].TimeTillTurn ();

					if (thisUnitTime < lowestTime && thisUnitTime >= 0) {
						lowestTime = thisUnitTime;
						unitTurn = rightUnits [i];
					}
				}
			}
		}

		return lowestTime;
	}

	//method to pass time for all units
	private void PassUnitTime(float timeToPass) {
		//iterate through left units first
		List<UnitController> leftUnits = activeUnits.LeftUnits;
		for (int i = 0; i < leftUnits.Count; i++) {
			leftUnits [i].PassTime (timeToPass);
		}

		//then iterate through right units
		List<UnitController> rightUnits = activeUnits.RightUnits;
		for (int i = 0; i < rightUnits.Count; i++) {
			rightUnits [i].PassTime (timeToPass);
		}
	}

	//method to predict a given number of turns ahead of the current one and return them in an array in order
	//this simulated prediction must function identially to the findNextUnit and passUnitTime to be valid
	//also take in the current turns unit so that its first turn can be ignored, or else the current unit would show up at the start of the returned array
	public UnitController[] PredictFutureTurns(UnitController currentUnit, uint numberOfPredictions) {
		//set up an array of unit controllers equal to it
		UnitController[] turnOrder = new UnitController[numberOfPredictions];
		//set to true when current unit has been found once, after this any following turns by that unit will be counted
		bool currentUnitFound = false;

		//to start off the simulation copies must be made of all the active unit's current speed, and cooldown
		//and this must be paired with the unitcontroller it is related to
		//these copies must be stored in the order the system searches for the next turn (left units then right) for the prediction to be accurate

		//cache the left and right active units
		List<UnitController> leftUnits = activeUnits.LeftUnits;
		List<UnitController> rightUnits = activeUnits.RightUnits;

		//make an array of simulation units, make the array a list as dead units shouldnt be included in the simulation
		List<TurnSimulationUnit> simulatedUnits = new List<TurnSimulationUnit>();
		//and make the simulated units, starting with the left
		for (int i = 0; i < leftUnits.Count; i++) {
			if (leftUnits [i].IsDead == false) {
				simulatedUnits.Add(new TurnSimulationUnit (leftUnits [i]));
			}
		}
		for (int i = 0; i < rightUnits.Count; i++) {
			if (rightUnits [i].IsDead == false) {
				simulatedUnits.Add(new TurnSimulationUnit (rightUnits [i]));
			}
		}

		//now place the list in an array
		TurnSimulationUnit[] arrayUnits = simulatedUnits.ToArray();

		//now find as many turns into the future as there are predictions
		for (int i = 0; i < turnOrder.Length; i++) {
			//step forward in the simulation and find the next turn
			UnitController nextTurn = TurnSimulationStep (arrayUnits);

			//see if the current turn's unit has been found once yet
			//and then check if the next predicted unit matches it
			if (currentUnitFound == false && nextTurn == currentUnit) {
				//show the unit has been found and set the iteration back by 1, so that the next turn in the order will be recalculated
				currentUnitFound = true;
				i--;
			} else {
				//otherwise add the nextturn to the order
				turnOrder[i] = nextTurn;
			}
		}

		return turnOrder;
	}
	//takes a turn simulation and steps it forward by one, returning the unit whose turn it is next
	//if no units are given or all units are frozen and so wont ever get a turn then this will return null
	private UnitController TurnSimulationStep(TurnSimulationUnit[] simulationUnits) {
		//the current lowest time found
		float lowestTime = 0;
		//the simulated unit with the lowest time
		TurnSimulationUnit nextUnit = null;

		//iterate through the simulated units looking for the one with the lowest time
		for (int i = 0; i < simulationUnits.Length; i++) {
			//if no unit is set yet then this unit is the next one
			if (nextUnit == null) {
				nextUnit = simulationUnits [i];
				lowestTime = simulationUnits [i].PredictTimeToTurn ();
				//if the time till turn is infinite then reject this unit
				if (lowestTime < 0) {
					lowestTime = 0;
					nextUnit = null;
				}
			} else {
				//otherwise calculate the time of this unit and compare it with the current lowest
				float thisUnitTime = simulationUnits[i].PredictTimeToTurn();

				//negative time represents an infinite time, discard these results
				if (thisUnitTime < lowestTime && thisUnitTime >= 0) {
					lowestTime = thisUnitTime;
					nextUnit = simulationUnits [i];
				}
			}
		}
			
		//once the next unit has been found pass time for all units equal to the lowest time
		for (int i = 0; i < simulationUnits.Length; i++) {
			simulationUnits [i].PassTime (lowestTime);
		}

		//return the referenced next unit, if none were found return null
		if (nextUnit != null) {
			//as the simulated unit would have finished its turn after this its cooldown must be reset to 1
			nextUnit.turnCooldown = 1.0f;
			return nextUnit.simulatedUnit;
		} else {
			return null;
		}
	}

	//class used to predict the ordering of furture turns, holds a reference to a unit along with the speed and current simulated cooldown
	private class TurnSimulationUnit {
		public UnitController simulatedUnit = null;
		public float turnCooldown;
		public float unitSpeed;

		public TurnSimulationUnit(UnitController unitToSimulate) {
			simulatedUnit = unitToSimulate;
			turnCooldown = unitToSimulate.TurnCooldown;
			unitSpeed = unitToSimulate.Speed;
		}

		//pass the given amount of time for the simulated unit
		public void PassTime(float timeToPass) {
			//use the value return from the unit controller method to ensure accurate simulation
			//make sure to clamp the cooldown time to 0 or more
			turnCooldown = UnitController.CalculateTurnCooldown(turnCooldown, timeToPass, unitSpeed);
			if (turnCooldown < 0) {
				turnCooldown = 0;
			}
		}

		//predict the amount of time until this unit's simulated turn
		//use unit controller methods for accuracy, a -ve value represents infinite time (if unit is frozen)
		public float PredictTimeToTurn() {
			return UnitController.PredictTimeTillTurn(turnCooldown, unitSpeed);
		}
	}

	public void BattleEnd() {
		//remove all the temporary visual effects
		EffectsHolder.DestroyRemainingEffects();

		//clear the current saved win conditions, units, etc
		leftWinConditions.Clear();
		rightWinConditions.Clear();
		leftVictory = null;
		rightVictory = null;

		//clear all the stored units

		//clear any attached overlays (destroy them)
		for (int i = overlayUIHolder.transform.childCount - 1; i >= 0; i--) {
			Destroy(overlayUIHolder.transform.GetChild (i).gameObject);
		}

		//tell the main battle controller that the battle is over
		battle.EndBattle();
	}
}
