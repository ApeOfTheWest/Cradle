using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//base of the pause menu component, handles the opening and closing of the menu
public class PauseMenu : MonoBehaviour {
	PersistentData data = null;

	//the holder of the content inactive content, should be kept inactive so as to be uninteractable
	//public GameObject inactiveContentHolder;
	//holder of active content
	public GameObject contentHolder;

	//the different menu contents to open
	private PartyTab partyTab;
	private InventoryTab inventoryTab;
	private LogTab logTab;
	private SystemTab systemTab;

	//the currently displayed content, will be placed back in the content holder when replaced
	private GameObject contentOnDisplay = null;

	void Awake() {
		partyTab = contentHolder.GetComponentInChildren<PartyTab> (true);
		partyTab.gameObject.SetActive (false);
		inventoryTab = contentHolder.GetComponentInChildren<InventoryTab> (true);
		inventoryTab.gameObject.SetActive (false);
		logTab = contentHolder.GetComponentInChildren<LogTab> (true);
		logTab.gameObject.SetActive (false);
		systemTab = contentHolder.GetComponentInChildren<SystemTab> (true);
		systemTab.gameObject.SetActive (false);
	}

	//open the menu
	public void OpenMenu() {
		//pause the game
		LevelController.levelControllerInstance.WorldPaused = true;

		//set up the menu
		SetupMenu(LevelController.levelControllerInstance.GetPersistentData());

		//set the audio mixer snapshot to the pause menu snapshot (audio levels must be updated after this)
		AudioMixerSnapshot pauseSnap = LevelController.soundSystemInstance.mixer.FindSnapshot("Paused");
		pauseSnap.TransitionTo (0);
		//apply volumes
		data.systemSettings.ApplyAudioSettings(LevelController.soundSystemInstance.mixer);

		//enable this component
		gameObject.SetActive(true);

		//start the menu off in the party tab
		Party();
	}

	//setup the menu using the persistent data (changes in the menu should be saved to the data)
	private void SetupMenu(PersistentData data) {
		this.data = data;

		partyTab.Initialise (data);
		inventoryTab.Initialise (data);
		logTab.Initialise (data);
		systemTab.Initialise (data);
	}

	//cleanup objects
	private void Cleanup() {
		partyTab.Cleanup ();
		inventoryTab.Cleanup ();
		logTab.Cleanup ();
		systemTab.Cleanup ();
	}

	//close the menu
	public void CloseMenu() {
		Cleanup ();

		//disable this component
		gameObject.SetActive(false);

		//return any content to the holder
		ReturnTabContent ();

		//set the audio mixer snapshot back
		AudioMixerSnapshot defaultSnap = LevelController.soundSystemInstance.mixer.FindSnapshot("Default");
		defaultSnap.TransitionTo (0);
		//apply volumes
		data.systemSettings.ApplyAudioSettings(LevelController.soundSystemInstance.mixer);

		//null reference
		data = null;

		//unpause the game
		LevelController.levelControllerInstance.WorldPaused = false;
	}

	//return the current content tab
	private void ReturnTabContent() {
		if (contentOnDisplay != null) {
			contentOnDisplay.SetActive (false);
			contentOnDisplay = null;
		}
	}

	//links from tab buttons in the menu, should be called when selected (except for back button)
	public void Party() {
		ReturnTabContent ();

		contentOnDisplay = partyTab.gameObject;
		contentOnDisplay.SetActive (true);
	}

	public void Inventory() {
		ReturnTabContent ();

		contentOnDisplay = inventoryTab.gameObject;
		contentOnDisplay.SetActive (true);
	}

	public void Log() {
		ReturnTabContent ();

		contentOnDisplay = logTab.gameObject;
		contentOnDisplay.SetActive (true);
	}

	public void System() {
		ReturnTabContent ();

		contentOnDisplay = systemTab.gameObject;
		contentOnDisplay.SetActive (true);
	}
}
