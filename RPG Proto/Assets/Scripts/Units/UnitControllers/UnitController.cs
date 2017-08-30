using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the unit controller for a battle unit, must be attached to the relevant unit
//common class attached to all active battle units
public class UnitController : MonoBehaviour {
	//the attributes of the unit, along with the modifiers and modified attirbs
	//this is private as any modifiers to stats will be compiled automatically from equipment, talents, buffs / debuffs every time a change in modifiers is made
	[SerializeField]
	private UnitAttributeModifiers attributeAndModifiers = new UnitAttributeModifiers();

	//property used to get the modified (non base) stats
	public UnitAttributes Attributes {
		get {
			return attributeAndModifiers.modifiedAttributes;
		}
	}
	public UnitAttributes BaseAttribues {
		get {
			return attributeAndModifiers.baseAttributes;
		}
	}

	//the current statuses of the unit
	//this is public for access to status by ai but effects should only be modified using the public status changing methods
	public UnitStatuses unitStatus = new UnitStatuses();

	//NOTE: magick and health resources are set using the attributes that are calculated and as such they should not be modified directly
	//the magick resource (can be 0 for units that dont use it)
	[SerializeField]
	private Magick magick = new Magick();
	public Magick Magick {
		get { 
			return magick; 
		}
	}
	//the health resource
	[SerializeField]
	private Health health = new Health();
	public Health Health {
		get {
			return health;
		}
	}
	//method to test if unit is dead, check if health is 0
	public bool IsDead {
		get {
			if (health.CurrentHealth <= 0) {
				return true;
			} else {
				return false;
			}
		}
	}

	//the rally resource, this is often shared by an entire side during combat, however certain units may have rally seperate to the other units (like bosses)
	//to account for special behaviour regarding rally it is a virtual property
	//can also be null
	private Rally rally = null;
	public virtual Rally Rally {
		get {
			return rally;
		}
		set {
			rally = value;
		}
	}

	//the speed of this unit, is taken from the attributes and converted into a float from 0f to infinity, with infinity being
	public float Speed {
		get {
			return (float)Attributes.speed / 100f;
		}
	}

	//the current cooldown of the unit, this starts at 1 after each turn is taken and ticks down according to the speed
	//when the cooldown hits 0 it is this unit's turn
	private float turnCooldown = 1.0f;
	public virtual float TurnCooldown {
		get {
			return turnCooldown;
		}
		set {
			//cooldown can't be negative
			if (value >= 0) {
				turnCooldown = value;
			} else {
				turnCooldown = 0;
			}
		}
	}

	//the movement speed of this unit, this is used to determine how fast a character runs on the battlefield
	//it should be enclosed in a property so that the move speed can be adjusted based on slowing / speeding status effects
	private float moveSpeed = 5f;
	public float MoveSpeed {
		get { return moveSpeed; }
		set {
			moveSpeed = value;
		}
	}

	//the direction the unit is facing (either left or right)
	//this should be set to the opposite direction of the leftside variable normally, but may be inverted while moving around for an ability
	//all this does is flip the unit on the x axis to visually flip all effects
	private bool rightFacing = true;
	public bool RightFacing {
		get { return rightFacing; }
		set {
			rightFacing = value;
			//invert x scale based on the value passed

			//if right facing is true then set the scale of the unit to be 1
			if (rightFacing == true) {
				transform.localScale = new Vector3 (1, 1, 1);
			} else {
				transform.localScale = new Vector3 (-1, 1, 1);
			}
		}
	}

	//the reward for killing this unit
	private BattleReward bounty;

	//reference to the battle controller
	protected BattleController battle;
	//provide a reference so that abilities can access the controller
	public BattleController Battle {
		get {
			return battle;
		}
	}

	//the 'brain' that selects what ability to use and on what target
	//this can be represented by a human or individual ai or group ai controllor
	[SerializeField]
	protected TurnBrain turnBrain;

	//the normalized home position of this unit
	public Vector2 normHomePosition = new Vector2();

	//the actual home position of this unit, depends on the battle scene its placed in
	//this is the position of the units feet not their centre of mass, and so attacks shouldn't usually be directed at this
	public Vector2 homePosition;

