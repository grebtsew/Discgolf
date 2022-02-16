using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscCollision : MonoBehaviour
{
   public AudioClip[] audio;
    private Rigidbody rigidBody;
     private AudioSource _audioSource;

    // Use this for initialization
    void Start()
    {
       
        _audioSource = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    {
       
            _audioSource.clip = audio[Random.Range(0, audio.Length)];
            _audioSource.Play();
        
    }
}
