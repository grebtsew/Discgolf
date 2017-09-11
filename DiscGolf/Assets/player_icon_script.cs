using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_icon_script : MonoBehaviour {


    static float height = 50;
    static float arm = 5;

    public Transform body;
    public Transform target;
    public Transform direction;

 
	// Update is called once per frame
	void Update () {
        body.position = new Vector3(target.position.x, height, target.position.z);
        direction.position = new Vector3(body.position.x + target.forward.x * arm, height, body.position.z + target.forward.z * arm);
    }
}
