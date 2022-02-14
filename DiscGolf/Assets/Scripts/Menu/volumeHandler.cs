using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumeHandler : MonoBehaviour
{
    public AudioSource[] audiosources;
    public Slider input;


    void Start(){
 
    if (audiosources.Length > 0){
        
     input.value = audiosources[0].volume ;  
    } else {
        input.value = AudioListener.volume ;
    }
    }

    public void updateVolume()
	{
         for( int index = 0 ; index < audiosources.Length ; ++index )
             {
            audiosources[index].volume = input.value;
             }
	}

     public void updateMasterVolume()
	{
         AudioListener.volume = input.value;
	}

     public void toogleMute()
	{
        if (input.value > 0){
             AudioListener.volume = 0;
             input.value = 0;
        } else {
             AudioListener.volume = 1f;
             input.value = 1;
        }
        
	}
}
