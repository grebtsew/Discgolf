using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basket_script : MonoBehaviour
{

    private Disc_Movement disc;

    // Use this for initialization
    void Start()
    {
        disc = FindObjectOfType<Disc_Movement>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "disc")
        {
            disc.rigidBody.velocity = Vector3.zero;
        }
    }


}
