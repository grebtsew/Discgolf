using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;

public class Disc_Movement : MonoBehaviour
{
    public static double gravity = -9.81; //The acceleration of gravity (m/s^2).
    public static double RHO = 1.23; //The density of air in kg/m^3.

    public Rigidbody rigidBody;
    private Disc Disc; // The Frisbee
    private basket_script basket; // an obligatory basket
    private disc_mode mode = disc_mode.GROUNDED;

    public Vector3 discNextPosition; // Next throw position
    public Vector3 current_throw_pos; // current throw pos
    public Vector3 force; // The force applayed on XYZ
    public Vector3 discInitialPosition; // Disc initial position 
    private Quaternion discInitialRotation; // Disc initial rotation 

    private float playerThrust; // Get thrust in percent 
    public float maxSpeed = 30f; // Max speed the disc can have
    public float rotateSpeed = 1000f; // initial speed
    public float power = 1000f; // initial power
    private float fade_rot_speed = 15f; // fade physics speed
    private float stable_speed = 5f; //  stable speed
    private float fade_speed = 10f; // fade speed
    private double delta_lift; // lift force
    private double delta_gravity; // total gravity 
    private double delta_resistance; // drag resistance
    private double delta_drag; // drag
    private float delta_rotation_speed;
    private float plan_speed = 5; // fade speed
    private float rot = 0;
    private float fade_x = 0; // fade x cos value
    private float fade_z = 0; // fade z cos value
    private float fade_z_physics = 0; // rotated 180
    private float min_speed = 20f;

    private int number_throws = 0;
    public Text distance; // distance text
    public Text Throws; // throw count text

    public bool isThrown = false; // Is the disc in the air
    private bool isGrounded = false; // Is the disc on the ground
    private bool isRotate = false; // If the disc in the air it rotates
    private bool hitStuff = false; // extra collision check
    public bool throw_mode = true; // is throw mode
    public bool backhand = true; // fore or backhand

