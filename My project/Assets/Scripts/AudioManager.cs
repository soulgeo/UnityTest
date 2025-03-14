using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // List of all sounds.
    public Sound[] sounds;

    // Audio Mixer and its snapshots
    public AudioMixer audioMixer;
    AudioMixerSnapshot playing;
    AudioMixerSnapshot paused;

    void Awake()
    {
        playing = audioMixer.FindSnapshot("Playing");
        paused = audioMixer.FindSnapshot("Paused");

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.group;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    /// <summary>
    /// Instantly sets the mixer to the Paused snapshot.
    /// </summary>
    public void setMixerToPaused()
    {
        paused.TransitionTo(0);
    }

    /// <summary>
    /// Transitions the mixer to the Paused snapshot over a period of time.
    /// </summary>
    /// <param name="time">The time it will take to reach the snapshot.</param>
    public void setMixerToPaused(float time)
    {
        paused.TransitionTo(time);
    }

    /// <summary>
    /// Instantly sets the mixer to the Playing snapshot.
    /// </summary>
    public void setMixerToPlaying()
    {
        playing.TransitionTo(0);
    }

    /// <summary>
    /// Transitions the mixer to the Playing snapshot over a period of time.
    /// </summary>
    /// <param name="time">The time it will take to reach the snapshot.</param>
    public void setMixerToPlaying(float time)
    {
        playing.TransitionTo(time);
    }

    /// <summary>
    /// Plays a sound, if it exists on the Audio Manager's list of sounds.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    public void Play(string name)
    {
        Sound s = FindSound(name);
        if (s != null)
            s.source.Play();
    }

    /// <summary>
    /// Checks if a sound is currently playing.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    /// <returns>True if playing, false if not.</returns>
    public bool IsPlaying(string name)
    {
        Sound s = FindSound(name);
        if (s != null)
            return s.source.isPlaying;
        return false;
    }

    /// <summary>
    /// Pauses a sound, if it exists on the Audio Manager's list of sounds.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    public void Pause(string name)
    {
        Sound s = FindSound(name);
        if (s != null)
            s.source.Pause();
    }

    /// <summary>
    /// Unpauses a sound, if it exists on the Audio Manager's list of sounds.
    /// </summary>
    /// <param name="name"></param>
    public void Resume(string name)
    {
        Sound s = FindSound(name);
        if (s != null)
            s.source.UnPause();
    }

    /// <summary>
    /// Stops a sound, if it exists on the Audio Manager's list of sounds.
    /// </summary>
    /// <param name="name"></param>
    public void Stop(string name)
    {
        Sound s = FindSound(name);
        if (s != null)
            s.source.Stop();
    }

    /// <summary>
    /// Plays a sound after some delay.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    /// <param name="time">Time of delay.</param>
    /// <returns></returns>
    public IEnumerator PlayWithDelay(string name, float time)
    {
        yield return new WaitForSeconds(time);
        Play(name);
    }

    /// <summary>
    /// Plays a sound at zero volume and linearly increases the volume to its original amount over time.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    /// <param name="fadeTime">The time it will take to reach original volume.</param>
    /// <returns></returns>
    public IEnumerator FadeIn(string name, float fadeTime)
    {
        Sound s = FindSound(name);
        if (s != null)
        {
            float endVolume = s.source.volume;
            s.source.volume = 0;
            s.source.Play();
            while (s.source.volume < endVolume)
            {
                s.source.volume += endVolume * Time.deltaTime / fadeTime;
                yield return null;
            }
            s.source.volume = endVolume;
        }
    }

    /// <summary>
    /// Linearly decreases the volume of a sound to zero, then stops the sound and sets its volume back up.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    /// <param name="fadeTime">The time it will take to reach zero volume.</param>
    /// <returns></returns>
    public IEnumerator FadeOut(string name, float fadeTime)
    {
        Sound s = FindSound(name);
        if (s != null)
        {
            float startVolume = s.source.volume;
            while (s.source.volume > 0)
            {
                s.source.volume -= startVolume * Time.deltaTime / fadeTime;
                yield return null;
            }
            s.source.Stop();
            s.source.volume = startVolume;
        }
    }

    /// <summary>
    /// Checks if a sound exists in the Audio Manager's list of sounds. Outputs a warning if it does not.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    /// <returns>True if it exists, false if not.</returns>
    Sound FindSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            Debug.LogWarning("[AudioManager] Sound of name '" + name + "' not found.");
        return s;
    }

    /// <summary>
    /// Converts a volume number to its corresponding decibel value.
    /// </summary>
    /// <param name="linear">Linear volume value between 0 and 1.</param>
    /// <returns>Value converted to decibel units.</returns>
    public static float LinearToDecibel(float linear)
    {
        float dB;

        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }

    /// <summary>
    /// Converts a volume value in decibel to its corresponding linear value.
    /// </summary>
    /// <param name="dB">Decibel value.</param>
    /// <returns>Value converted to a number between 0 and 1.</returns>
    public static float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20.0f);

        return linear;
    }

}
