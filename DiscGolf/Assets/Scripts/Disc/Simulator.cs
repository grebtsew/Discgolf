using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;

/*
This is the actual simulation of disc throws.
We are relying on rigidbody and applying forces in correct directions.
*/

public class Simulator : MonoBehaviour
{
    private float minAnimationSpeed = 5;
    private float animationSpeed = 30;
    private const double RHO = 1.23; //The density of air in kg/m^3.
    private const float smooth_delta_loss = 1.001f;
    private const float smooth_delta_turn_loss = 1.004f;
   
    // Used for calculation
    private float alpha = 0;
    private float current_speed;
    private Vector3 init_force;
    private Quaternion init_rotation;
    private double roll;
    private double pitch;
    private double spin;
    
     [Header("Visuals")]
    // for UI
    //public Text distance; // distance text
    //public Text Throws; // throw count text
    public float throw_arm_length = 5;
    private float throw_acceleration = 1.05f; // 5% acceleraton
   
  
    
    [Header("States")]
    // States
    public bool isThrown = false; // Is the disc in the air
    private bool isRotate = false; // If the disc in the air it rotates
    public bool throw_mode = true; // is throw mode
    public bool backhand = true; // fore or backhand
    public bool at_tee = true;
    

    [Header("Throw Simulation Input")]
    public float rotationalSpeed=50;
    public float speed = 100;
    public Vector3 position;
    public Quaternion rotation;
    //TODO: player throw deviations

    [Header("Don't change these")]
     public int number_throws = 0;
    public Disc Disc; // The Frisbee
    public basket_script basket; // an obligatory basket, used to look at!
    public Rigidbody rigidBody;
    public bool throw_animation = false;
    public float throw_delta_acceleration = 0;
    public Vector3 discNextPosition; // Next throw position
    public Vector3 current_throw_pos; // current throw pos
    public Vector3 discInitialPosition; // Disc initial position
    public Quaternion discInitialRotation; // Disc initial rotation
    public follow_camera cam; // camera
    public List<Vector3> throwpositions = new List<Vector3>(); // all throw positions, history list

    public void UpdateDiscComponent(){
         Disc = GetComponentInChildren<Disc>();
    }

    void Start()
    {
        cam = FindObjectOfType<follow_camera>();
        UpdateDiscComponent();
        basket = FindObjectOfType<basket_script>();
        rigidBody = GetComponent<Rigidbody>(); // Get Disc Rigidbody

        discInitialPosition = transform.position; // Get disc localposition related to Player
        discInitialRotation = transform.rotation; // Get disc localrotation
        ResetDisc();


         alpha =  Vector3.Angle(rigidBody.velocity, transform.forward);
    }

      public void ResetDisc()
    {
        isThrown = false; // Next throw
        isRotate = false; // The disc will not rotate
    }

    /*
     These are the forces applied to flying disc

        // Orientation:
        // ROLL
        // z = Roll = p
        // forward

        // PITCH
        // x = Pitch  = q
        // sides

        // SPIN DOWN
        // y = spin down = r
        // height
     */

    private void Lift(){
        double cl = Disc.CL0 + Disc.CLA *alpha *Math.PI / 180; // lift constant
        double lift = (RHO * Math.Pow(rigidBody.velocity.magnitude, 2) * Disc.AREA * cl /2 /  Disc.m)*Time.deltaTime*4 + Disc.GLIDE/9; // TODO make this more realistic!
        rigidBody.AddForce(transform.up.normalized * (float) lift , ForceMode.Acceleration );
    }

    private void Drag(){
        double cd = Disc.CD0 + Disc.CDA * Mathf.Pow((float)(alpha - (Disc.ALPHA0)*Math.PI / 180),2);
        double drag = (RHO * Math.Pow(rigidBody.velocity.magnitude, 2) * Disc.AREA * cd)/2 * (15/Disc.SPEED)/1.5f; // Gamification!
        rigidBody.AddForce(-rigidBody.velocity.normalized * (float)drag, ForceMode.Acceleration ) ;
    }
    
    private void Gravity(){
         rigidBody.AddForce(0,-9.82f,0, ForceMode.Acceleration );
    }

