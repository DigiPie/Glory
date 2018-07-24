using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public StateSystem stateSystem;
    public AudioMixer audioMixer;
    // Setting Resolution

    public void SetResolution(int resolutionIndex)
    {
        switch (resolutionIndex)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(1366, 768, Screen.fullScreen);
                break;
            case 3:
                Screen.SetResolution(1280, 760, Screen.fullScreen);
                break;
            default:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
        }
    }

    // Tutorial Checkbox
    public void PlayTutorial(bool tutorialCheck)
    {
        stateSystem.EnableTutorial(tutorialCheck);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void SetSFXVolume(float volume)
    {
       
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
