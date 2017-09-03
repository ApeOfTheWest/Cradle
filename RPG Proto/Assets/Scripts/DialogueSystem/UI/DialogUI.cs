using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour {
	//controls the ui system that plays back dialogue from a stored conversation
	//takes orders from the dialog player stored on it
	private DialogPlayer player = new DialogPlayer();
	public DialogPlayer Player {
		get {
			return player;
		}
	}

	//lock the dialog system, prevents transitioning to the next node while locked
	//used while custom ui or animation is being played to prevent skipping ahead
	private bool locked;
	public bool Locked {
		get {
			return locked;
		}
		set {
			locked = value;
		}
	}

	//whether the main dialog ui should be hidden (used with custom ui)
	private bool hideMainUI;
	public bool HideMainUI {
		get {
			return hideMainUI;
		}
		set {
			hideMainUI = value;

			baseDialogUI.SetActive (!hideMainUI);
			
		}
	}

	//the base dialog ui (no custom stuff)
	[SerializeField]
	private GameObject baseDialogUI;

	//different ui components
	[SerializeField]
	private TextHolder textHolder;
	[SerializeField]
	private NameLabel nameLabel;
	[SerializeField]
	private Image leftSprite;
	[SerializeField]
	private Image rightSprite;
	[SerializeField]
	private ResponseHolder responseHolder;
	//gameobject to hold a custom ui in
	//any remnants will be wiped on loading a new screen
	[SerializeField]
	private GameObject customUI;
	public GameObject CustomUI {
		get {
			return customUI;
		}
	}

	//spped to scroll text
	public float typeSpeed = 0.1f;

	//whether this node uses responses
	bool responseNode = false;
	//whether the current text stream has finished
	bool textFinished = false;

	void Awake() {
		player.ui = this;
	}

	void Update() {
		//skip this if text has already finished
		if (textFinished == false) {
			//if the skip button is pressed here then tell the holder to skip foward
			if (Input.anyKeyDown) {
				textHolder.Skip ();
			}

			//update the text holder and retireve a bool to show if the text is finished
			textFinished = textHolder.UpdateText (typeSpeed);

			//if the text has just finished display the responses
			if (textFinished == true) {
				responseHolder.ButtonsActive = true;
			}
		} else if (responseNode == false) {
			//if text has been finished for a frame and has no responses then check for the skip command that will go to the next node
			if (Input.anyKeyDown) {
				//go onto the next node
				player.GotoNextNode ();
			}
		}
	}

	//call to end a conversation (called by the dialog player)
	public void EndConversation() {
		//pass back control to the player, unpause world if paused
		if (LevelController.levelControllerInstance != null) {
			LevelController.levelControllerInstance.WorldPaused = false;
		}

		//disable this
		gameObject.SetActive(false);
	}

	//display the speaker, text, responses and sprites of a node
	//any requested information should be appended to the text in this node first before passing it in
	public void DisplayNodeContents(DialogNode node) {
		textFinished = false;
		textHolder.SetText (node.nodeText);

		//clear any previous node's responses
		responseHolder.ClearButtons();
		//hide responses until text is displayed
		responseHolder.ButtonsActive = false;

		if (node.responses.Count > 0) {
			responseNode = true;

			//make the responses and hide them until the text is displayed
			for (int i = 0; i < node.responses.Count; i++) {
				responseHolder.AddResponse (node.responses [i], i, player);
			}
		} else {
			responseNode = false;
		}

		//if the name is non null then display the label and set it
		if (node.speakerName != "") {
			nameLabel.gameObject.SetActive (true);
			nameLabel.SetName (node.speakerName);
		} else {
			nameLabel.gameObject.SetActive (false);
		}

		//if left portrait is non null display it
		if (node.leftPortrait != null) {
			leftSprite.gameObject.SetActive (true);
			//set the size of the iamge to the sprite size
			leftSprite.rectTransform.sizeDelta = new Vector2(node.leftPortrait.rect.width, node.leftPortrait.rect.height);
			//leftSprite.rectTransform.rect.height = node.leftPortrait.rect.height;

			leftSprite.sprite = node.leftPortrait;
		} else {
			leftSprite.gameObject.SetActive (false);
		}

		//if right portrait is non null display it
		if (node.rightPortrait != null) {
			rightSprite.gameObject.SetActive (true);

			rightSprite.rectTransform.sizeDelta = new Vector2(node.rightPortrait.rect.width, node.rightPortrait.rect.height);
			rightSprite.sprite = node.rightPortrait;
		} else {
			rightSprite.gameObject.SetActive (false);
		}
	}
		
	//call to start a conversation (called by the outside world), include a bool showing whether to pause the world or not
	public void StartConversation(DialogTree conversation, DialogEventInterface eventInterface, bool pauseWorld = true) {
		//only pause the world if pause is true (avoids potential unwanted unpausing)
		if (pauseWorld == true) {
			LevelController.levelControllerInstance.WorldPaused = pauseWorld;
		}
		//start it unlocked by default
		Locked = false;
		//make sure ui isnt being hidden by default
		HideMainUI = false;

		//enable the display objects
		gameObject.SetActive (true);

		//start the player
		player.StartConversation(conversation, eventInterface);
	}
		
}
