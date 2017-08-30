using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//the unit status display is a display interface that shows the player's health, damage taken, status ailments
//and generic messages from the unit
public class UnitStatusDisplay : MonoBehaviour{
	//the time to keep the display up for after it is woken up before deativating it again
	//(if it is being held active or there are still messages qued it wont dissapear)
	private bool heldActive = false;
	public bool HeldActive {
		get {
			return heldActive;
		}
		set {
			heldActive = value;

			if (heldActive == true) {
				WakeUpDisplay ();
			}
		}
	}

	//the rect transform of this ui element
	private RectTransform rectTransform;

	//how long to stay active before dissapearing
	public const float maxActiveTime = 2f;
	//the current time active, after theres nothing else to display
	private float timeActive = 0f;

	//the current message que
	private Queue<MessageData> messageQue = new Queue<MessageData>();
	//the textbox prefab to use for all the messages
	//must be colourable so healing can be coloured different to damage
	[SerializeField]
	private StatusDisplayMessage messageText;

	//the time between displaying messages in seconds
	public const float messageSpacing = 0.5f;
	//the current cooldown before the next message can be released
	private float nextMessageCooldown = 0f;

	//the gameobject to attach messages to
	private GameObject messageHolder;
	//the image representing the health bar
	private Image healthBarImage;
	//the gameobject to attack status icons to
	private GameObject statusListHolder;

	//the camera rendering this display, used to convert world space to screen space
	public Camera displayRenderCamera = null;

	//the unit that this status bar is attached to
	private UnitController attachedUnit = null;
	public UnitController AttachedUnit {
		get {
			return attachedUnit;
		}
	}

	//class to hold all the infor needed for a message before it is displayed
	private class MessageData {
		public MessageData(string message, Color colour) {
			this.message = message;
			this.colour = colour;
		}

		public string message = "";
		public Color colour = Color.white;
	}

	void Awake() {
		messageHolder = transform.Find ("MessageHolder").gameObject;
		healthBarImage = transform.Find ("HealthAndStatus/HealthBar/BarFill").GetComponent<Image>();
		statusListHolder = transform.Find ("HealthAndStatus/StatusList").gameObject;
		rectTransform = GetComponent<RectTransform> ();
	}

	//attach this display to a unit, can be null if detaching from a unit
	public void AttachToUnit(UnitController unit) {
		//if unit is currently non null then unsubscribe from certain event listeners
		ListenerCleanup();

		attachedUnit = unit;
		//tell the unit that this is its status display
		attachedUnit.statusDisplay = this;

		//listen for changes in health events
		attachedUnit.Health.MaxHealthChanged += ChangeMaxHealth;
		attachedUnit.Health.CurrentHealthChanged += ChangeHealth;
		//listen for changes in status events
		attachedUnit.unitStatus.AddStatus += StatusAdded;
		attachedUnit.unitStatus.RemoveStatus += StatusRemoved;

		//redraw the healthbar
		RedrawHealthBar();
	}

	//ques a message to be displayed, the messeage will be added to a que of messages and displayed when the que clears
	public void QueDisplayMessage(string message, Color messageColor) {
		messageQue.Enqueue (new MessageData (message, messageColor));

		//whenever a message is added to the que tell this display to wake itself up
		WakeUpDisplay();
	}

	//call when the current health has been changed by a certain amount
	//display a message and update the health bar
	public void ChangeHealth(int amountChanged) {
		//display a message of the change, colour the message based on whether the change is positive or negative
		Color messageColor;
		if (amountChanged > 0) {
			messageColor = Color.green;
		} else {
			messageColor = Color.white;
		}
		//set the amount changed to an absolute before displaying it
		amountChanged = Mathf.Abs(amountChanged);
		QueDisplayMessage (amountChanged.ToString(), messageColor);

		//redraw the health bar after
		RedrawHealthBar();
	}

	//call when max health is changed
	public void ChangeMaxHealth(int amountChanged) {
		//display a message of the change
		Color messageColor;
		string message;
		if (amountChanged > 0) {
			messageColor = Color.green;
			message = "+" + amountChanged.ToString();
		} else {
			messageColor = Color.white;
			message = "-" + amountChanged.ToString();
		}
		QueDisplayMessage (message + " Max Health", messageColor);

		//redraw the health bar after
		RedrawHealthBar();
	}

