using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class updateHand : MonoBehaviour
{

    private Text text;
    private SimulatorUIhelper disc;

    void Start()
    {
        text = GetComponent<Text>();
        disc = FindObjectOfType<SimulatorUIhelper>();


    }



}
