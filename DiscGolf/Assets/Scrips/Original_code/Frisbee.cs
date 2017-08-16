using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Frisbee : MonoBehaviour {

    private static double x;
    //The x position of the frisbee.
    private static double y;
    //The y position of the frisbee.
    private static double vx;
    //The x velocity of the frisbee.
    private static double vy;
    //The y velocity of the frisbee.
    private static double g = -9.81;

    //The acceleration of gravity (m/s^2).
    private static double m = 0.175;
    //The mass of a standard frisbee in kilograms.
    private static double RHO = 1.23;
    //The density of air in kg/m^3.
    private static double AREA = 0.0568;
    //The area of a standard frisbee.
    private static double CL0 = 0.1;
    //The lift coefficient at alpha = 0.
    private static double CLA = 1.4;
    //The lift coefficient dependent on alpha.
    private static double CD0 = 0.08;
    //The drag coefficent at alpha = 0.
    private static double CDA = 2.72;
    //The drag coefficient dependent on alpha.
    private static double ALPHA0 = -4;

    private bool kast = false;
    

    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (kast) { 
        simulate( 20, 0, 1, Time.deltaTime);
        }

        
    }

    void OnCollisionEnter(Collision collision)
    {
        kast = false;
    }

    public void kasta()
    {
        kast = true;
    }


    /**
    * A method that uses Euler’s method to simulate the flight of a frisbee in
    * two dimensions, distance and height (x and y, respectively).
    *
        */
    public void simulate( double vx0, double vy0,
    double alpha, double deltaT)

    {
        //Calculation of the lift coefficient using the relationship given
        //by S. A. Hummel.
        double cl = CL0 + CLA * alpha * Math.PI / 180;
        //Calculation of the drag coefficient (for Prantl’s relationship)
        //using the relationship given by S. A. Hummel.
        double cd = CD0 + CDA * Math.Pow((alpha - ALPHA0) * Math.PI / 180, 2);
        //Initial position x = 0.
        x = transform.position.x;
        //Initial position y = y0.
        y = transform.position.y;
        //Initial x velocity vx = vx0.
        vx = vx0;
        //Initial y velocity vy = vy0.
        vy = vy0;

            //A loop index to monitor the simulation steps.
            int k = 0;
            //A while loop that performs iterations until the y position
            //reaches zero (i.e. the frisbee hits the ground).
            
                //The change in velocity in the y direction obtained setting the
                //net force equal to the sum of the gravitational force and the
                //lift force and solving for delta v.
                double deltavy = (RHO * Math.Pow(vx, 2) * AREA * cl / 2 / m + g) * deltaT;
                //The change in velocity in the x direction, obtained by
                //solving the force equation for delta v. (The only force
                //present is the drag force).
                double deltavx = -RHO * Math.Pow(vx, 2) * AREA * cd * deltaT;
                //The new positions and velocities are calculated using
                //simple introductory mechanics.
                vx = vx + deltavx;
                vy = vy + deltavy;
                x = x + vx * deltaT;
                y = y + vy * deltaT;

         

            transform.position = new Vector3(ToSingle(x), ToSingle(y), transform.position.z);
                

        }

    public static float ToSingle(double value)
    {
        return (float)value;
    }

}
