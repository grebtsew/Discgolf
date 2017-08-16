using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookat_camera : MonoBehaviour
{

    private Disc_Movement frisbee;

    void Start()
    {
        frisbee = FindObjectOfType<Disc_Movement>();
    }

    void Update()
    {
        transform.LookAt(frisbee.transform);
    }
}
