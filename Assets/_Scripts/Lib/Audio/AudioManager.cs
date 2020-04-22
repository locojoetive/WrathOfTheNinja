using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
    public Sound[] themes;
    public Sound[] sounds;
    private void Awake()
    {
        foreach (Sound theme in themes)
        {
            theme.source = gameObject.AddComponent<AudioSource>();
            theme.source.clip = theme.clip;
            theme.source.pitch = theme.pitch;
            theme.source.volume = theme.volume;
            theme.source.loop = true;
            theme.isTheme = true;
        }
        
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.pitch = sound.pitch;
            sound.source.volume = sound.volume;
            sound.source.loop = false;
            sound.isTheme = false;
        }

    }

    public void PlaySound(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " was not found");
            return;
        }
        else if (s.source.isPlaying)
        {
            Debug.Log("Sound: " + name + "is already Playing");
            s.source.Stop();
        }
        // s.source.pitch = pitch;
        s.source.Play();
    }

    internal bool isPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " was not found");
            return false;
        }

        return s.source.isPlaying;
    }

    public void PlayTheme(string name)
    {
        Sound t = Array.Find(themes, theme => theme.name == name);
        if (t == null)
        {
            Debug.LogError("Theme: " + name + " was not found");
            return;
        }
        else if (t.source.isPlaying)
        {
            Debug.LogWarning("Theme: " + name + "is already Playing");
            return;
        }
        FadeOutOtherThemes();
        t.source.Play();
    }

    public void StopTheme()
    {
        Sound t = Array.Find(themes, theme => theme.source.isPlaying);
        if (t != null) 
            t.source.Stop();
    }

    private void FadeOutOtherThemes()
    {
        Sound currentTheme = Array.Find(themes, theme => theme.source.isPlaying);
        if (currentTheme == null)
        {
            Debug.LogWarning("No theme was played before, therfore no fading out");
            return;
        }
        if (!currentTheme.isFadingOut)
        {
            StartCoroutine(currentTheme.FadeOut());
        }
    }

    public void StopSound(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogError("Sound: " + name + " was not found");
            return;
        }
        sound.source.Stop();
    }

    public void StopAllSounds()
    {
        foreach (Sound sound in sounds)
        {
            if (!sound.isFadingOut)
            {
                StartCoroutine(sound.FadeOut());
            }
        }
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        StopAllSounds();
    }
}