	//call when the health of the current unit is changed (whether it is max or current doesn't matter as they both required a change in graphic)
	public void RedrawHealthBar() {
		//don't necessarily wake up the health bar when this happens
		//get the health of the unit
		Health health = attachedUnit.Health;

		//calculate the percentage of the max health there is
		float healthPercent = (float)health.CurrentHealth / (float)health.MaxHealth;
		//apply this to the fill percent of the image
		healthBarImage.fillAmount = healthPercent;
	}

	//call when the status of the current unit is changed 
	//redo all the status icons based on the new status
	//also display a message showing how the status has changed
	public void StatusAdded(StatusEffect addedStatus, bool buff) {

	}

	public void StatusRemoved(StatusEffect removedStatus, bool buff) {

	}

	//wake up the display, display will fade after a set amount of time after all qued messages have dissapeared
	public void WakeUpDisplay() {
		//set the time active to 0
		timeActive = 0f;
		//if the display was not yet active reset the message cooldown to 0 on wakeup
		if (gameObject.activeSelf == false) {
			nextMessageCooldown = 0f;
		}

		//make the display active
		gameObject.SetActive(true);
	}

	//in on pre cull place the status display above the unit's head in screenspace
	public void CameraPreCull() {
		//note: the camera will move itself in lateupdate so this has to be called by a camera script
		//first get the point of the status display for the unit in worldspace
		Vector3 displayPoint = attachedUnit.StatusDisplayPoint;
		//now convert it into display space using the camera
		Vector3 screenPos = displayRenderCamera.WorldToScreenPoint(displayPoint);
		transform.position = screenPos;
	}

	//push out a new message if enough time has passed scince the last one
	void Update() {
		//if theres no messages in the que then just lower the cooldown to 0
		if (messageQue.Count == 0) {
			//lower the cooldown
			nextMessageCooldown -= Time.deltaTime;

			if (nextMessageCooldown < 0) {
				nextMessageCooldown = 0f;
			}
		} else {
			//if theres messages in waiting then account for overshoot and manually update the message by the overshoot

			//but first check if the cooldown is 0, if so then produce message as if no time has passed as it is the front of the chain
			if (nextMessageCooldown == 0) {
				DispatchMessage ();
			} else {
				//still recovering from prev message, lower cooldown by time to maintain constant spacing

				//lower the cooldown
				nextMessageCooldown -= Time.deltaTime;

				//now while there are still messages and the cooldown is still negative or zero dispatch more messages
				while (messageQue.Count > 0 && nextMessageCooldown <= 0) {
					//dispatch another message
					DispatchMessage();
				}
			}
		}

		//update the active time if heldactive is false and theres no qued messages
		if (heldActive == false && messageQue.Count == 0) {
			timeActive += Time.deltaTime;

			//if max time is reach disable the status bar
			if (timeActive >= maxActiveTime) {
				gameObject.SetActive (false);
			}
		}
	}

	//method to use whenever a message is dispatched
	private void DispatchMessage() {
		//ensure there is a qued message
		if (messageQue.Count == 0) {
			return;
		} 

		//get the next message from the que
		MessageData message = messageQue.Dequeue();

		//display the new message by intialising it as a child of the message holder gameobject on this display
		StatusDisplayMessage newMessage = Instantiate(messageText, messageHolder.transform);
		//set the text and colour of the message based on the next in que
		Text text = newMessage.MessageText;
		text.text = message.message;
		text.color = message.colour;

		//update the created message manually by the time negative magnitude of the cooldown
		//(negative cooldown shows a message that is created between last frame and this one)
		if (nextMessageCooldown < 0) {
			float msgUpdateTime = Mathf.Abs (nextMessageCooldown);
			//update the message
			newMessage.ManualUpdate(msgUpdateTime);
		}

		//incriment the cooldown based on spacing settings
		nextMessageCooldown += messageSpacing;
	}

	//unsubscribe from any necessary event listeners
	private void ListenerCleanup() {
		if (attachedUnit != null) {
			//tell the prev unit that this is not its status display anymore
			attachedUnit.statusDisplay = null;

			attachedUnit.Health.MaxHealthChanged -= ChangeMaxHealth;
			attachedUnit.Health.CurrentHealthChanged -= ChangeHealth;
			attachedUnit.unitStatus.AddStatus -= StatusAdded;
			attachedUnit.unitStatus.RemoveStatus -= StatusRemoved;
		}
	}

	//on destroy make sure to cleanup event listeners
	void OnDestroy() {
		ListenerCleanup ();
	}
}
