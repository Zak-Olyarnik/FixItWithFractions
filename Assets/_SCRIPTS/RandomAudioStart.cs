using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioStart : MonoBehaviour {

	void Start ()
    {
        AudioSource source = GetComponent<AudioSource>();

        if (source != null)
        {
            /* Choose a random start point */
            float startTime = Random.Range(0f, source.clip.length);
            source.time = startTime;
        }
	}
}
