using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;

    private static AudioManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    private void Start() {
        Play("Theme");
    }

    public void Play(string soundName) {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.source.Play();
    }

    public void Stop(string soundName) {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.source.Stop();
    }
}