	//the gameobject that marks the point on this unit that melee attacks should be aimed at, if null this units position will be returned
	private MeleePoint meleePoint;
	//the melee point, it is a vector 3 so that depth can be given, this depth is the depth that the attacker should be above to be visible (for hitspark effects)
	public Vector3 MeleePoint {
		get {
			if (meleePoint != null) {
				return meleePoint.GetPoint();
			} else {
				return transform.position;
			}
		}
	}
	//the point on this unit that ranged attacks should be aimed at, such as guns and spells
	private RangedPoint rangedPoint;
	public Vector3 RangedPoint {
		get {
			if (rangedPoint != null) {
				return rangedPoint.GetPoint();
			} else {
				return transform.position;
			}
		}
	}
	//the point on this unit where health bars and status icons should be displayed along with damage / heal messages
	private StatusDisplayPoint statusDisplayPoint;
	public Vector3 StatusDisplayPoint {
		get {
			if (statusDisplayPoint != null) {
				return statusDisplayPoint.GetPoint ();
			} else {
				return transform.position;
			}
		}
	}
	//the status display linked to this unit, this will be added when the unit is made active and removed when inactive
	//this automatically displays health when hit or targeted, and status ailments
	//but it should also be used to display messages such as when an attack misses, or is evaded
	public UnitStatusDisplay statusDisplay = null;

	//the default idle animation string, to use the in built idle method this string should be used in the animator
	public const string DEFAULT_IDLE_STRING = "Idle";
	private static int defaultIdleHash = Animator.StringToHash (DEFAULT_IDLE_STRING); 
	public static int DefaultIdleHash {
		get {
			return defaultIdleHash;
		}
	}
	public const string DEFAULT_RUN_STRING = "Running";
	private static int defaultRunHash = Animator.StringToHash (DEFAULT_RUN_STRING); 
	public static int DefaultRunHash {
		get {
			return defaultRunHash;
		}
	}
	public const string DEFAULT_HURT_STRING = "Hurt";
	private static int defaultHurtHash = Animator.StringToHash (DEFAULT_HURT_STRING); 
	public static int DefaultHurtHash {
		get {
			return defaultHurtHash;
		}
	}
	public const string DEFAULT_DIE_STRING = "Die";
	private static int defaultDieHash = Animator.StringToHash (DEFAULT_DIE_STRING); 
	public static int DefaultDieHash {
		get {
			return defaultDieHash;
		}
	}
	public const string DEFAULT_REVIVE_STRING = "Revive";
	private static int defaultReviveHash = Animator.StringToHash (DEFAULT_REVIVE_STRING); 
	public static int DefaultReviveHash {
		get {
			return defaultReviveHash;
		}
	}

	//which side the unit is on
	private bool leftSide = true;
	public bool LeftSide {
		get {return leftSide; }
		set {
			leftSide = value;
			//set right facing to the same value by default
			RightFacing = leftSide;
		}
	}

	//the sprite to use as a portrait for the unit in the turn order display, should be the same res as the display portraits to prevent blur
	public Sprite turnOrderPortrait;

	//whether the unit is using an ability or not
	//the unit should not react to taking damage while using an ability as this can knock the unit out of an ability animation and interrupt it
	//if the unit should react to damage mid ability then this can be re-enabled early
	private bool usingAbility = false;
	public bool UsingAbility {
		get { return usingAbility; }
		set {
			usingAbility = value;
		}
	}

	//bool showing whether the units turn is ending, this is required as some abilities are free and don't cost a turn
	//if an ability wants to be free then it should set this to false as part of the ability cycle, this allows free moves in certain conditions
	public bool endTurnAfterNextAbility = true;

	//events for the various actions that a unit controller may produce callbacks on

	//event system for ability hits given by the animation subsystem to sync up ability damage with animations
	public delegate void AbilityHit();
	public event AbilityHit abilityHitEvent;

	//every unit must have an animator that can be called on by abilities and give callbacks to the ability system
	//(but some specialty units may have multiple animators, or even null animators)
	protected Animator animator = null;
	public Animator Animator {
		get {
			return animator;
		}
	}
	//every unit must have some sort of collider that can be used for selection the units and keeping them in the camera view
	//the unit should also have a rigidbody so the collider isnt marked as static
	protected Collider2D unitBounds;

	//every unit must have an attached audiosource that can be used by abilities to play sound effects
	protected AudioSource audioSource;
	public AudioSource AudioSource {
		get {
			return audioSource;
		}
	}

	//just about every unit will need some method to play footstep audio, place an array of clips to choose from here
	//these can be changed out for different terrains if necessary
	public AudioClip[] footstepSounds = null;

