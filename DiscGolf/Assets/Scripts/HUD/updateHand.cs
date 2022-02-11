using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class updateHand : MonoBehaviour
{

    private Text text;
    private Disc_Movement disc;

    void Start()
    {
        text = GetComponent<Text>();
        disc = FindObjectOfType<Disc_Movement>();


    }



    // Update is called once per frame
    void Update()
    {
        /*
        if (disc.backhand)
        {
            text.text = "B";
        }
        else
        {
            text.text = "F";
        }
         */
    }
}
