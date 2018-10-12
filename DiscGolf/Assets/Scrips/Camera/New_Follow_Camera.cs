using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_Follow_Camera : MonoBehaviour {

    private Vector3 offset;
    private Vector3 stand_offset;
    private Disc_Movement frisbee;
    public camera_mode mode = camera_mode.throw_mode;

    private float climbSpeed = 4;
    private float normalMoveSpeed = 10;
    private float slowMoveFactor = 0.25f;
    private float fastMoveFactor = 3;

    void Start()
    {
        frisbee = FindObjectOfType<Disc_Movement>();
        offset = transform.position - frisbee.transform.position;

       


    }



    void Update()
    {
    
                transform.position = frisbee.transform.position + offset;
                transform.LookAt(frisbee.transform);
              
    }

  


}