    private Vector3 sideDir; // fade direction
    private Vector3 locVel; // local velocity
    private follow_camera cam; // camera
    private List<Vector3> throwpositions = new List<Vector3>(); // all throw positions
    private List<GameObject> Lines = new List<GameObject>();
    private bool show_lines = false;
    public bool at_tee = true;
    private float turn_drag;
    private static float turn_boost = 10;
    private static float smooth_delta_loss = 1.001f;
    private static float smooth_delta_turn_loss = 1.004f;
    public float throw_speed = 100; // throw speed 0-150
    private float glide = 0;

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
    public void setShowLine()
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
        // Math.Sin((transform.eulerAngles.z * Math.PI) / 180);
        //  Debug.Log();
        if (isThrown)
        {
            // hit terrain
            if (!hitStuff)
            {

                gravity_physics();
                drag_physics();

            }
            CheckLanding();

            if (rigidBody.velocity.magnitude > maxSpeed)
                rigidBody.velocity = rigidBody.velocity.normalized * maxSpeed;
        }
    }
    private void gravity_physics()
    {
        if (sideDir.y < 0.8f && sideDir.y > -0.8)
        {
            delta_lift = Disc.CL0 + Disc.CLA * Math.Sin((transform.localEulerAngles.x * Math.PI) / 180) * Math.PI / 180;
        }
        else
        {
            delta_lift = 0;
        }
        // gravity
        locVel = transform.InverseTransformDirection(rigidBody.velocity);
        delta_gravity = RHO * Math.Pow(locVel.z, 2) * Disc.AREA * delta_lift / 2 / Disc.m + gravity + glide;
        Physics.gravity = new Vector3(0.0f, (float)delta_gravity, 0.0f);
    }
    private void drag_physics()
    {
        // drag
        delta_resistance = Disc.CD0 + Disc.CDA * Math.Pow((Math.Sin((transform.eulerAngles.x * Math.PI) / 180) - Disc.ALPHA0) * Math.PI / 180, 2);
        delta_drag = RHO * Math.Pow(locVel.z, 2) * Disc.AREA * delta_resistance * Time.deltaTime;
        rigidBody.drag = (float)delta_drag;
    }
    void CheckLanding()
    {
        if (rigidBody.IsSleeping() && isGrounded && isThrown)
        {

            NextThrow();
            distance.text = Math.Round(Vector3.Distance(discInitialPosition, transform.position)).ToString();
        }
    }
    void FixedUpdate()
    {

        if (isThrown && !isGrounded)
        {
            fade_physics();

        }
        if (isRotate && !isGrounded)
        {
            // Rotate the disc
            Rotate();
        }
        Throws.text = number_throws.ToString();
    }
    private void fade_physics()
    {
        /*
        Fade movement  with fix for all directions
        Flipp movement
        */

        fade_movement(plan_speed);


    }

    private void flipp_movement()
    {


        sideDir = Vector3.Cross(rigidBody.transform.up, rigidBody.velocity).normalized;
        rigidBody.AddForce(new Vector3(fade_z_physics * turn_drag, 0, fade_x * turn_drag));
        turn_drag /= smooth_delta_turn_loss;

    }
    private void fade_movement(float speed)
    {
        sideDir = Vector3.Cross(rigidBody.transform.up, rigidBody.velocity).normalized;
        if (sideDir.y < 0.7f && sideDir.y > -0.7f)
        {
            rigidBody.AddForce(new Vector3(fade_z_physics * speed * sideDir.y, 0, fade_x * speed * sideDir.y));

            if (sideDir.y < 0.1f && sideDir.y > -0.1f)
            {

                flipp_movement();
            }
        }
    }



    /// Start Throw methods
    /// 
    /// 
    private void set_up_force_x()
    {
        // Force side (Wind)
        force.x = Random.Range(50, 60) * playerThrust;
    }
    private void set_up_force_y()
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
    public void ThrowDisc()
    {
        // Added variables
        delta_rotation_speed = 0;
        rot = 0;                                // reset rotation speed
        discInitialRotation = transform.rotation; // Get disc localrotation

        glide = Disc.GLIDE - 5;

        // movement dependent on y rotation as radians
        fade_x = (float)Math.Sin((transform.eulerAngles.y * Math.PI) / 180);
        fade_z = (float)Math.Cos((transform.eulerAngles.y * Math.PI) / 180);
        fade_z_physics = (float)Math.Cos(Math.PI + (transform.eulerAngles.y * Math.PI) / 180);

        number_throws++;                        // count throws
        hitStuff = false;                       // hit terrain reset
        rigidBody.useGravity = true;            // activate gravity
        playerThrust = 0.3f;                    // 30%

        set_up_force_x();
        set_up_force_y();

        if (show_lines)
        {
            ClearLines();
        }
        fade_speed = 10f;
        delta_rotation_speed = rotateSpeed * playerThrust; // rotation start

        if (backhand)
        {
            turn_drag = Disc.TURN;
        }
        else
        {
            turn_drag = -Disc.TURN;
        }

        fade_rot_speed = 15f + Disc.FADE;
        plan_speed = Disc.FADE+ 5f;

        if (throw_speed / 10 > Disc.SPEED)
        {
            fade_speed += (throw_speed / (Disc.SPEED * 10)) * 10f;
              plan_speed += (throw_speed / (Disc.SPEED * 10)) * 8f;
            turn_drag *= -(throw_speed / (Disc.SPEED * 10))*3;
            fade_rot_speed += (throw_speed / (Disc.SPEED * 10)) * 100;
        }

        maxSpeed = (throw_speed / (Disc.SPEED * 10)) * Disc.SPEED + min_speed;   // set speed


        force.z = power;                        // set up z-force
        

        rigidBody.isKinematic = false; // Add gravity to the disc
       

        // Add force
        rigidBody.AddForce(transform.right * force.x); // Add force on X to the disc
        rigidBody.AddForce(transform.up * force.y); // Add force on Y to the disc
        rigidBody.AddForce(transform.forward * force.z); // Add force on Z to the disc

        isThrown = true; // Disc is thrown by player
        isRotate = true; // The disc will rotate
        throw_mode = false;
        at_tee = false;
    }

    /// Rotation Methods
    /// 
    /// 
    private void Rotate()
    {

        // while rotating
        if (delta_rotation_speed > 0)
        {

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
            if (rot > fade_rot_speed)
            {

                stabelize();
            }
            else
            {
                mode = disc_mode.FADE;
                fade();
            }

        }
        else
        {
            rotstopped();
        }
    }

    private void rotstopped()
    {


        hitStuff = true;
        Physics.gravity = new Vector3(0.0f, -9.81f, 0.0f);  // Reset Gravity. Nicer look when disc hits things and fall down

    }
    private void stabelize()
    {
        // stabelise disc
        if (sideDir.y < 0.8f && sideDir.y > -0.8f)
        {

            if (rigidBody.rotation.x != 0 && rigidBody.rotation.y != 0)
            {
                rigidBody.MoveRotation(Quaternion.RotateTowards(rigidBody.rotation, Quaternion.Euler(0, transform.localEulerAngles.y, 0), stable_speed * Time.deltaTime));
                mode = disc_mode.STABELIZE;
            }

        }
    }
    private void fade()
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
        if (collision.gameObject.name.Contains("Terrain"))
        {
            isGrounded = true;
            mode = disc_mode.GROUNDED;
        }
    }


    /// Reseting throw and game
    /// 
    /// 
    void ResetDisc()
    {
        isThrown = false; // Next throw
        isGrounded = false; // Not on ground
        isRotate = false; // The disc will not rotate


    }
    void NextThrow()
    {
        rigidBody.isKinematic = true; // Remove gravity to the disc
        rigidBody.useGravity = false;

    }
    public void next_reset(Vector3 reset_pos)
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
    public void next()
    {
        if (isThrown)
        {
            throw_mode = true;

            current_throw_pos = Disc.transform.position;
            throwpositions.Add(current_throw_pos);
            next_reset(current_throw_pos);
        }
    }
    public void back()
    {
        if (isThrown)
        {

            next_reset(current_throw_pos);
            return;
        }
        throw_mode = true;

        if (throwpositions.Count > 1)
        {
            throwpositions.Remove(current_throw_pos);
            current_throw_pos = throwpositions[throwpositions.Count - 1];


            next_reset(current_throw_pos);
        }
        else
        {
            reset();
        }

    }
    public void reset()
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
