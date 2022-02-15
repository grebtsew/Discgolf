using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class discInfoNotifier : MonoBehaviour
{
    public Disc[] discs;

    public Slider speed;
    public Slider glide;
    public Slider turn;
    public Slider fade;
    
    public Text name;
    public Text manufacturer;

    public int index = 0;
    public Carousel carousel;

    void Start(){
        
        name.text = discs[0].name;
        manufacturer.text = discs[0].manufacturer;

        speed.value = discs[0].SPEED;
        glide.value = discs[0].GLIDE;
        turn.value = discs[0].TURN;
        fade.value = discs[0].FADE;

    }

    public void notifyAll(){
        index = carousel.CurrentIndex;

        speed.value = discs[index].SPEED;
        glide.value = discs[index].GLIDE;
        turn.value = discs[index].TURN;
        fade.value = discs[index].FADE;

        name.text = discs[index].name;
        manufacturer.text = discs[index].manufacturer;
    }

}
