using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class AudioManager : MonoBehaviour {

    public Slider sliderBGM, sliderSFX;
    public int volumeBGM, volumeSFX;

    public Sound[] sounds;

    public static AudioManager instance;


    // Use this for initialization
    void Awake () {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
		foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent <AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.loop = s.loop;
        }
	}

    private void Start()
    {
        sliderBGM.value = volumeBGM;
        sliderSFX.value = volumeSFX;
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        // Warning if sound does not exist
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
}
