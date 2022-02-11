using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Disc_Material {
      public PLASTIC plastic;
    
      private float sustainability;
      private float friction;
      private float glossiness;
      public Color color;

      private Material material;

       void Start()
    {
        
        switch(plastic){
            case PLASTIC.DX:
            sustainability = 20;
            friction = 20;
            glossiness = 0;
            break;
            case PLASTIC.Champion:
            sustainability = 50;
            friction = 10;
            glossiness = 50;
            break;
            case PLASTIC.Star:
            sustainability = 90;
            friction = 1;
            glossiness = 100;
            break;
        }
    
    }

    public static float Clamp(float value, float min, float max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }


}


