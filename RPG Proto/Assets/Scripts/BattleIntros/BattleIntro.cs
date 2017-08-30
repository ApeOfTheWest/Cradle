using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//battle intro is a class made to be attached to a battle intro type gameobject, 
//the gameobject will start the battle and then call on the battle start function of the battle controller once it is done, it will then be destroyed by the controller
//this can be used to create custom intros into boss battles for example
public abstract class BattleIntro : MonoBehaviour {

	private BattleController controller = null;

	// Use this for initialization
	void Start () {
		
	}

	//starts the intro, typically the intro will enable itself and play itself
	//then the intro will call the battle start function of the battle controller, this will then cause the controller to destroy it and start the battle
	//if the intro never calls battle start then the battle will never begin and the game will softlock
	//an intro holder object is also provided, this will give the intro a place to store objects that should be visible and a camera to use
	//any objects left in this holder will be destroyed when intro finish is called
	public void StartIntro(BattleController control, IntroHolder holder) {
		//grab reference to the controller so it can be adjusted by the intro if desired
		controller = control;
	}

	//stops the intro
	private void StopIntro() {
		//tell the controlller the intro is finished
		controller.BattleIntroFinished();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
