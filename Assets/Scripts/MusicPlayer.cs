using UnityEngine;
using System.Collections;

public class MusicPlayer {
    public GameObject gameObj;
    public AudioSource source;
    public AudioClip activeClip;
    public string xfadePath;
    public bool readyToPlay;
    public bool paused;
    public bool hasStarted;
    public float playbackSpeed;
    public float desiredVolume;
    public bool fadeToDesired;
    public bool loopMode;
    public static float XFADE_RATE = 0.15f;

	public MusicPlayer() {
        gameObj = new GameObject("Music Player");
        gameObj.AddComponent<AudioListener>();
        source = gameObj.AddComponent<AudioSource>();
        playbackSpeed = 1.0f;
        desiredVolume = 1f;
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

        if (loopMode && !source.isPlaying && source.clip != null)
            source.Play();

        if (xfadePath != null) {
            if (!source.isPlaying)
                source.volume = 0f;
            else
                source.volume = Mathf.Lerp(source.volume, 0f, XFADE_RATE);
            if (source.volume <= 0.01f) {
                PlayAudioClip(xfadePath);
                fadeToDesired = true;
                xfadePath = null;
            }
        } else {
            if (fadeToDesired) {
                source.volume = Mathf.Lerp(source.volume, desiredVolume, XFADE_RATE);
                if (Mathf.Abs(source.volume - desiredVolume) <= 0.01f)
                    fadeToDesired = false;
            } else
                source.volume = desiredVolume;
        }
    }

    // Tell the music player what time in the song it's supposed to be at, and if there's a mismatch, resync the audio
    public void SyncTracker(float seconds)
    {
        if (source.isPlaying && Mathf.Abs(source.time-seconds) > ((1f/VoezEditor.Editor.framesPerSecond)*VoezEditor.musicSyncThreshold))
            source.time = seconds;
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

    public void Unload()
    {
        if (source.isPlaying) {
            source.Stop();
            hasStarted = false;
        }
        if (activeClip != null) {
            Object.DestroyImmediate(activeClip);
            activeClip = null;
            readyToPlay = false;
        }
        if (source.clip != null)
            Object.DestroyImmediate(source.clip);
    }

    public void Destroy()
    {
        Unload();
        Object.Destroy(gameObj);
    }

    public void PlayAudioClip(string filePath)
    {
        Unload();
        WWW www = new WWW(filePath);
        AudioClip songClip = www.GetAudioClip(false, false, AudioType.WAV);
        activeClip = songClip;
        readyToPlay = true;
    }

    public void CrossfadeIntoClip(string filePath)
    {
        if (filePath == null || filePath == "") {
            desiredVolume = 0f;
            fadeToDesired = true;
        } else {
            desiredVolume = 1f;
            xfadePath = filePath;
        }
    }
}
