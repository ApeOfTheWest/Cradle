using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SystemSettings {
	//the path to save the settings at
	public const string SAVE_PATH = "SystemSettings.Sett";

	//the volume levels on the mixer
	//these are stored as +- db (can go from -80 to +20)
	public float masterVolume = 0f;
	public float musicVolume = 0f;
	public float soundEffectVolume = 0f;

	public bool fullscreen = true;
	//apply resolution through properties to prevent wierd resolutions
	private Resolution resolution;
	public Resolution Resolution {
		get {
			return resolution;
		}
		set {
			resolution = value;

			//check resolution to make sure it is a sensible value
			//first off if either value is less than 300 make it 300
			if (resolution.width < 700) {
				resolution.width = 700;
			}
			if (resolution.height < 700) {
				resolution.height = 700;
			}
		}
	}

	//when a new system settings is made it should automatically grab the resolution from the computer and start in fullscreen mode
	//public SystemSettings() {
		//get the resolution from the monitor in use
		//resolution = Screen.currentResolution;
//	}

	//method that applies any setting changes to the scene
	//call when first starting the game and also when changes are made
	//take in the audio mixer to use
	public void ApplySettings(AudioMixer mixer) {
		ApplyAudioSettings (mixer);
		ApplyGraphicSettings ();
	}

	//method that only applies audio settings
	public void ApplyAudioSettings(AudioMixer mixer) {
		//get the audio mixer object and apply the audio settings to the channels
		mixer.SetFloat("MasterVol", masterVolume);
		mixer.SetFloat("MusicVol", musicVolume);
		mixer.SetFloat("SFXVol", soundEffectVolume);
		mixer.SetFloat("WorldSFXVol", soundEffectVolume);
	}

	//method that only applies graphical settings
	public void ApplyGraphicSettings() {
		//Screen.SetResolution (resolution.width, resolution.height, fullscreen);
	}

	//method to save the settings at the specified location
	public void SaveSettings() {

	}

	//method to try to load settings and return them, returns null if none are found
	public static SystemSettings LoadSettings() {


		return null;
	}
}
