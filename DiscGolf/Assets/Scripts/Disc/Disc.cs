using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum difficulty
{
    EASY,
    NEUTRAL,
    ADVANCED
}

public enum disctype
{
    PA,
    MD,
    FD,
    DD
}


public class Disc : MonoBehaviour {

     [Header("Disc Standard Attributes")]

    /* SPEED  1 - 15
     * Speed is the rate at which a disc can travel through the air.
     * Speed 14 Distance Drivers are the fastest, having the PDGA maximum legal wing width.
     * Faster discs cut into the wind with less effort and are best when throwing up wind.
     * Slower discs take more power to throw upwind, but they’re easier to throw more accurately
     * and may actually go farther downwind. High speed discs are not recommended for beginners
     * as they require more power to fly properly.
     */
     public float SPEED;

    /* GLIDE 1 - 7
     * Glide describes the discs ability to maintain loft during flight.
     * Discs with more glide are best for new players, and for producing maximum distance.
     * Beginners wanting more distance should choose discs with more glide.
     * Discs with less glide are more accurate in high wind situations.
     */
     public float GLIDE;

    /* TURN +2 - -5
     * High Speed Turn is the tendency of a disc to turn over or bank to the right (for RHBH throws)
     * during the initial part of the flight. A disc with a +1 rating is most resistant to turning over,
     * while a -5 rating will turn the most. Discs rated -3 to -5 make good roller discs.
     * Discs with less turn are more accurate in the wind. Discs with more turn are easier to throw for beginners.
     */
       public float TURN;

    /* FADE 0 - 5
     * Low Speed Fade is the discs tendency to hook left (for RHBH throws)
     * at the end of the flight. Fade is rated from 0 to 5. A disc rated 0 will finish straightest,
     * while a disc rated 5 will hook hard at the end of the flight. 
     * High fade discs are usually used for Spike and Skip shots.
     */
      public float FADE;     

    [Header("Shape")]
    
        public float diameter = 0.21f; // in m
        public float m; // in kg

        public float rim_thickness; // 
        public float rim_height; //
        public float center_height; //

    [Header("Specifics")]
        public string name;
        public bool approved;
        public difficulty difficulty = difficulty.NEUTRAL;
        public disctype disctype = disctype.MD;
        public string plastic;
        public string manufacturer;
        //public Texture texture; 

    [Header("Constants")]
        public float resistant_constant; // 
        public float lift_constant; //

    [Header("Time dependent")]
      
        public float health = 100;

    [Header("Advanced")]
    public double AREA = 0.0568; //The area of a standard frisbee.
    public double CL0 = 0.1; //The lift coefficient at alpha = 0.
    public double CLA = 1.4; //The lift coefficient dependent on alpha.
    public double CD0 = 0.08;  //The drag coefficent at alpha = 0.
    public double CDA = 2.72;  //The drag coefficient dependent on alpha.
    public double ALPHA0 = -4;

    void Start()
    {
        
        // Clamp values of flight parameters, max and min
        SPEED = Clamp(SPEED, 1, 15);
        GLIDE = Clamp(GLIDE, 1, 7);
        TURN = Clamp(TURN, -5, 2); 
        FADE = Clamp(FADE, 0, 5); 
    }

    public static float Clamp(float value, float min, float max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }




}


