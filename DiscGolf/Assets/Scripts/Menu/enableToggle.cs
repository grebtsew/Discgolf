using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableToggle : MonoBehaviour
{
 public GameObject extraEnable;
 
     public void DisableGameObject()
 {
      
      gameObject.SetActive( false ) ;
      extraEnable.SetActive(true);
 }

 public void EnableGameObject()
 {
      gameObject.SetActive( true ) ;
      extraEnable.SetActive(true);
 }

 public void toggle()
 {
      if( gameObject.activeSelf )
           DisableGameObject();
      else
           EnableGameObject();
 }


}
