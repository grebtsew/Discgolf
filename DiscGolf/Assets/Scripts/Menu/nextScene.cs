using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class nextScene : MonoBehaviour
{
 public string next;
 private string selected= "";


  public void LoadNextScene(){
       SceneManager.LoadScene(next);
  }

public void setSelected(string select){
    selected = select;
}

public void LoadSelected(){
     if (selected != ""){
     SceneManager.LoadScene(selected);
     }
    }

}
