using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioInputReciever : MonoBehaviour {

    private AudioSource realTimeInput;

	// Use this for initialization
	void Start ()
	{
	    realTimeInput = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
