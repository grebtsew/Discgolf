using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Listener : MonoBehaviour
{
    // Camera control
    public Text Selected_Camera_Text;
    public Text Selected_Camera_Mode_Text;

    

    public void NotifyAll(string camera_name, string camera_mode){
        Selected_Camera_Text.text = camera_name;
        Selected_Camera_Mode_Text.text = camera_mode;
    }

}
