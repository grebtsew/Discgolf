using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableToggle : MonoBehaviour
{
 public GameObject extraEnable;
 
     public void DisableGameObject()
 {
      
      gameObject.SetActive( false ) ;
      if (extraEnable){
      extraEnable.SetActive(true);
      }
 }

 public void EnableGameObject()
 {
      gameObject.SetActive( true ) ;
      if(extraEnable){
      extraEnable.SetActive(true);
      }
 }

 public void toggle()
 {
      if( gameObject.activeSelf )
           DisableGameObject();
      else
           EnableGameObject();
 }


}
