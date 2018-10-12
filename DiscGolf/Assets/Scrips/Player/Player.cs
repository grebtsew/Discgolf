using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	  [Header("Difficult dependent")]
	public LEVEL vision;
	public LEVEL repeatability;

	 [Header("Character dependent")]
	public HAND hand;
	public float spin;
	public float strength;	
	public float speed = 90;  // km/h

	// can add player skins and animations here
}