	//default methods called on every ability when waking
	//make protected so it can be used when derived
	protected void Awake() {
		//look for an animator at the top level
		animator = GetComponent<Animator> ();
		//look for a collider that determines the bounds of the unit
		unitBounds = GetComponent<Collider2D>();
		//look for a meleepoint and ranged point
		meleePoint = GetComponentInChildren<MeleePoint>();
		rangedPoint = GetComponentInChildren<RangedPoint> ();
		statusDisplayPoint = GetComponentInChildren<StatusDisplayPoint> ();
		audioSource = GetComponent<AudioSource> ();

		//listen for die and revive events from health system
		Health.DieEvent += Die;
		Health.ReviveEvent += Revive;

		//IMPORTANT: units should use an instance of thei turn brain rather than modify the origional
		//this will prevent unwanted carryovers between loading
		turnBrain = Instantiate(turnBrain);
	}

	//on destroy unsub from die and revive events
	void OnDestroy() {
		Health.DieEvent -= Die;
		Health.ReviveEvent -= Revive;
	}

	//call when this unit is hit with a hit packet, this will first check if the hit connects, and then this will apply damage
	//and apply statuses if any are included
	//returns true if the attack connects
	public bool ApplyHit(AbilityHitPacket hitPacket, UnitController sourceOfHit) {
		//first test if the ability even hits the target, instantly return false if it misses
		//check for unevadable hits and hits that can't be missed
		if (hitPacket.missable == true) {
			//first get the hit chance from a combination of the ability accuaracy and the unit's accuracy
			int hitChance = hitPacket.accuracy + (sourceOfHit.Attributes.accuracy - 100);

			//now find a random number between 0 and 100
			int hitRand = Random.Range(0, 100);
			//if the number is greater than the hit chance then the attack misses
			if (hitRand > hitChance) {
				//attack missed code
				statusDisplay.QueDisplayMessage("miss", Color.white);
				return false;
			} else if (hitPacket.evadable == true) {
				//if the attack didn't miss then it can still be evaded if the hit is marked as evadable
				//add the evasion to the random number
				hitRand += Attributes.dodgeChance;

				//now if the random number is greater the attack is dodged
				if (hitRand > hitChance) {
					//attack evaded code
					statusDisplay.QueDisplayMessage("dodged", Color.white);
					return false;
				}
			}
		}

		//by this point the hit must not have missed, so go into damage calculation

		//perform damage calculation steps if the hitpacket is damaging
		if (hitPacket.damage.IsDamaging ()) {
			//if this unit is classified as weak to the element then change the status display to show the damage done is done by an attack it is weak to
			if (CheckWeak (hitPacket)) {
				//change the statusdisplay state

				//add the broken status effect if not afflicted already
			}

			//the damage after applying damage calculations (can be 0)
			int finalDamage = CalculateDamageTaken(hitPacket, sourceOfHit);

			//pass the damage onto the health system, make sure to invert as it is taken away by default
			Health.ChangeHealth(-finalDamage);

			//change the status display state back to normal for future messages

			//if the hit ability does any damage at all (after damage calculations) then play the hurt animation
			//only do this if using ability is false so as not to interupt the ability animation
			//dont play hurt animation if unit is dead
			if (finalDamage > 0 && usingAbility == false && IsDead == false) {
				PlayHurtAnimation ();
			}
		} 

		//finally apply any status changes
		for (int i = 0; i < hitPacket.additionalEffects.Count; i++) {
			hitPacket.additionalEffects [i].ChangeStatus (this, sourceOfHit);
		}

		//if this point is reached the attack must have landed
		return true;
	}

	//check if this unit is classified as weak to the given hit packet
	public bool CheckWeak(AbilityHitPacket hitPacket) {
		//if the ability has an element then compare the element
		if (hitPacket.element != null) {
			bool absorb;
			int resistance;
			hitPacket.element.GetUnitResistanceToElement (this, out absorb, out resistance);

			if (absorb == false && resistance <= UnitAttributes.ELEMENT_WEAKNESS_THRESHOLD) {
				//if both these conditions are satisfied then the unit is weak to the hit
				return true;
			}
		}

		//otherwise return false
		return false;
	}

