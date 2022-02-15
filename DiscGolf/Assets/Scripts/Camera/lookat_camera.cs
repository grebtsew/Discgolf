using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookat_camera : MonoBehaviour
{

    private SimulatorUIhelper frisbee;

    void Start()
    {
        frisbee = FindObjectOfType<SimulatorUIhelper>();
    }

    void Update()
    {
        transform.LookAt(frisbee.transform);
    }
}
