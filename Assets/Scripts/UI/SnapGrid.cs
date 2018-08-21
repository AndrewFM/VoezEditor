using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SnapGrid : UIElement {
    public float lastSnapGridSetting;
    public int lastBPM;
    public bool despawned;

    public SnapGrid() {
        lastSnapGridSetting = VoezEditor.Editor.selectedTimeSnap;
        lastBPM = VoezEditor.Editor.project.songBPM;
    }

    public override void Update() {
        base.Update();
    }

    public void SnapPlaytimeToGrid()
    {
        if (VoezEditor.Editor.selectedTimeSnap > 0) {
            float timeIncrement = 0;
            if (VoezEditor.Editor.project.songBPM > 0) {
                float secondsPerBeat = 60f / VoezEditor.Editor.project.songBPM;
                float framesPerBeat = secondsPerBeat * VoezEditor.Editor.framesPerSecond;
                timeIncrement = framesPerBeat / VoezEditor.Editor.selectedTimeSnap; // BPM data available; set time snap to match BPM
            } else
                timeIncrement = VoezEditor.Editor.framesPerSecond / VoezEditor.Editor.selectedTimeSnap; // No BPM data; treat time snap as beats per second -- ie: 60 BPM

            VoezEditor.Editor.currentFrame = (Mathf.Floor(VoezEditor.Editor.currentFrame / timeIncrement) * timeIncrement);
        }
    }

    public override void InitiateSprites(SpriteGroup sGroup) {
        int numVisible = 0;
        if (VoezEditor.Editor.selectedTimeSnap > 0) {
            numVisible = Mathf.CeilToInt(Note.NOTE_DURATION / (1f / VoezEditor.Editor.selectedTimeSnap));
            if (VoezEditor.Editor.project.songBPM > 0) {
                float secondsPerBeat = 60f / VoezEditor.Editor.project.songBPM;
                numVisible = Mathf.CeilToInt(Note.NOTE_DURATION / (secondsPerBeat / VoezEditor.Editor.selectedTimeSnap));
            }
        }
        sGroup.sprites = new FSprite[numVisible];

        for (int i = 0; i < sGroup.sprites.Length; i += 1) {
            sGroup.sprites[i] = new FSprite("Futile_White");
            sGroup.sprites[i].scaleX = VoezEditor.windowRes.x / sGroup.sprites[i].width;
            sGroup.sprites[i].scaleY = 2f / sGroup.sprites[i].height;
            sGroup.sprites[i].alpha = 0.2f;
        }
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress) {
        if (VoezEditor.Editor.selectedTimeSnap == 0 || !VoezEditor.Editor.EditMode) {
            foreach (FSprite fsprite in sGroup.sprites)
                fsprite.RemoveFromContainer();
            despawned = true;
        }
        else if (despawned) {
            ReInitiateSprites(sGroup);
            lastSnapGridSetting = VoezEditor.Editor.selectedTimeSnap;
            despawned = false;
        }

        if (!despawned)
        {
            if (lastSnapGridSetting != VoezEditor.Editor.selectedTimeSnap || lastBPM != VoezEditor.Editor.project.songBPM) {
                ReInitiateSprites(sGroup);
                lastSnapGridSetting = VoezEditor.Editor.selectedTimeSnap;
            }

            float timeIncrement = 0;
            if (VoezEditor.Editor.project.songBPM > 0) {
                float secondsPerBeat = 60f / VoezEditor.Editor.project.songBPM;
                timeIncrement = secondsPerBeat / VoezEditor.Editor.selectedTimeSnap; // BPM data available; set time snap to match BPM
            } else
                timeIncrement = 1f / VoezEditor.Editor.selectedTimeSnap; // No BPM data; treat time snap as beats per second -- ie: 60 BPM

            float offset = VoezEditor.Editor.songTime - (Mathf.Floor(VoezEditor.Editor.songTime / timeIncrement) * timeIncrement);
            float pixelsPerSecond = (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / Note.NOTE_DURATION;
            int numVisible = Mathf.CeilToInt(Note.NOTE_DURATION / timeIncrement);

            for (int i = 0; i < sGroup.sprites.Length; i += 1) {
                if (i >= numVisible)
                    sGroup.sprites[i].isVisible = false;
                else {
                    sGroup.sprites[i].isVisible = true;
                    sGroup.sprites[i].x = VoezEditor.windowRes.x * 0.5f;
                    sGroup.sprites[i].y = (VoezEditor.windowRes.y * (1f - Track.TRACK_SCREEN_HEIGHT)) - (offset * pixelsPerSecond) + (i * timeIncrement * pixelsPerSecond);
                }
            }
        }

        base.DrawSprites(sGroup, frameProgress);
    }
}
