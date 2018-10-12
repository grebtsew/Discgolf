using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;

public class Disc_Movement_old : MonoBehaviour
{
    // const
    public const double GRAVITY = -9.81; //The acceleration of gravity (m/s^2).
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
        cam = FindObjectOfType<follow_camera>();
        Disc = GetComponent<Disc>();
        basket = FindObjectOfType<basket_script>();
        rigidBody = GetComponent<Rigidbody>(); // Get Disc Rigidbody

        discInitialPosition = transform.position; // Get disc localposition related to Player
        discInitialRotation = transform.rotation; // Get disc localrotation

       

        ResetDisc();
    }

    /// Tracker Line methods
    /// 
    /// 
    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.startWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        Lines.Add(myLine);

    }
    void ClearLines()
    {
        foreach (GameObject go in Lines)
        {
            GameObject.Destroy(go);
        }
    }

    /// Publicly used methods
    /// 
    ///
    public void SetRotSpeed(float f)
    {
        if (!isThrown)
        {
            rotateSpeed = f;
        }
    }
    public void SetPower(float f)
    {
        if (!isThrown)
        {
            power = f;
        }
    }
    public void SetChangeHand()
    {
        if (!isThrown)
        {
            if (backhand)
            {
                backhand = false;
            }
            else
            {

                backhand = true;
            }

        }
    }
    public void SetShowLine()
    {
        ClearLines();
        if (show_lines)
        {
            show_lines = false;
        }
        else
        {
            show_lines = true;
        }
    }
    public void SetSpeed(float f)
    {
        throw_speed = f;
    }

    
    /// Update related methods
    /// 
    /// 
    void Update()
    {
       
        if (isThrown)
        {
           
           

               // flat flight physics
                Gravity_physics();
                Drag_physics();
            

            CheckLanding();
            
            // clamp speedlimit
            if (rigidBody.velocity.magnitude > MAXSPEED)
                rigidBody.velocity = rigidBody.velocity.normalized * MAXSPEED;

        } else if (throw_animation)
        {

           // Throw animation

            // add rotation flipp on throw here!

            throw_delta_acceleration *= throw_acceleration;
            transform.position += transform.forward * throw_delta_acceleration;

            if (Vector3.Distance(transform.position, throwpos-transform.forward*throw_arm_length) >= Vector3.Distance( throwpos, throwpos - transform.forward * throw_arm_length))
            {
                SetUpThrow();
               
                throw_animation = false;
            }
        }
    }
    private void Gravity_physics()
    {
        // gravity
        delta_lift = (1 - Math.Abs(sideDir.y)) * Disc.CL0 + Disc.CLA * Math.Sin((transform.localEulerAngles.z * Math.PI) / 180) * Math.PI / 180;
        locVel = transform.InverseTransformDirection(rigidBody.velocity);
        delta_gravity = RHO * Math.Pow(locVel.z, 2) * Disc.AREA * delta_lift / 2 / Disc.m + GRAVITY + glide;
        Physics.gravity = new Vector3(0, (float)delta_gravity, 0);
    }
    private void Drag_physics()
    {
        // drag
       //  old drag
        delta_speed_drag = Math.Pow((Math.Sin((transform.eulerAngles.z * Math.PI) / 180) - Disc.ALPHA0) * Math.PI / 180, 2);
        //   delta_resistance = (Disc.CD0 + Disc.CDA * delta_speed_drag)*(1/Disc.SPEED)*2;
        delta_resistance = (Disc.CD0 + Disc.CDA * delta_speed_drag);
        delta_drag = RHO * Math.Pow(locVel.z, 2) * Disc.AREA * delta_resistance * Time.deltaTime;
        rigidBody.drag = (float)delta_drag ;
       

       // double cd = Disc.CD0 + Disc.CDA * Math.Pow((Math.Sin((transform.eulerAngles.z - Disc.ALPHA0) * Math.PI/180, 2) * Math.PI / 180, 2);
     //   rigidBody.drag = (float)  Disc.RHO ;



    }
    void CheckLanding()
    {
        if(rigidBody.velocity.x <= 0 && rigidBody.velocity.y <= 0 && rigidBody.velocity.z <= 0)
        {
            distance.text = Math.Round(Vector3.Distance(discInitialPosition, transform.position)).ToString();
            delta_rotation_speed *= fast_loss;
        }
    }
    void FixedUpdate()
    {

        if (isThrown )
        {
            Fade_movement(plan_speed);

        }
        if (isRotate)
        {
           
            Rotate();
        }
        
    }
   
    private void Fade_movement(float speed)
    {
        // Fade at end of throw and also stabalization

        sideDir = Vector3.Cross(rigidBody.transform.up, rigidBody.velocity).normalized;

        if (sideDir.y <= 0.5f && sideDir.y >= -0.5f)
        {
            rigidBody.AddForce(new Vector3(fade_z_physics * speed * sideDir.y, 0, fade_x * speed * sideDir.y));

            
        } else
        {
            // to make span from 0 - 0.5 - 0
           if( sideDir.y > 0.5f)
            {
                
                rigidBody.AddForce(new Vector3(fade_z_physics * speed * (1 - sideDir.y), 0, fade_x * speed * (1 - sideDir.y)));
            }
            if (sideDir.y < -0.5f)
            {
              
                rigidBody.AddForce(new Vector3(fade_z_physics * speed * (-1 - sideDir.y), 0, fade_x * speed * (-1 - sideDir.y)));
            }
            
        }
    }

    /// Start Throw methods
    /// 
    /// 
    private void Set_up_force_x()
    {
        // Force side (Wind)
        force.x = Random.Range(50, 60) * playerThrust;
    }
    private void Set_up_force_y()
    {
        // Force up (xTilt)
        if (transform.rotation.x < 0) force.y = Random.Range(10, 20) + (playerThrust * 100);
        else if (playerThrust > 0 && playerThrust < 0.40f) force.y = Random.Range(0, 10) + (playerThrust * 100);
        else if (playerThrust >= 0.40f && playerThrust < 0.50f) force.y = Random.Range(10, 20) + (playerThrust * 100);
        else if (playerThrust >= 0.50f && playerThrust < 0.60f) force.y = Random.Range(20, 30) + (playerThrust * 100);
        else if (playerThrust >= 0.60f && playerThrust < 0.70f) force.y = Random.Range(30, 40) + (playerThrust * 100);
        else if (playerThrust >= 0.70f && playerThrust < 0.80f) force.y = Random.Range(40, 50) + (playerThrust * 100);
        else if (playerThrust >= 0.80f && playerThrust < 0.90f) force.y = Random.Range(50, 60) + (playerThrust * 100);

        // Force up (160 - 170)
        else force.y = Random.Range(70, 90) + (playerThrust * 100);
    }
    private void SetUpThrow()
    {
        // Added variables
        rot = 0;                                // reset rotation speed
        discInitialRotation = transform.rotation; // Get disc localrotation

        // movement dependent on y rotation as radians
        fade_x = (float)Math.Sin((transform.eulerAngles.y * Math.PI) / 180);
        fade_z = (float)Math.Cos((transform.eulerAngles.y * Math.PI) / 180);
        fade_z_physics = (float)Math.Cos(Math.PI + (transform.eulerAngles.y * Math.PI) / 180);

        number_throws++;                        // count throws
        fade_started = false;
        rigidBody.useGravity = true;            // activate gravity
        playerThrust = fast_loss;               // 30%

        // random forces
        Set_up_force_x();
        Set_up_force_y();

        // clear old lines
        if (show_lines)
        {
            ClearLines();
        }

      //  fade_speed = standard_fade_speed + 2*Disc.FADE;
        delta_rotation_speed = rotateSpeed * playerThrust; // rotation start
      
        /*
        stable_speed = Disc.SPEED;                             // speed to stabelize disc
        fade_rot_speed = min_fade_speed + Disc.FADE;           // when to start fade
        plan_speed = Disc.FADE*2 + glide_normalizer;         // fade, turn speed
        glide = Disc.GLIDE - glide_normalizer;
        */

        MAXSPEED = (throw_speed / 10) + min_speed;   // set speed

      //  turn_drag =  Disc.TURN * (MAXSPEED - min_speed)/10;

        rigidBody.isKinematic = false;          // Add gravity to the disc

        force.z = power;                        // set up z-force

        // Add force
        rigidBody.AddForce(transform.right * force.x); // Add force on X to the disc
        rigidBody.AddForce(transform.up * force.y); // Add force on Y to the disc
        rigidBody.AddForce(transform.forward * force.z); // Add force on Z to the disc
       
        isThrown = true; // Disc is thrown by player
        throw_mode = true;
        isRotate = true; // The disc will rotate
        at_tee = false;
    }
    public void ThrowDisc()
    {

        throw_delta_acceleration =throw_arm_length / MAXSPEED;
        throwpos = transform.position;
        transform.position -= transform.forward * throw_arm_length; 
        throw_animation = true;
        Throws.text = number_throws.ToString();
    }

    /// Rotation Methods
    /// 
    /// 
    private void Rotate()
    {

        // while rotating
       
                // reduce rotation
                delta_rotation_speed /= smooth_delta_loss; // % rot loss
                rot = delta_rotation_speed * Time.deltaTime;
            
            // rotation direction
            if (backhand)
            {

                transform.Rotate(Vector3.up, rot);
            }
            else
            {
                transform.Rotate(Vector3.up, -rot);
            }

      
        // handle stabelization and fade
        if (rigidBody.velocity.magnitude > fade_rot_speed && !fade_started)
            {
                
                Stabelize();
                Flipp_movement();

            }
            else
            {
            if (rigidBody.velocity.magnitude > 0)
            {
                fade_started = true;
            }
            
                Fade();
            }

        
     
    }
    private void Flipp_movement()
    {
        // turn dependent on disc TURN
        sideDir = Vector3.Cross(rigidBody.transform.up, rigidBody.velocity).normalized;
        rigidBody.AddForce(new Vector3(fade_z_physics * turn_drag * (1 - Math.Abs(sideDir.y)), 0, fade_x * turn_drag));
        turn_drag /= smooth_delta_turn_loss;

    }
    private void Stabelize()
    {
        // stabelise disc
        if (sideDir.y < 0.8f && sideDir.y > -0.8f)
        {

            if (rigidBody.rotation.x != 0 && rigidBody.rotation.y != 0)
            {
                rigidBody.MoveRotation(Quaternion.RotateTowards(rigidBody.rotation, Quaternion.Euler(0, transform.localEulerAngles.y, 0), stable_speed * Time.deltaTime));
              
            }

        }
    }
    private void Fade()
    {
        // fade - solution for all directions!
        if (backhand)
        {
            rigidBody.MoveRotation(Quaternion.RotateTowards(rigidBody.rotation, Quaternion.Euler(90 * fade_x, transform.rotation.y, 90 * fade_z), fade_speed * Time.deltaTime));
        }
        else
        {
            rigidBody.MoveRotation(Quaternion.RotateTowards(rigidBody.rotation, Quaternion.Euler(-90 * fade_x, transform.rotation.y, -90 * fade_z), fade_speed * Time.deltaTime));
        }
    }

    /// Collision
    /// 
    /// 
    void OnCollisionEnter(Collision collision)
    {
        

        if (sideDir.y > 0.5f && !backhand || sideDir.y < -0.5 && backhand)
        {
            Roller_physics();
        } else
        {
            Skip_physics();
        }
        
    }
    private void Skip_physics()
    {
        // add material
        // add more lift
        // rotation

      //  rigidBody.AddForce(rigidBody.velocity+ rigidBody.transform.up * rigidBody.velocity.magnitude);
    }

    private void Roller_physics()
    {
        // roller physics
       // make dynamic

            if (backhand)
            {
                rigidBody.AddForce(new Vector3(-sideDir.y * rot * ROLL_SPEED * fade_x, 0, -sideDir.y * rot * ROLL_SPEED * fade_z));
            } else
            {
                rigidBody.AddForce(new Vector3(sideDir.y * rot * ROLL_SPEED * fade_x, 0, sideDir.y * rot * ROLL_SPEED * fade_z));
            }
           
         //   delta_rotation_speed *= ROLL_DECREASE + 0.01f * Disc.TURN;
        
    }

    /// Reseting throw and game
    /// 
    /// 
    void ResetDisc()
    {
        isThrown = false; // Next throw
       
        isRotate = false; // The disc will not rotate
    }
    void NextThrow()
    {
        rigidBody.isKinematic = true; // Remove gravity to the disc
        rigidBody.useGravity = false;

    }
    public void Next_reset(Vector3 reset_pos)
    {
        if (show_lines)
        {
            DrawLine(transform.position, new Vector3(reset_pos.x, reset_pos.y + 2, reset_pos.z), Color.red);
        }

        isThrown = false;
        throw_mode = true;
        transform.position = new Vector3(reset_pos.x, reset_pos.y + 2, reset_pos.z);
        transform.LookAt(basket.transform);

        ResetDisc();
        NextThrow();

        // update camera
        cam.updatePosition();
        cam.mode = camera_mode.throw_mode;
    }
    public void Next()
    {
        if (isThrown)
        {
            throw_mode = true;

            current_throw_pos = Disc.transform.position;
            throwpositions.Add(current_throw_pos);
            Next_reset(current_throw_pos);
          
        }
    }
    public void Back()
    {
        if (isThrown)
        {

            Next_reset(current_throw_pos);
            return;
        }
        throw_mode = true;

        if (throwpositions.Count > 1)
        {
            throwpositions.Remove(current_throw_pos);
            current_throw_pos = throwpositions[throwpositions.Count - 1];


            Next_reset(current_throw_pos);
        }
        else
        {
            Reset();
        }

    }
    public void Reset()
    {
        if (show_lines)
        {
            DrawLine(transform.position, discInitialPosition, Color.red);
        }

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
