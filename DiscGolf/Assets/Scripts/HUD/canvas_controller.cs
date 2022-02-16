using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvas_controller : MonoBehaviour
{

    public Slider x_rot;
    public Slider y_rot;
    public Slider z_rot;
    public Slider x_pos;
    public Slider y_pos;
    public Slider z_pos;
    public Slider spin;
    public Slider speed;

    private SimulatorUIhelper frisbee;
    private Vector3 startpos;
    private Rigidbody rigidBody;

    void Start()
    {
        frisbee = FindObjectOfType<SimulatorUIhelper>();
        startpos = frisbee.transform.position;
        rigidBody = frisbee.getRigidbody();
        // set spinner start value
        //spin.value = frisbee.getSpin();
        //speed.value = frisbee.getSpeed();
    }

    /// publicly used methods
    /// 
    /// 
    public void resetSliders()
    {
      x_rot.value = 0;
      y_rot.value = 0;
      z_rot.value = 0;
      x_pos.value = 0;
      y_pos.value = 0;
      z_pos.value = 0;
}
    public void updateSpin()
    {
        frisbee.SetSpin(spin.value);
    }
    public void updateSpeed()
    {
        frisbee.SetSpeed(speed.value);
    }
    public void updateRotation()
    {
        rigidBody.MoveRotation(Quaternion.Euler(x_rot.value, y_rot.value+90 , z_rot.value));
 
    }
    public void updateposition()
    {
        if (frisbee.isAtTee()) { 
        rigidBody.position = new Vector3(startpos.x + x_pos.value, startpos.y + y_pos.value, startpos.z + z_pos.value);
        }
    }
    public void updateStartpos()
    {
        startpos = frisbee.transform.position;
    }

}