	//method with which to do all unit based damage calculations, take a hitpacket and apply damage (or healing) based on stats and resistances
	public int CalculateDamageTaken(AbilityHitPacket rawHit, UnitController sourceOfHit) {
		//final damage as a float before converting to int, get the raw damage from the damage var, passing in the user
		float finalDamage = (float) rawHit.damage.CalculateDamage (sourceOfHit);

		//first up check if resistances should apply at all
		if (rawHit.ignoreResistance == false) {
			//first apply raw damage reduction based on the type of damage
			int damageReduction = 0;
			if (rawHit.hitType == AbilityHitPacket.HitType.Physical) {
				damageReduction = Attributes.toughness;
			} else if (rawHit.hitType == AbilityHitPacket.HitType.Magical) {
				damageReduction = Attributes.will;
			}

			//ensure that the damage reduction doesn't take away more than the magnitude of the damage inflicted
			int magnitudeBeforeReduction = Mathf.Abs((int)finalDamage);
			if (magnitudeBeforeReduction < damageReduction) {
				damageReduction = magnitudeBeforeReduction;
			}

			//reduce the damage in a sense that decreases the magnitude (so depends on polarity)
			if (finalDamage >= 0) {
				finalDamage -= (float)damageReduction;
			} else {
				finalDamage += (float)damageReduction;
			}

			//next apply any elemental restances if the attack has an element
			Element hitElement = rawHit.element;
			if (hitElement != null) {
				bool absorbElement;
				int elementResistance;

				hitElement.GetUnitResistanceToElement (this, out absorbElement, out elementResistance);

				//if absorb element is true then set element resistence to 200% if the damage is positive (this will flip the damage sign)
				if (finalDamage > 0 && absorbElement == true) {
					elementResistance = 200;
				} else if (elementResistance > 100) {
					//if absorption rules are not in effect then then resistence is not allowed to be higher than 100 (which would negate all damage)
					elementResistance = 100;
				}

				//convert the resistance into a milltiplier to apply on the damage
				float elementDamageMultiplier = 1f - ((float) elementResistance / 100f);

				finalDamage *= elementDamageMultiplier;
			}

			//lastly apply any necessary protection reduction
			float protectionPercent = (float)Attributes.protection;
			//clamp protections effect to negating 100% of damage
			if (protectionPercent > 1f) {
				protectionPercent = 1f;
			}

			finalDamage *= 1f - protectionPercent;
		}

		//casting to int here will floor the floating point result (round down)
		return (int)finalDamage;
	}

	//call while moving this unit to a position, this will move the unit at its speed towards the position without changing the current animation (so set it before hand)
	//returns true when the location is reached
	//will also flip the direction the unit is facing to face towards the destination
	public bool MoveToPosition(Vector2 destination) {
		//get the vector from the current position to the destination
		Vector2 directionVector = destination - (Vector2)transform.position;

		//change direction, dont change direction if the x component is 0
		if (directionVector.x > 0) {
			RightFacing = true;
		} else if (directionVector.x < 0) {
			RightFacing = false;
		}

		//get the length of the vector
		float distance = directionVector.magnitude;

		//get the distance to travel based on the speed and the time
		float distanceToTravel = moveSpeed * Time.deltaTime;

		//check if the distance to travel is greater than or equal to the current distance
		if (distanceToTravel >= distance) {
			//if so set the position of the controller to the target distance
			transform.position = destination;

			//return true to show the destination has been reached
			return true;
		} else {
			//normalise the direction (using the pre-calculated distance to save operations)
			directionVector /= distance;

			//multiply the direction normalised with the distance to travel
			directionVector *= distanceToTravel;

			//translate by the given direction
			transform.position += new Vector3(directionVector.x, directionVector.y);

			//return false, the destination is not yet reached
			return false;
		}
	}

	//method that sets the unit back into its idle animation
	public virtual void SetIdle() {
		//by defualt just use the default hash on the animator
		Animator.Play(DefaultIdleHash);
	}

	//method to set the unit into its running animation, this should work from any state as this should cancel other animations
	public virtual void SetRunning() {
		//by defualt just use the default hash on the animator
		Animator.Play(DefaultRunHash);
	}

	//method to be called when the unit is killed
	public virtual void Die() {
		//call the death animation only if ability play is not in effect
		//otherwise system will softlock
		if (usingAbility == false) {
			Animator.Play(DefaultDieHash);
		}
		//no flag needs to be set because the curhealth being at 0 acts as a flag
	}

	//method to be called when the unit is revived
	public virtual void Revive() {
		//call the revive animation
		Animator.Play(DefaultReviveHash);
	}

