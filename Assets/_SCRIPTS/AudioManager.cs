using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	private static AudioManager instance;
	[SerializeField] AudioMixer mixer;

	#region Audio Mixer Exposed Parameters
	private const string backgroundVolume = "BackgroundVolume";
	private const string effectsVolume = "EffectsVolume";
	private const string masterVolume = "MasterVolume";
	private const string backgroundPitch = "BackgroundPitch"; /* TODO: Change this based on the coaster's speed(?) */
	#endregion /* Audio Mixer Exposed Parameters */

	public static AudioManager Instance {
		get {return instance;}
	}

	void Awake()
	{
		if (Instance != null)
		{
			/* TODO: This code executes when a new scene is loaded. Is there anything the AudioManager needs to do OnSceneLoad?*/

			Debug.LogWarning("A second AudioManager exists. Destroying its gameObject (" + this.name + ")");
			Destroy(this.gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject); /* Persist this gameObject */
		}
	}

	void Start()
	{
		/* Initialize the AudioMixer parameters */
		UpdateAudioMixer();
		
	}

	public void UpdateAudioMixer()
	{
		mixer.SetFloat(backgroundVolume, Constants.backgroundVolume);
		mixer.SetFloat(effectsVolume, Constants.effectsVolume);
		mixer.SetFloat(masterVolume, Constants.masterVolume);
		mixer.SetFloat(backgroundPitch, Constants.backgroundPitch);
	}
}
