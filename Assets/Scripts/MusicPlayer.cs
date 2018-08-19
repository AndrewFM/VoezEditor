﻿using UnityEngine;
using System.Collections;

public class MusicPlayer {
    public GameObject gameObj;
    public AudioSource source;
    public AudioClip activeClip;
    public bool readyToPlay;
    public bool paused;
    public bool hasStarted;
    public float playbackSpeed;

	public MusicPlayer() {
        gameObj = new GameObject("Music Player");
        source = gameObj.AddComponent<AudioSource>();
        playbackSpeed = 1.0f;
    }
	
    public bool Ready()
    {
        return readyToPlay && activeClip != null && activeClip.loadState == AudioDataLoadState.Loaded;
    }

    private void StartPlaying()
    {
        source.clip = activeClip;
        source.Play();
        hasStarted = true;
        activeClip = null;
        readyToPlay = false;
    }

    public void Update()
    {
        if (Ready() && !paused)
            StartPlaying();
        if (source.isPlaying)
            source.pitch = playbackSpeed;
    }

    public void PauseSong()
    {
        paused = true;
        if (source.isPlaying)
            source.Pause();
    }

    public void ResumeSong()
    {
        if (paused) {
            paused = false;
            if (hasStarted)
                source.UnPause();
        }
    }

    public void PlayAudioClip(AudioClip clip)
    {
        activeClip = clip;
        readyToPlay = true;
    }
}
