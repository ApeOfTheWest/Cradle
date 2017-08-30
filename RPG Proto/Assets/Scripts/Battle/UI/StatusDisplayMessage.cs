using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//a status display message should be able to display a message on text along with a colour
//the message should float upwards from its creation point before destroying itself after a given time
public class StatusDisplayMessage : MonoBehaviour {
	//lifetime of the message
	public const float messageLifetime = 1.5f;
	//how long it has been alive for
	private float currentLifetime = 0f;

	//how long to fade the alpha for (happens when about to die)
	private float fadeTime = 0.2f;
	//the distance to make the message move over its lifetime
	public const float messageMoveDistance = 100f;

	//the text of the message
	private Text messageText;
	public Text MessageText {
		get {
			return messageText;
		}
	}

	// Use this for initialization
	void Awake () {
		//get a reference to the text component of this gui
		messageText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		ManualUpdate (Time.deltaTime);
	}

	//the message can be update manually to account for being created in between frames
	public void ManualUpdate(float deltaTime) {
		currentLifetime += deltaTime;

		//if the current life exceeds the full lifetime then destroy the object
		if (currentLifetime >= messageLifetime) {
			Destroy (gameObject);
		} else {
			//otherwise update the position and alpha of the text based on the lifetime

			//get the lifetime percentage
			float lifePercent = currentLifetime / messageLifetime;
			transform.localPosition = new Vector3(transform.localPosition.x, lifePercent * messageMoveDistance, transform.localPosition.z);

			//if in the fading stage get the fade percentage
			float timeFading = currentLifetime - (messageLifetime - fadeTime);
			if (timeFading > 0) {
				float fadePercent = 1 - timeFading / fadeTime;
				Color newColor = messageText.color;
				newColor.a = fadePercent;
				messageText.color = newColor;
			}
		}
	}
}
