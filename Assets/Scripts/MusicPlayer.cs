using UnityEngine;
using System.Collections;

public class MusicPlayer {
    public GameObject gameObj;
    public AudioSource source;
    public AudioClip activeClip;
    public bool readyToPlay;

	public MusicPlayer() {
        gameObj = new GameObject("Music Player");
        source = gameObj.AddComponent<AudioSource>();
    }
	
    public void Update()
    {
        if (readyToPlay && activeClip != null && activeClip.isReadyToPlay) {
            source.clip = activeClip;
            source.Play();
            activeClip = null;
            readyToPlay = false;
        }
    }

    public void PlayAudioClip(AudioClip clip)
    {
        activeClip = clip;
        readyToPlay = true;
    }
}
