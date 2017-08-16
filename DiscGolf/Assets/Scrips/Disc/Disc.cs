using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour {

    
    // Test with values, will later be polymorphism root
    public double m = 0.175; //The mass of a standard frisbee in kilograms.
    public double AREA = 0.0568; //The area of a standard frisbee.
    public static double CL0 = 0.1; //The lift coefficient at alpha = 0.
    public static double CLA = 1.4; //The lift coefficient dependent on alpha.
    public static double CD0 = 0.08;  //The drag coefficent at alpha = 0.
    public static double CDA = 2.72;  //The drag coefficient dependent on alpha.
    public static double ALPHA0 = -4;

    public float SPEED;     // 1 - 14 -> speed of disc, accuracy and need more speed
    public float GLIDE;     // 1 - 7 -> longer throws, in balance
    public float TURN;      // +1 - -5 -> turn not fade path in begining of throw
    public float FADE;      // 0 - 5 -> fade amount at end

    void Start()
    {
        SPEED = Clamp(SPEED, 1, 14);
        GLIDE = Clamp(GLIDE, 1, 7);
        TURN = Clamp(TURN, -5, 1); 
        FADE = Clamp(FADE, 0, 5); 
    }

    public static float Clamp(float value, float min, float max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }
}
