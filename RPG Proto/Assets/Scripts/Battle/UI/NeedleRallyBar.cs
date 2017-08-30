using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedleRallyBar : MonoBehaviour, RallyBar {
	//the current normalised position of the needle, may lag beind the true value
	//needle will visually jitter around this value
	//the bar itself will stay at this value
	private float currentBarPosition = 0f;

	//the current offset of the needle due to jitter, this is in degrees
	//this added to the needle position gives the absolute position of the needle
	private float needleOffsetDegrees = 0;

	//the time that the needle should be taking to reach the next destination
	//this is set when a new value is called is updated with
	private float transitionDuration = 0f;
	//the current time that the needle is at during the transition
	//this is reset to 0 when the end value changes
	private float transitionTime = 0f;

	//the starting value of the bar when the last transition was called for
	private float barStartPosition = 0f;
	//the end value of the bar when the last transition was called for
	private float barEndPosition = 0f;

	//public variables that control how the needle on the rally meter animates
	//the amount that the needle lags behind the value by when it changes, at 0 it keeps up perfectly
	//this is defined as the amount of time in seconds it would take for the needle to catch up with a full meter change
	//the needle should be faster the further from equilibrium it is
	public float needleLag = 2f;
	//min jitter size and the amount of randomly generated variation that may be added
	public float needleJitterVariation = 3f;
	public float needleJitterMin = 1f;
	//the time taken to perform one half cycle of jitter
	public float needleJitterDuration = 0.3f;

	//which side the needle jitter is currently on, will swap each time
	//this determines whether the offset is positive or negative
	private bool jitterPositive = true;

	//the time of the current jitter cycle
	private float needleJitterTime = 0f;
	//the magnitude of the current jitter cycle
	private float currJitterMag = 0f;

	//the meter bar to fill
	private Image meterBar;
	//the needle to follow the bar
	private Image needle;

	// Use this for initialization
	void Awake () {
		//get the image components by name
		meterBar = transform.Find("BarFill").GetComponent<Image>(); 
		needle = transform.Find("Needle").GetComponent<Image>(); 

		//upon initializing calculate the jitter magnitude for the first jitter cycle
		RandomiseJitterMagnitude();
	}

	//calculate the jitter magnitude using a randomiser
	private void RandomiseJitterMagnitude() {
		currJitterMag = needleJitterMin + Random.Range(0f, needleJitterVariation);
	}
	
	//called every update, value may not have changed so check against last cached value before doing a transition
	public void UpdateRallyPercent(float newLevel) {
		//check if the new level is different from the current goal position
		//if so then update the end and start positions to reflect this position
		if (newLevel != barEndPosition) {
			barStartPosition = currentBarPosition;
			barEndPosition = newLevel;

			//reset the transition timer
			transitionTime = 0f;
			//set the duration based on the needle lag and the distance being travelled
			transitionDuration = Mathf.Abs(barEndPosition - barStartPosition) * needleLag;
		}

		//update time if the transition isn't finished yet
		if (transitionTime < transitionDuration) {
			transitionTime += Time.deltaTime;
		}

		//move the needle from the current position to the next position
		//do this based on the lag
		if (transitionTime >= transitionDuration) {
			//if transition is finished then just set the current position to the end
			currentBarPosition = barEndPosition;
		} else {
			//otherwise set the position using a quadratic in/out easing function between the start position and end position
			//get the change in value from start
			float valueChange = barEndPosition - barStartPosition;

			//divide the time by half the duration
			float timeHalfDur = transitionTime / (transitionDuration / 2f);

			//accelerate if less than halfway through duration
			//deccelerate if more
			if (timeHalfDur < 1) {
				currentBarPosition = valueChange / 2f * timeHalfDur * timeHalfDur * timeHalfDur + barStartPosition; 
			} else {
				timeHalfDur -= 2;

				currentBarPosition = valueChange / 2f * (timeHalfDur * timeHalfDur * timeHalfDur + 2f) + barStartPosition; 
			}
		}

		//update the needle offset based on the jitter variables
		needleJitterTime += Time.deltaTime;

		//check if the time is greater than the duration
		//if it is then calculate a new jitter maginitude
		if (needleJitterTime >= needleJitterDuration) {
			RandomiseJitterMagnitude ();
		}
		//while time is greater than duration flip the signage of the jitter
		while (needleJitterTime >= needleJitterDuration) {
			needleJitterTime -= needleJitterDuration;
			jitterPositive = !jitterPositive;
		}
		//and now calculate the offset in degrees using the time, duration and maginute with the positive half of a sin function
		needleOffsetDegrees = currJitterMag * Mathf.Sin( (needleJitterTime / needleJitterDuration) * Mathf.PI);
		//if the jitter is negative then invert this
		if (jitterPositive == false) {
			needleOffsetDegrees *= -1f;
		}

		SetNeedleAndBar ();
	}

	//called to set the display rather than update it, should avoid transitioning, usually called at start of batlle
	public void SetRallyPercent(float newLevel) {
		//set the needle offset to 0
		needleOffsetDegrees = 0;
		//set the bar position according to the level directly
		currentBarPosition = newLevel;

		//set the bar and needle directly
		SetNeedleAndBar();
	}

	//method that sets the meter fill amount and needle rotation according to the current bar position and needle offset
	private void SetNeedleAndBar() {
		//set the bar fill percentage from 0 to 1
		meterBar.fillAmount = currentBarPosition;

		//set the needle position by rotating it from 0 to -90 in the z axis
		//add the needle offset
		needle.transform.rotation = Quaternion.Euler(0, 0, -90f * currentBarPosition - needleOffsetDegrees);
	}

	//call when resetting the level display
	public void ResetDisplay() {
		//reset the position of the bar and needle to 0
		needleOffsetDegrees = 0f;
		currentBarPosition = 0f;
	}
}