    private void Torque(){
         roll = (Disc.CRR*rigidBody.angularVelocity.y+Disc.CRP*rigidBody.angularVelocity.x)* 1/2 * RHO * Math.Pow(rigidBody.velocity.magnitude,2) * Disc.AREA * Disc.diameter*0.01f*6-Disc.TURN/2;//TODO make this more realistic!
        roll-= Disc.FADE*3; // Gamification!
        spin = -(Disc.CNR*rigidBody.angularVelocity.y) * 1/2 * RHO * Math.Pow(rigidBody.velocity.magnitude,2) * Disc.AREA *Disc.diameter*0.01f;
        pitch = (Disc.CM0 + Disc.CMA*(Math.PI / 180*(alpha))  + Disc.CMq* rigidBody.angularVelocity.z) * 1/2 * RHO * Math.Pow(rigidBody.velocity.magnitude,2) * Disc.AREA * Disc.diameter*0.01f*6; //TODO make this more realistic!
        
        if (!backhand){ // TODO: doesnt seem to work?
            rigidBody.AddTorque(Vector3.Cross(transform.up,rigidBody.velocity).normalized*(float)-roll, ForceMode.Acceleration);
         }else{
            rigidBody.AddTorque(Vector3.Cross(transform.up,rigidBody.velocity).normalized*(float)roll, ForceMode.Acceleration);
         }
        //Debug.Log(new Vector3((float)roll,(float)spin,(float)pitch));
       
        rigidBody.AddTorque(transform.up*(float)spin, ForceMode.Acceleration);
       rigidBody.AddTorque(rigidBody.velocity.normalized*(float)pitch, ForceMode.Acceleration);
    }

    void FixedUpdate()
    {
       

        if (isThrown)
        {
            Lift();
            Drag();
            Gravity();
            Torque();
            CheckLanding();
            CheckOutOfBounds();

        } else {

         if (throw_animation)
        {

            throw_delta_acceleration *= throw_acceleration;
            transform.position += transform.forward * throw_delta_acceleration;

            if (Vector3.Distance(transform.position, position-transform.forward*throw_arm_length) >= Vector3.Distance( position, position - transform.forward * throw_arm_length))
            {
                InitThrow();
                throw_animation = false;
            }
        }

        }
    }


    private void CheckOutOfBounds(){
        if (this.transform.position.y < -1000 ){
            Reset();
            Debug.Log("Out of bounds. Reset disc position.");
        }
    }

    private void CheckLanding()
    {
        if(rigidBody.velocity.x <= 0 && rigidBody.velocity.y <= 0 && rigidBody.velocity.z <= 0)
        {
           //distance.text = Math.Round(Vector3.Distance(discInitialPosition, transform.position)).ToString();
        }
    }

    private void NextThrow()
    {
        rigidBody.isKinematic = true; // Remove gravity to the disc
        rigidBody.useGravity = false;
    }

    private void InitThrow()
    {
        /*
        Prepare necessary variables and states for a new throw!
        */
        
        // Added variables
                                 // reset rotation speed
        discInitialRotation = transform.rotation; // Get disc localrotation
        animationSpeed = (speed / 10) + minAnimationSpeed;   // set speed
        
        rigidBody.maxAngularVelocity = 2000;

        rigidBody.drag = 0;
        rigidBody.mass = Disc.m;
        rigidBody.useGravity = false;
        rigidBody.isKinematic = false;          // Add gravity to the disc

        if (!backhand){
            rigidBody.AddTorque(transform.up*-rotationalSpeed*0.01f, ForceMode.Impulse );
        } else {
            rigidBody.AddTorque(transform.up*rotationalSpeed*0.01f, ForceMode.Impulse );
        }
        

      //  rigidBody.AddForce(transform.right * force.x); // Add force on X to the disc
      //  rigidBody.AddForce(transform.up * force.y); // Add force on Y to the disc
        
        rigidBody.AddForce(transform.forward*speed/3.6f , ForceMode.Impulse ); // Add force on forward to the disc

        isThrown = true; // Disc is thrown by player
        throw_mode = true;
        isRotate = true; // The disc will rotate
        at_tee = false;
    }

       
    public void Throw()
    {
        // Perform actual throw
        number_throws++;
        throw_delta_acceleration =throw_arm_length / animationSpeed;
        position = transform.position;
        transform.position -= transform.forward * throw_arm_length;
        throw_animation = true;
       
    }



    public void Reset()
    {
       
        throwpositions.Clear();
        isThrown = false;
        throw_mode = true;
        transform.position = discInitialPosition;
        transform.rotation = discInitialRotation;
        at_tee = true;
        number_throws = 0;
        ResetDisc();
        NextThrow();

        // update camera
        cam.updatePosition();
        cam.mode = camera_mode.throw_mode;

    }

}
