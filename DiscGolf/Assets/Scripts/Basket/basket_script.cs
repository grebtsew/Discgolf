using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basket_script : MonoBehaviour
{


    public float dampening = 100f;
    
    private SimulatorUIhelper disc;
    private Rigidbody rigidBody;

    // Use this for initialization
    void Start()
    {
        disc = FindObjectOfType<SimulatorUIhelper>();
        rigidBody = disc.getRigidbody();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "disc")
        {
            rigidBody.velocity = rigidBody.velocity/dampening;
            // Also trigger audio here!
        }
    }


}
