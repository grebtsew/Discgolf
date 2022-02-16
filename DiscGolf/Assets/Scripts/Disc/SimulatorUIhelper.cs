using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;

public class SimulatorUIhelper : MonoBehaviour
{
    public Text distanceText;
    public Text throwsText;

    void Update(){
        distanceText.text = Math.Round(Vector3.Distance(simulator.discInitialPosition, simulator.transform.position)).ToString();
        throwsText.text = simulator.number_throws.ToString();

        if (Input.GetKey(KeyCode.T) )
        {
            NextThrow();
        }
        if (Input.GetKey(KeyCode.N) )
        {
            Next();
        }
         if (Input.GetKey(KeyCode.B) )
        {
            Back();
        }
          if (Input.GetKey(KeyCode.R) )
        {
            Reset();
        }
       

    }

   private Simulator simulator;
    void Start(){
        simulator = GetComponent<Simulator>();
    }

    public float getSpeed(){
        return simulator.speed;
    }

    public float getSpin(){
        return simulator.rotationalSpeed;
    }

    public bool isThrown(){
        return simulator.isThrown;
    }

    public Rigidbody getRigidbody(){
        return GetComponent<Rigidbody>();
    }

     public Vector3 getInitialPosition(){
        return simulator.discInitialPosition;
    }

    public Quaternion getInitialRotation(){
        return simulator.discInitialRotation;
    }

    public void SetSpin(float f)
    {
        if (!simulator.isThrown)
        {
            simulator.rotationalSpeed = f;
        }
    }

    public void SetHand()
    {
        if (!simulator.isThrown)
        {
            if (simulator.backhand)
            {
                simulator.backhand = false;
            }
            else
            {

                simulator.backhand = true;
            }

        }
    }

    public bool isAtTee(){
        return simulator.at_tee;
    }
    
    public void SetSpeed(float f)
    {
        simulator.speed = f;
    }

    public void ThrowDisc()
    {
        // Perform actual throw
        simulator.throw_delta_acceleration = simulator.throw_arm_length / 30;
        simulator.position = transform.position;
        simulator.transform.position -= transform.forward * simulator.throw_arm_length;
        simulator.throw_animation = true;
        //simulator.Throws.text = simulator.number_throws.ToString();
    }

   
    void NextThrow()
    {
        simulator.rigidBody.isKinematic = true; // Remove gravity to the disc
        simulator.rigidBody.useGravity = false;
    }

    public void Next_reset(Vector3 reset_pos)
    {
        simulator.isThrown = false;
        simulator.throw_mode = true;
        simulator.transform.position = new Vector3(reset_pos.x, reset_pos.y + 2, reset_pos.z);
        
        if (simulator.basket){
        simulator.transform.LookAt(simulator.basket.transform);
        }

        simulator.ResetDisc();
        NextThrow();

        // update camera
        simulator.cam.updatePosition();
        simulator.cam.mode = camera_mode.throw_mode;
    }

    public void Next()
    {
        if (simulator.isThrown)
        {
            simulator.throw_mode = true;

            simulator.current_throw_pos = simulator.Disc.transform.position;
            simulator.throwpositions.Add(simulator.current_throw_pos);
            Next_reset(simulator.current_throw_pos);

        }
    }

    public void Back()
    {
        if (simulator.isThrown)
        {

            Next_reset(simulator.current_throw_pos);
            return;
        }
        simulator.throw_mode = true;

        if (simulator.throwpositions.Count > 1)
        {
            simulator.throwpositions.Remove(simulator.current_throw_pos);
            simulator.current_throw_pos = simulator.throwpositions[simulator.throwpositions.Count - 1];


            Next_reset(simulator.current_throw_pos);
        }
        else
        {
            Reset();
        }
    }

    public void Reset()
    {
       
        simulator.throwpositions.Clear();
        simulator.isThrown = false;
        simulator.throw_mode = true;
        simulator.transform.position = simulator.discInitialPosition;
        simulator.transform.rotation = simulator.discInitialRotation;
        simulator.at_tee = true;
        simulator.number_throws = 0;
        simulator.ResetDisc();
        NextThrow();

        // update camera
        simulator.cam.updatePosition();
        simulator.cam.mode = camera_mode.throw_mode;

    }

}
