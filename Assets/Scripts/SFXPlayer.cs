using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer {

    public GameObject sfxObj;
    public AudioSource metroSource;
    public AudioClip metroTick1;
    public AudioClip metroTick2;

    public AudioSource clickSource;
    public AudioClip hitSoundClick;
    public bool clickPlayedThisFrame;
    public int clickSoundCount;

    public AudioSource slideSource;
    public AudioClip hitSoundSlide;
    public bool slidePlayedThisFrame;
    public int slideSoundCount;

    public AudioSource releaseSource;
    public AudioClip hitSoundRelease;
    public bool releasePlayedThisFrame;

    public SFXPlayer()
    {
        sfxObj = new GameObject("Sound Player");
        metroSource = sfxObj.AddComponent<AudioSource>();
        clickSource = sfxObj.AddComponent<AudioSource>();
        slideSource = sfxObj.AddComponent<AudioSource>();
        releaseSource = sfxObj.AddComponent<AudioSource>();
        metroTick1 = VoezEditor.LoadSoundEffect("metroTick1");
        metroTick2 = VoezEditor.LoadSoundEffect("metroTick2");
        hitSoundClick = VoezEditor.LoadSoundEffect("hitSoundClick");
        hitSoundRelease = VoezEditor.LoadSoundEffect("hitSoundRelease");
        hitSoundSlide = VoezEditor.LoadSoundEffect("hitSoundSlide");
    }

    public void Update()
    {
        clickPlayedThisFrame = false;
        releasePlayedThisFrame = false;
        slidePlayedThisFrame = false;
    }

    public void ClickHitSound()
    {
        if (!clickPlayedThisFrame) {
            clickSource.volume = 0.6f;
            if (clickSoundCount % 30 < 15)
                clickSource.pitch = 0.75f + (clickSoundCount % 30) * 0.05f;
            else
                clickSource.pitch = 1.5f - ((clickSoundCount-15) % 30) * 0.05f;
            clickSource.PlayOneShot(hitSoundClick);
            clickSoundCount += 1;
        }
        clickPlayedThisFrame = true;
    }

    public void SlideHitSound()
    {
        if (!slidePlayedThisFrame) {
            slideSource.volume = 0.4f;
            if (slideSoundCount % 40 < 20)
                slideSource.pitch = 0.5f + (slideSoundCount % 40) * 0.05f;
            else
                slideSource.pitch = 1.5f - ((slideSoundCount - 20) % 40) * 0.05f;
            slideSource.PlayOneShot(hitSoundSlide);
            slideSoundCount += 1;
        }
        slidePlayedThisFrame = true;
    }

    public void ReleaseHitSound()
    {
        if (!releasePlayedThisFrame) {
            releaseSource.volume = 0.2f;
            releaseSource.pitch = Random.value * 0.2f + 0.9f;
            releaseSource.PlayOneShot(hitSoundRelease);
        }
        releasePlayedThisFrame = true;
    }
}
