using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a hitspark should last for a certain amount of time before destroying itself and removing itself
public class Hitspark : MonoBehaviour {
	//the lifetime of the hitspark, after this it is destroyed
	public float lifetime = 1f;
	//the time the hitspark has been alive for
	public float timeAlive = 0f;

	// Use this for initialization
	void Awake () {
		timeAlive = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		timeAlive += Time.deltaTime;

		if (timeAlive >= lifetime) {
			//destroy the hitspark and the object it is attached to
			Destroy(gameObject);
		}
	}
}
