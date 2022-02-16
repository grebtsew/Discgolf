using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscFlightAudio : MonoBehaviour
{
   public AudioClip audio;
    private AudioSource _audioSource;
    public Simulator sim;

    // Use this for initialization
    void Start()
    {
      
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = audio;
    }


    void Update()
    {
            if(sim.isThrown && sim.rigidBody.velocity.magnitude > 0.1f){
                
                _audioSource.volume = sim.rigidBody.velocity.magnitude;
                 
                _audioSource.Play();
            } else {
                _audioSource.Pause();
            }
    }
}
