using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;

public class Disc_Movement : MonoBehaviour
{
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

    private double cl = 0;
 private double cd = 0;
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


    private void Gyroscope_physics(){

            // inertia

            float i = (float) (1/2 * Disc.m * Math.Pow((Disc.diameter/2), 2));
            rigidBody.AddRelativeTorque(new Vector3(rigidBody.angularVelocity.x , rigidBody.angularVelocity.y , rigidBody.angularVelocity.z ) * i);
        
    }

    private void Aerodynamic_physics(){
        //z = Roll - p
        float crr = 0.014f;
        float crp = -0.0055f;
        float temp = (crr*rigidBody.angularVelocity.y + crp*rigidBody.angularVelocity.z);
        double R = temp* 1/2 * RHO * Math.Pow(rigidBody.velocity.magnitude,2) * Disc.AREA * Disc.diameter;
        //x = Pitch  - q
        float CM0 = -0.08f;
        float CMA = 0.43f;
        float CMq = - 0.005f;
         temp = (CM0 + CMA*transform.eulerAngles.x + CMq*rigidBody.angularVelocity.x);
        double M =temp * 1/2 * RHO * Math.Pow(rigidBody.velocity.magnitude,2) * Disc.AREA * Disc.diameter;

        //y = spin down - r
        float CNR = -0.000034f;
        double N = (CNR*rigidBody.angularVelocity.y) * 1/2 * RHO * Math.Pow(rigidBody.velocity.magnitude,2) * Disc.AREA * Disc.diameter;
        
        

        rigidBody.AddRelativeTorque((float)R,(float)M,(float)N);
        
        Debug.Log(R + " " + M + " " + N);
    }
    
    /// Update related methods
    /// 
    /// 
    void Update()
    {
       
        if (isThrown)
        {
           

               // flat flight physics
                Lift_And_Gravity_physics();
                Drag_physics();
                Aerodynamic_physics();

            CheckLanding();
            
            // clamp speedlimit
        //    if (rigidBody.velocity.magnitude > MAXSPEED)
          //      rigidBody.velocity = rigidBody.velocity.normalized * MAXSPEED;

        } else
        
         if (throw_animation)
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


    private void Lift_And_Gravity_physics(){
        double deltavy = ( RHO * Math.Pow(rigidBody.velocity.magnitude, 2) * Disc.AREA * cl /2 /  Disc.m + GRAVITY)  ;
        rigidBody.AddForce(0, (float) deltavy, 0, ForceMode.Acceleration);
    }
  
    private void Drag_physics(){
        delta_drag = RHO * Math.Pow(rigidBody.velocity.magnitude, 2) * Disc.AREA * cd ;
        rigidBody.AddForce(-rigidBody.velocity.normalized * (float)delta_drag, ForceMode.Force);
    }
    void CheckLanding()
    {
        if(rigidBody.velocity.x <= 0 && rigidBody.velocity.y <= 0 && rigidBody.velocity.z <= 0)
        {
            distance.text = Math.Round(Vector3.Distance(discInitialPosition, transform.position)).ToString();
         //  delta_rotation_speed *= fast_loss;
        }
    }
 
   
    

    
  
    private void SetUpThrow()
    {
        // Added variables
        rot = 0;                                // reset rotation speed
        discInitialRotation = transform.rotation; // Get disc localrotation

        // movement dependent on y rotation as radians
      //  fade_x = (float)Math.Sin((transform.eulerAngles.y * Math.PI) / 180);
      //  fade_z = (float)Math.Cos((transform.eulerAngles.y * Math.PI) / 180);
       // fade_z_physics = (float)Math.Cos(Math.PI + (transform.eulerAngles.y * Math.PI) / 180);

       
        

        // clear old lines
        if (show_lines)
        {
            ClearLines();
        }

      //  fade_speed = standard_fade_speed + 2*Disc.FADE;
        delta_rotation_speed = rotateSpeed * playerThrust; // rotation start
      
     
        MAXSPEED = (throw_speed / 10) + min_speed;   // set speed

      //  turn_drag =  Disc.TURN * (MAXSPEED - min_speed)/10;

        rigidBody.isKinematic = false;          // Add gravity to the disc


        force.z = power;                        // set up z-force

        
        rigidBody.mass = Disc.m;        
        rigidBody.useGravity = false;

           // init
        cd = Disc.CD0 + Disc.CDA * Mathf.Pow((float)((-rigidBody.rotation.x - Disc.ALPHA0)*Math.PI / 180),2);
        cl = Disc.CL0 + Disc.CLA *-rigidBody.rotation.x *Math.PI / 180;
      
        Debug.Log(rigidBody.rotation.x);
     
        rigidBody.AddRelativeTorque(0,rotateSpeed,0);
       
      //  rigidBody.AddForce(transform.right * force.x); // Add force on X to the disc
      //  rigidBody.AddForce(transform.up * force.y); // Add force on Y to the disc
        rigidBody.AddForce(transform.forward * force.z, ForceMode.VelocityChange); // Add force on Z to the disc
       
     

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
