using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//the sound system handles the playing of music, and the playing of non positional sounds (like notification noises)
public class SoundSystem : MonoBehaviour {
	//the audio source to use for music
	private AudioSource musicSource;
	public AudioSource MusicSource {
		get {
			return musicSource;
		}
	}
	//audio source for one off sound effects
	private AudioSource systemSFX;
	public AudioSource SystemSFX {
		get {
			return systemSFX;
		}
	}
	private AudioSource worldSFX;
	public AudioSource WorldSFX {
		get {
			return worldSFX;
		}
	}

	//the mixer being used by the sound system
	public AudioMixer mixer;

	//audio groups stored by the mixer, these are used to adjust the audio levels of sounds
	private AudioMixerGroup music;
	//both world and system sound effects are controlled by the same volume slider but world fx may be affected by reverb based on the environment
	private AudioMixerGroup worldSoundEffects;
	private AudioMixerGroup systemSoundEffects;
	private AudioMixerGroup master;

	//the saved music audio state, often saved before a battle and reverted after
	private AudioClip savedMusicClip = null;
	private float savedMusicPosition;

	//global variables for controlling audio with

	// Use this for initialization
	void Awake () {
		//get both audio souces and assign them their roles, loop the music source
		AudioSource[] audios = GetComponents<AudioSource>();

		musicSource = audios [0];
		musicSource.loop = true;
		systemSFX = audios [1];
		worldSFX = audios [2];

		//set the music and system SFX to 2D mode
		musicSource.spatialBlend = 0f;
		systemSFX.spatialBlend = 0f;
		worldSFX.spatialBlend = 1f;

		//set the outputs based on the audio mixer
		music = mixer.FindMatchingGroups("Music")[0];
		worldSoundEffects = mixer.FindMatchingGroups ("Worldspace SFX")[0];
		systemSoundEffects = mixer.FindMatchingGroups ("System SFX") [0];
		master = mixer.FindMatchingGroups ("Master")[0];

		musicSource.outputAudioMixerGroup = music;
		systemSFX.outputAudioMixerGroup = systemSoundEffects;
		worldSFX.outputAudioMixerGroup = worldSoundEffects;
	}

	//save the current music position
	public void SaveMusicState() {
		savedMusicClip = musicSource.clip;
		savedMusicPosition = musicSource.time;
	}
	//revert to the last saved music position
	public void RevertMusicState() {
		musicSource.clip = savedMusicClip;
		musicSource.time = savedMusicPosition;
		musicSource.Play ();
	}
}
