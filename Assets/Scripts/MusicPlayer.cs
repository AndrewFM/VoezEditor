using UnityEngine;
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
        gameObj.AddComponent<AudioListener>();
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

    public void PlayAudioClip(AudioClip clip)
    {
        activeClip = clip;
        readyToPlay = true;
    }
}
