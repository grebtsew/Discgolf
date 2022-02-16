using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basket_script : MonoBehaviour
{

    public AudioClip[] audio;
    public float dampening = 100f;
    
    private SimulatorUIhelper disc;
    private Rigidbody rigidBody;
     private AudioSource _audioSource;

    // Use this for initialization
    void Start()
    {
        disc = FindObjectOfType<SimulatorUIhelper>();
        rigidBody = disc.getRigidbody();
        _audioSource = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "disc")
        {
            rigidBody.velocity = rigidBody.velocity/dampening;
            // Also trigger audio here!
            
            _audioSource.clip = audio[Random.Range(0, audio.Length)];
            _audioSource.Play();
        }
    }


}
