using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that is a singleton for audio management.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;                        // Instance of the AudioManager class.

    [Header("UI Settings: ")]
    public AudioSource userInterfaceAudioSource;                // UI AudioSource.
    [Range(0, 1)] public float userInterfaceVolume = 1.0f;      // The volume settings for the UI AudioSource.

    [Header("Background Music Settings: ")]
    public AudioSource backgroundMusicAudioSource;              // Background Music AudioSource.
    public AudioClip backGroundMusicAudioClip;                  // The main audio clip for the Background Music. (might turn into array...)
    [Range(0, 1)] public float backgroundMusicVolume = 1.0f;    // The volume settings for the Background Music AudioSource.

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;    // Sets the instance of the class.
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        SetAudioSourceSettings();                       // Sets the AudioSource settings.
        PlayBackgroundMusic(backGroundMusicAudioClip);  // Play the backgroundMusicAudioClip at the start of the scene.
    }
    
    /// <summary>
    /// A function to call the UI AudioSource to play a UI audioclip.
    /// </summary>
    public void PlayUserInterfaceSound(AudioClip clip)
    {
        userInterfaceAudioSource.PlayOneShot(clip);     // Play UI audioclip
    }

    /// <summary>
    /// A function to call the Background Music AudioSource to play a Background Music audioclip.
    /// </summary>
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip)
        {
            backgroundMusicAudioSource.Stop();      // Stop the previous music track.
            backgroundMusicAudioSource.clip = clip; // Play Background Music audioclip.
            backgroundMusicAudioSource.Play();      // Play the current music track.
        }
    }

    /// <summary>
    /// A function that sets the volume for all current audio sources.
    /// </summary>
    private void SetAudioSourceSettings()
    {
        userInterfaceAudioSource.volume = userInterfaceVolume;      // Set UI volume.
        backgroundMusicAudioSource.volume = backgroundMusicVolume;  // Set Background Music Volume.
    }
}
