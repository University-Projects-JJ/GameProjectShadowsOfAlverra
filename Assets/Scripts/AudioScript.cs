using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public GameObject walking;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        walking.GetComponent<AudioSource>().mute = true;
        
        if (Input.GetButton("Horizontal"))
        {
            walking.GetComponent<AudioSource>().mute = false;
        }
        
        if (Input.GetButton("Vertical"))
        {
            walking.GetComponent<AudioSource>().mute = false;
        }
    }
}
