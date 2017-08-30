using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour {
	//the button ui component
	private Button button;
	public Button Button {
		get {
			return button;
		}
	}

	//the text that holds the ability name
	[SerializeField]
	private Text abilityNameLabel;

	//the enableable requirement objects
	[SerializeField]
	private AbilityRequirement magickReq;
	[SerializeField]
	private AbilityRequirement healthReq;
	[SerializeField]
	private AbilityRequirement cooldownReq;
	[SerializeField]
	private AbilityRequirement rallyReq;
	[SerializeField]
	private AbilityRequirement rallyCost;

	//the overlay object holder, to be displayed when an ability is disabled for whatever reason
	[SerializeField]
	private GameObject overlayHolder;
	//the fillable overlay image
	[SerializeField]
	private Image overlayFill;
	//the text indicating the cooldown remaining (if any)
	[SerializeField]
	private Text cooldownText;

	//the ability being referred to
	private AbilitySlot ability = null;
	public AbilitySlot Ability {
		get {
			return ability;
		}
	}

	// Use this for initialization
	void Awake () {
		button = GetComponent<Button> ();
	}

	//use to set the reference of the ability button and update its displays, set null to clear it
	public void SetAbility(AbilitySlot newAbility) {
		ability = newAbility;

		if (ability != null) {
			//set the visible components if ability is non null
			//first set the properties based on the ability slot properties
			abilityNameLabel.text = ability.ability.abilityName;

			//set the requirements, disable them if not required
			SetRequirementState(magickReq, ability.magickCost);
			SetRequirementState(healthReq, ability.healthCost);
			SetRequirementState(cooldownReq, ability.abilityCooldown);
			SetRequirementState(rallyReq, ability.rallyRequired);
			SetRequirementState(rallyCost, ability.rallyExpended);

			//enable the overlay if the ability is not currently usable
			if (ability.usable == false) {
				overlayHolder.SetActive (true);

				//if the ability is on cooldown then adjust the fill amount and display the cooldown text
				if (ability.CooldownLeft > 0) {
					float cooldownPercent = (float)ability.CooldownLeft / (float)ability.abilityCooldown;

					overlayFill.fillAmount = cooldownPercent;
					cooldownText.gameObject.SetActive (true);
					cooldownText.text = ability.CooldownLeft.ToString ();
				} else {
					//otherwise set the fill to full
					overlayFill.fillAmount = 1f;
					//and disable the text
					cooldownText.gameObject.SetActive(false);
				}
			} else {
				//otherwise disable it
				overlayHolder.SetActive(false);
			}
		}
	}

	//method to set a reuirement's state based on a costing integer
	private void SetRequirementState(AbilityRequirement requirement, int cost) {
		if (cost > 0) {
			requirement.gameObject.SetActive (true);
			requirement.SetRequirementText (cost.ToString ());
		} else {
			requirement.gameObject.SetActive (false);
		}
	}
}
