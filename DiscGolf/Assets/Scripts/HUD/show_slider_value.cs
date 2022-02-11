using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class show_slider_value : MonoBehaviour
{

    private Text text;
    private Slider slider;

    void Start()
    {
        text = GetComponent<Text>();
        slider = GetComponentInParent<Slider>();

    }

    void Update()
    {
        text.text = Math.Round(slider.value).ToString();
    }
}
