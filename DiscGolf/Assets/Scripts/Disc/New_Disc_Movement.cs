using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System;

public class New_Disc_Movement : MonoBehaviour {

 

    // const
    public const float GRAVITY = -9.81f; //The acceleration of gravity (m/s^2).
    public const double RHO = 1.23; //The density of air in kg/m^3.
    private const float ROLL_DECREASE = 0.85f;
    private const float ROLL_SPEED = 10;
    private const float smooth_delta_loss = 1.001f;
    private const float smooth_delta_turn_loss = 1.004f;

    public float MAXSPEED = 30f; // Max speed the disc can have
    public Rigidbody rigidBody;
    private Disc Disc; // The Frisbee
    private basket_script basket; // an obligatory basket
    public Vector3 discNextPosition; // Next throw position
    public Vector3 current_throw_pos; // current throw pos
    public Vector3 force; // The force applayed on XYZ
    public Vector3 discInitialPosition; // Disc initial position 
    private Quaternion discInitialRotation; // Disc initial rotation 
    public Text distance; // distance text
    public Text Throws; // throw count text
    private Vector3 sideDir; // fade direction
    private Vector3 locVel; // local velocity
    private follow_camera cam; // camera
    private List<Vector3> throwpositions = new List<Vector3>(); // all throw positions
    private List<GameObject> Lines = new List<GameObject>();
    private int number_throws = 0;
    private Vector3 throwpos;
    float current_speed;

    public bool isThrown = false; // Is the disc in the air
    private bool isRotate = false; // If the disc in the air it rotates
    public bool throw_mode = true; // is throw mode
    public bool backhand = true; // fore or backhand
    private bool show_lines = false;
    public bool at_tee = true;
    private bool throw_animation = false;

    private float playerThrust; // Get thrust in percent 
    public float rotateSpeed; // initial speed
    public float power = 1000f; // initial power
    private float fade_rot_speed = 25f; // fade physics speed
    private  float stable_speed = 5f; //  stable speed
    private float fade_speed = 10f; // fade speed
    private float delta_rotation_speed;
    private float plan_speed = 5; // fade speed
    private float rot = 0; // rotation
    private float fade_x = 0; // fade x cos value
    private float fade_z = 0; // fade z cos value
    private float fade_z_physics = 0; // rotated 180
    private float min_speed = 20f;
    private float fast_loss = 0.3f; // 30% loss
    private float turn_drag;
    public float throw_speed = 100; // throw speed 0-150
    private float glide = 0;
    private float throw_arm_length = 5;
    private float throw_delta_acceleration = 0;
    private float throw_acceleration = 1.05f; // 5% acceleraton
    private float glide_normalizer = 5;
    private float min_fade_speed = 20f;
    private float standard_fade_speed = 10f;

    private double delta_speed_drag = 0;
    private double delta_lift; // lift force
    private double delta_gravity; // total gravity 
    private double delta_resistance; // drag resistance
    private double delta_drag; // drag
    private bool fade_started = false;


    void Start()
    {
        Disc = GetComponentInParent<Disc>();
        rigidBody = GetComponent<Rigidbody>(); // Get Disc Rigidbody

        discInitialPosition = transform.position; // Get disc localposition related to Player
        discInitialRotation = transform.rotation; // Get disc localrotation

        SetUpThrow();
    }

   
    void Update()
    {

               // force down
                Gravity_physics();

                // force up
                Lift_physics();

                // force speed reduction
                Drag_physics();
            

            CheckLanding();
          
       
        
    }

    private void Lift_physics(){
          /* 
        Divide problem
        Lift = Fz * cos(a) - Fx * sin(a)

         */

    }

    private void Gravity_physics()
    {
        /* Gravity physics
            F = ma = gm
            + eventual wind 
         */ 
    }
    private void Drag_physics()
    {
        /*
        Drag = Fz*sin(a) + Fx*sin(a)
         */
       

    }
    void CheckLanding()
    {
        if(rigidBody.velocity.x <= 0 && rigidBody.velocity.y <= 0 && rigidBody.velocity.z <= 0)
        {
          //  distance.text = Math.Round(Vector3.Distance(discInitialPosition, transform.position)).ToString();
            delta_rotation_speed *= fast_loss;
        }
    }
   
   
    
    private void SetUpThrow()
    {
     
      discInitialRotation = transform.rotation; // Get disc localrotation

        // movement dependent on y rotation as radians
        fade_x = (float)Math.Sin((transform.eulerAngles.y * Math.PI) / 180);
        fade_z = (float)Math.Cos((transform.eulerAngles.y * Math.PI) / 180);
        fade_z_physics = (float)Math.Cos(Math.PI + (transform.eulerAngles.y * Math.PI) / 180);

      
        // random forces
        force.x = 50;
        force.y = 70;
        delta_rotation_speed = 100; 
        force.z = 100;                        

        
        // Add force
        rigidBody.AddForce(transform.right * force.x); // Add force on X to the disc
        rigidBody.AddForce(transform.up * force.y); // Add force on Y to the disc
        rigidBody.AddForce(transform.forward * force.z); // Add force on Z to the disc

    }


}
