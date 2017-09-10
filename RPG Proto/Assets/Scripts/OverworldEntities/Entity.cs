using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//entity is a generic object for any (non living) objects on the scene that can be interacted with
//but they should be objects that are non static in nature, and as such should have a rigidbody so that their attached colliders are non-static
//any object that is fixed therefore may be better off not being an entity
//for example, treasure, items, boxes
public class Entity : MonoBehaviour {
	//the rigidbody for the entity, all entities have one form of rigidbody or another so that they can be pushed around or moved
	//or just spawned and destroyed without unity assuming the colliders attached are static
	private Rigidbody2D rigidBody;
	public Rigidbody2D RigidBody {
		get {
			return rigidBody;
		}
	}
	//entities may have no colliders or wierdly nested colliders so don't include a collider object here

	//make protected so the methods above can call the base method
	protected void Awake () {
		rigidBody = GetComponent<Rigidbody2D> ();
		//in development warn if no rigidbody is found
		#if UNITY_EDITOR
		if (rigidBody == null) {
		Debug.Log("Warning: No rigidbody found on entity object");
		}
		#endif

		//set the physics layer appropritely
		SetLayer();
	}

	//set the layer of the entity for the physics engine, is virtual so that this layer can be overidden if desired
	protected virtual void SetLayer() {
		//set the layer of this to the entity layer
		gameObject.layer = GlobalTweakedConstants.ENTITY_LAYER;
	}
}
