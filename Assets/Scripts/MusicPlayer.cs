using UnityEngine;
using System.Collections;

public class MusicPlayer {
    public GameObject gameObj;
    public AudioSource source;
    public AudioClip activeClip;
    public bool readyToPlay;
    public float playbackSpeed;

	public MusicPlayer() {
        gameObj = new GameObject("Music Player");
        source = gameObj.AddComponent<AudioSource>();
        playbackSpeed = 1.0f;
    }
	
    public void Update()
    {
        if (readyToPlay && activeClip != null && activeClip.loadState == AudioDataLoadState.Loaded) {
            source.clip = activeClip;
            source.Play();
            activeClip = null;
            readyToPlay = false;
        }
        if (source.isPlaying)
            source.pitch = playbackSpeed;

        // TODO: Debug Keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
            playbackSpeed = 1.0f;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            playbackSpeed = 0.5f;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            playbackSpeed = 0.25f;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            playbackSpeed = 2.0f;
    }

    public void PlayAudioClip(AudioClip clip)
    {
        activeClip = clip;
        readyToPlay = true;
    }
}
