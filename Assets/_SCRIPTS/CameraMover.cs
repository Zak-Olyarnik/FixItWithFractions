using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "cart (1)")
        {
            if(Camera.main.GetComponent<TrackObject>().paused)
                /* The coaster entered this trigger, so tell the camera to start following it */
                Camera.main.GetComponent<TrackObject>().ResumeTracking();
            else
                /* The coaster entered this trigger, so tell the camera to stop following it */
                Camera.main.GetComponent<TrackObject>().PauseTracking();
        }
    }
}