	//method to play the hurt animation, included publicly for cutscene purposes
	//the hurt animation should automatically reset into the idle state when finished
	public virtual void PlayHurtAnimation() {
		Animator.Play (DefaultHurtHash);
	}

	//often called at the end of a melee ability, returns the unit to its home position and calls the end of the current ability
	public void ReturnHomeAndEndAbility() {
		StartCoroutine (ReturnAndEndAbility());
	}

	//coroutine that returns the user home and ends the current ability cycle
	private IEnumerator ReturnAndEndAbility() {
		while (MoveToPosition (homePosition) == false) {
			yield return null;
		}
			
		//set the animation to idle
		SetIdle();
		//face the unit the right way
		RightFacing = LeftSide;

		//notify the unit that the ability is stopped
		AbilityStopped ();
	}

	//called when someone else has a turn to lower this unit's turncooldown by the amount of time spent leading up to their turn
	//this simulates the passage of time in a battle
	public void PassTime(float timePassed) {
		//dont pass time if this unit is dead
		if (IsDead == false) {
			TurnCooldown = CalculateTurnCooldown (TurnCooldown, timePassed, Speed);
		}
	}
	//static method that calculates the cooldown of a unit with a given speed after so much time has passed
	//can return a negative number if the turn would have been passed
	//this same method is used between the unit's internal system and the turn prediction system to ensure both are equal
	public static float CalculateTurnCooldown(float initialCooldown, float timePassed, float unitSpeed) {
		//lower the cooldown by the time multiplied by the speed
		//when the cooldown hits 0 it is this units turn
		return initialCooldown - timePassed * unitSpeed;
	}

	//use to predict the amount of time until this units next turn, this prediction is used to find whose turn it is next
	//a negative value indicates infinity (from a currently frozen unit)
	public float TimeTillTurn() {
		return PredictTimeTillTurn (TurnCooldown, Speed);
	}
	//static method that takes a cooldown, a speed and calculates the amount of time till the next turn
	//negative numbers represent infinite time
	public static float PredictTimeTillTurn(float unitTurnCooldown, float unitSpeed) {
		//include special case for when speed is 0 (frozen)
		if (unitSpeed != 0) {
			return unitTurnCooldown / unitSpeed;
		} else {
			return -1;
		}
	}

	//method used to tell the player to go back home
	//usually after a move or being knocked out of position

	//method called at the start of a battle this unit is participating in
	//sets up the battle controller reference and gives the unit a chance to initialise stuff
	//there might be an intro before the battle starts so dont play sounds or animations, just set stuff up
	public virtual void InitialiseBattle(BattleController setBattle) {
		battle = setBattle;

		//rebuild all the modifers applied to the unit and calculate the modified attributes from there
		RecalculateModifiersAndAttributes ();

		//by default max out the health and mana resources when starting the battle
		magick.MaxOut();
		health.MaxOut ();
	}

	//method called when a unit is made active in a battle, will be called at the start of the battle if the unit is active then
	//if a unit wants to reference other units then it should grab a reference here, keep in mind it may have to watch for other units being activated
	//and current units being de-activated, if it wants to maintain a current reference list
	public virtual void UnitMadeActive() {
		//use the battle ui to give this unit a status display bar
		battle.ActiveBattle.battleUI.AddUnitStatusDisplay(this);
	}
	//method called when a unit is made inactive in a battle
	public virtual void UnitMadeInactive() {
		//remove the status display bar
		battle.ActiveBattle.battleUI.RemoveUnitStatusDisplay(this);
	}

	//method called to play the units intro animation at the start of the battle
	//must tell the battle controller when the intro is done or else control will never be given back and the battle will hang
	public virtual void PlayUnitIntro() {
		//by default just tell the controller that this units intro is done
		battle.UnitIntroFinished(this);
	}

	//common method used by most unit controllers
	//often called by the animation subststem when the intro animation is finished
	public virtual void IntroAnimationFinished() {
		//by default just tell the controller that this units intro is done
		battle.UnitIntroFinished(this);
	}

