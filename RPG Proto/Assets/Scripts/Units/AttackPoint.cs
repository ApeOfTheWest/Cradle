using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used as a guide for where on a unit certain attacks should land
//should be positioned and moved with unit such that the attack lines up visually
public abstract class AttackPoint : MonoBehaviour {
	//give the point the ability to modify the return value rather than just giving the transform
	public virtual Vector3 GetPoint() {
		return transform.position;
	}
}
