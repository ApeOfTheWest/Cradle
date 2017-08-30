using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Health {
	//maximum health must be at least 1
	//serialize the max so it can be modified in inspector (note: modifying it directly may cause the current resouce to exceed the max until updated)
	[SerializeField]
	private int maxHealth = 1;
	public int MaxHealth {
		get {
			return maxHealth;
		}
		set {
			//store the old val
			int oldValue = maxHealth;
			maxHealth = value;

			//if trying to set health to value less than 1 default to 1
			if (maxHealth < 1) {
				maxHealth = 1;
			}

			//if current health is more than the new max then lower it
			if (currentHealth > maxHealth) {
				currentHealth = maxHealth;
			}

			//if the health changed event is non null call it with the change in health
			if (MaxHealthChanged != null) {
				MaxHealthChanged (maxHealth - oldValue);
			}
		}
	}

	[SerializeField]
	private int currentHealth = 1;
	public int CurrentHealth {
		get {
			return currentHealth;
		}
		set {
			currentHealth = value;

			//if the value is greater than the max then clamp to max
			if (currentHealth > maxHealth) {
				currentHealth = maxHealth;
			} else if (currentHealth < 0) {
				//health cant be under 0
				currentHealth = 0;
			}
		}
	}

	public delegate void ChangeHealthEvent(int amountChange);

	//will be called whenever the max health is set to something different
	public event ChangeHealthEvent MaxHealthChanged;
	//will be called whenever the current health is changed but not set
	public event ChangeHealthEvent CurrentHealthChanged;

	public delegate void HealthStateEvent();

	//will be called when the health system reaches death point
	public event HealthStateEvent DieEvent;
	//will be called when the health system is revived from death
	public event HealthStateEvent ReviveEvent;

	//constructor should take a value for the max health, the system will then start with a maxed out health resource
	public Health() {

	}
	public Health(int max) {
		MaxHealth = max;

		MaxOut ();
	}

	//max out the health bar
	public void MaxOut() {
		CurrentHealth = maxHealth;
	}
	//change the health by the given amount
	//this can heal as well as damage
	public void ChangeHealth(int amount) {
		//cache whether unit was dead before change
		bool preDead = false;
		//set the amount changed to 0 if unit is currently dead (prevents non revival healing spells from reviving people)
		if (CurrentHealth <= 0) {
			amount = 0;
			preDead = true;
		}

		CurrentHealth += amount;

		//notify any listeners of the change
		if (CurrentHealthChanged != null) {
			CurrentHealthChanged(amount);
		}

		//if the new change in health has killed the unit then dispatch die event
		if (preDead == false && CurrentHealth <= 0 && DieEvent != null) {
			DieEvent ();
		}
	}

	//revive the health system with the given percentage, ensures at least 1 health is given no matter how low the percentage
	//does nothing if health system is not in dead state
	public void Revive(float revivePercent) {
		//check that sytem is dead before doing anything
		if (CurrentHealth <= 0) {
			//the health to restore with
			int reviveHealth = Mathf.RoundToInt(revivePercent * (float) MaxHealth);

			//ensure revive health is at least one
			if (reviveHealth <= 0) {
				reviveHealth = 1;
			}
			CurrentHealth = reviveHealth;

			//call revive event before showing the change in health
			if (ReviveEvent != null) {
				ReviveEvent ();
			}

			if (CurrentHealthChanged != null) {
				CurrentHealthChanged(reviveHealth);
			}
		}
	}
}
