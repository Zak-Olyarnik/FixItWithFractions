using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossFadeAudio : MonoBehaviour {

    private static CrossFadeAudio owner; /* The newest audioSource */
    [SerializeField] private float fadeDuration = 5f; /* How long to perform the crossFade */
    private Coroutine fadeIn; /* The coroutine fading in the current owner */

	void Start ()
    {
		/* A new audio source has spawned. Perform a cross fade */
        DontDestroyOnLoad(this.gameObject);
 
        

        /* Begin fading out the previous owner if one exists */
        if (owner != null)
        {
            owner.FadeOut();
        }

        /* Set this as the new owner and begin fading in */
        owner = this;
        AudioSource toPlay = this.GetComponent<AudioSource>();
        float endVolume = toPlay.volume;
        toPlay.volume = 0f;
        toPlay.Play();
        fadeIn = StartCoroutine(Fade(toPlay, 0f, endVolume, fadeDuration));
	}
	
    protected void FadeOut()
    {
        /* If still fading in, stop */
        if (!ReferenceEquals(fadeIn, null))
            StopCoroutine(fadeIn);

        AudioSource old = this.GetComponent<AudioSource>();
        StartCoroutine(Fade(old, old.volume, 0f, fadeDuration, true));

        if (owner == this)
            owner = null;
    }

    /// <summary>
    /// Adapted from https://forum.unity.com/threads/audiosource-cross-fade-component.443257/#post-3187561
    /// </summary>
    private IEnumerator Fade(AudioSource sourceToFade, float startVolume, float endVolume, float duration, bool destroyOnFinished = false)
    {
        float startTime = Time.time;

        while(sourceToFade.volume != endVolume)
        {
            float elapsedTime = Time.time - startTime;

            sourceToFade.volume = Mathf.Clamp01(Mathf.Lerp(startVolume, endVolume, elapsedTime / duration));

            yield return null;
        }

        if (destroyOnFinished)
            Destroy(this.gameObject);
    }
}