	//method called when it is the unit's turn, typically the unit will use an ability and then pass back control to the battle controller
	public virtual void StartTurn() {
		//increase the rally of the unit by the passive rally generation
		Rally.ChangeRallyProgress(Attributes.rallyGeneration);

		//lower the cooldown of all abilities by 1 (can lower by more under special statuses)
		turnBrain.CooldownAbilities(1);
		/*for (int i = 0; i < abilities.Count; i++) {
			abilities [i].Cooldown (1);
		}*/
			
		SelectAbilityAndTarget();
	}
	//method called to end this units turn
	//gives control back to the battle controller
	public virtual void EndTurn() {
		//increase this units cooldown by 1 by default
		//increment rather then setting to allow for a larger/smaller cooldown if the unit desires it
		TurnCooldown += 1.0f;

		battle.EndCurrentTurn ();
	}

	//used to pick out an ability to be used along using the unit's turn brain, the units brain may be a human or a computer one
	//the turn brain may take more than one frame to choose an ability and target (in the case of a human picking from a menu for example)
	//so a callback function for starting the ability must be used
	private void SelectAbilityAndTarget() {
		//set the ability started function
		AbilityStarted();

		//use the turn brain to pick out which ability to use and start it
		turnBrain.ChooseAndUseAbility(this, battle.ActiveBattle.ActiveUnits.LeftUnits.ToArray(), 
			battle.ActiveBattle.ActiveUnits.RightUnits.ToArray(), 
			battle.ActiveBattle.ReserveUnits.LeftUnits.ToArray(), battle.ActiveBattle.ReserveUnits.RightUnits.ToArray());

		//check if any ability exists
		/*if (abilities.Count > 0) {
			//use the assumed single ability on the enemy
			UnitController target;
			if (leftSide == true) {
				target = battle.ActiveBattle.ActiveUnits.RightUnits [0];
			} else {
				target = battle.ActiveBattle.ActiveUnits.LeftUnits [0];
			}

			((SingleAbility)abilities [0].ability).UseAbility (this, target);
		} else {
			//otherwise end turn
			AbilityStopped();
		}*/
	}

	//called when an ability is starting to be used
	//take in whether the ability is a reaction ability or not so that people can prepare to react
	//tell this unit that it is in the middle of an ability so that no animations will be auto played that might break the ability animation
	private void AbilityStarted() {
		UsingAbility = true;
		//unless the ability turns this to false the turn will end after this ability is used
		endTurnAfterNextAbility = true;
	}

	//called when the last ability used is finished
	//places the units back at home if any were moved
	public void AbilityStopped() {
		UsingAbility = false;
		//clear any listeners on the ability hit event
		abilityHitEvent = null;

		//unit can't die in the middle of using an ability even if it reaches 0 health because of it, so check for death here
		//if unit dies here then endturnAfterNextAbility must be true
		if (health.CurrentHealth <= 0) {
			Die ();
			endTurnAfterNextAbility = true;
		}

		//end the turn if the ability was not free
		if (endTurnAfterNextAbility == true) {
			EndTurn();
		} else {
			//choose another ability
			SelectAbilityAndTarget();
		}
			
	}

	//a genric callback function from the animation subsystem, used to sync the dealing of damage /effects with the animation being played
	public void AbilityAnimationHit() {
		//activate callbacks on any event listeners (likely any active ability object)
		if (abilityHitEvent != null) {
			//call the event
			abilityHitEvent ();
		}
	}
		
	//called at the start of the battle, should also be called when a modifier is taken away to prevent a light error introduced from adding and taking away repeatedly
	public void RecalculateModifiersAndAttributes() {
		//first off reset the modifiers to their defaults
		attributeAndModifiers.ResetModifers();

		//then apply the effects of every possible source for a modifier to the modifiers
		//for most units this is just buffs / debuffs
		//players may have equipment and talents


		//and finally recalculate the modified attributes
		RecalculateAttributes();
	}

	//should be called when a modifier has been added, doesn't recalculate the modifiers, but does recalculate the changed attributes
	public void RecalculateAttributes() {
		attributeAndModifiers.UpdateModifiedAttributes ();

		//change the max health and mana based on changes from the modified attributes (don't max them out, keep the current wounds)
		health.MaxHealth = Attributes.maxHealth;
		magick.MaxMagick = Attributes.maxMagick;
	}

	//called by animator whenever footstep sound effect is to be played
	public void FootStep() {
		//make sure the footstep clip array is not null or 0
		if (footstepSounds != null && footstepSounds.Length > 0) {
			//choose a random sound from the array
			int clip = Random.Range(0, footstepSounds.Length);
			//use play oneshot
			audioSource.PlayOneShot(footstepSounds[clip]);
		}
	}
}
