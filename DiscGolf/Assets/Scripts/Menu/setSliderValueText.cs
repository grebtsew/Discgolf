using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setSliderValueText : MonoBehaviour
{
    public Text text;
    public Slider slider;
    public string unit;

    void Start(){
       updateText();
    }

   public void updateText(){
       text.text = slider.value.ToString() + " "+ unit;
   }
}
