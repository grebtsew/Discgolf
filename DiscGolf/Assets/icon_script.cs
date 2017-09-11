using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class icon_script : MonoBehaviour {

    static float height = 50;

    private Transform self;

    public Transform target;

	// Use this for initialization
	void Start () {
        self = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        self.position = new Vector3(target.position.x,height, target.position.z);
	}
}
