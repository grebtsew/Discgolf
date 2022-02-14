using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Resolution : MonoBehaviour
{
    public Dropdown dropdown;

    public void UpdateResolution(){
        if (dropdown.itemText.text == "1920x1080"){
             Screen.SetResolution(1920, 1080, true);
        } else {
             Screen.SetResolution(3840, 2160, true);
        }
    }
}
