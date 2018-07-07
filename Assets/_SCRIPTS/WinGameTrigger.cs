using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        /* Notify GameController that the coaster hit the trigger */
        if (collider.tag == "coaster")
            GameController.Instance.EndGame(true);
    }
}
