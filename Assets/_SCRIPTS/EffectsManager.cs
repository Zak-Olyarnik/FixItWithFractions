using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EffectsManager : MonoBehaviour {

	private static EffectsManager instance;
	public enum Effects {
		// Visual
		Confetti,
        DustCloud,
		
		//Auditory
		Yay,
        Snap,
        Incorrect,
        CutTool,
        Correct,
        Construction
    };

	[SerializeField] private GameObject Confetti;
    [SerializeField] private GameObject DustCloud;
    [SerializeField] private AudioClip Yay;
    [SerializeField] private AudioClip Snap;
    [SerializeField] private AudioClip Correct;
    [SerializeField] private AudioClip Incorrect;
    [SerializeField] private AudioClip Construction;
    [SerializeField] private AudioClip CutTool;


	private AudioSource audioSource;

	public static EffectsManager Instance
	{
		get {return instance;}
	}

	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("Deleting " + this.name + " due to duplicate EffectsManager.");
			GameObject.Destroy(this.gameObject);
		}
		else
		{
			instance = this;
			Instance.audioSource = GetComponent<AudioSource>();
		}
	}

	public void PlayEffect(Effects effect)
	{
		switch (effect)
		{
			case Effects.Confetti:
				Instantiate(Confetti, this.transform, false); /* The Confetti particle system has autoDestroy */
                break;
            case Effects.DustCloud:
                Instantiate(DustCloud, this.transform, false); /* The Confetti particle system has autoDestroy */
                break;
            case Effects.Yay:
                audioSource.PlayOneShot(Yay);
                break;
            case Effects.Snap:
                audioSource.PlayOneShot(Snap);
                break;
            case Effects.Correct:
                audioSource.PlayOneShot(Correct);
                break;
            case Effects.Incorrect:
                audioSource.PlayOneShot(Incorrect);
                break;
            case Effects.Construction:
                audioSource.PlayOneShot(Construction);
                break;
            case Effects.CutTool:
                audioSource.PlayOneShot(CutTool);
                break;
			default:

			break;
		}
	}

    public void PlayEffect(Effects effect, Transform parent)
    {
        switch (effect)
        {
            case Effects.Confetti:
                Instantiate(Confetti, parent, false); /* The Confetti particle system has autoDestroy */
                break;
            case Effects.DustCloud:
                Instantiate(DustCloud, parent, false); /* The Confetti particle system has autoDestroy */
                break;
            case Effects.Yay:
                audioSource.PlayOneShot(Yay);
                break;
            case Effects.Snap:
                audioSource.PlayOneShot(Snap);
                break;
            case Effects.Correct:
                audioSource.PlayOneShot(Correct);
                break;
            case Effects.Incorrect:
                audioSource.PlayOneShot(Incorrect);
                break;
            case Effects.Construction:
                audioSource.PlayOneShot(Construction);
                break;
            case Effects.CutTool:
                audioSource.PlayOneShot(CutTool);
                break;
            default:

                break;
        }
    }
}
