using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAnimationTrigger : MonoBehaviour {

    private Animator[] birdAnimators;

	// Use this for initialization
	void Start () {
        birdAnimators = GetComponentsInChildren<Animator>();

        /* Randomize the animation offset for the birds, so that they don't all do the same thing */
        foreach (Animator animator in birdAnimators)
        {
            float randomIdleStart = Random.Range(0f, animator.GetCurrentAnimatorStateInfo(0).length);
            animator.Play("Bird_idle", 0, randomIdleStart);
        }
	}
	
	void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "coaster")
            return;
        /* Trigger the fly animation on each bird */
        foreach (Animator animator in birdAnimators)
        {
            if (Random.Range(0, 10) > 3 || birdAnimators.Length > 1)
                animator.SetTrigger("Fly");
            else
                animator.SetTrigger("Smack");
            Destroy(gameObject, 6f);
        }
    }
}
