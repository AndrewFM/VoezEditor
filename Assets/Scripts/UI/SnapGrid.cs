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
            float timeIncrement = VoezEditor.Editor.GetBPMTimeIncrement()*VoezEditor.Editor.framesPerSecond;
            VoezEditor.Editor.currentFrame = (Mathf.Floor(VoezEditor.Editor.currentFrame / timeIncrement) * timeIncrement);
        }
    }

    public float GetSongTimeAtGridY(float y)
    {
        float timeIncrement = VoezEditor.Editor.GetBPMTimeIncrement();
        float offset = VoezEditor.Editor.songTime - (Mathf.Floor(VoezEditor.Editor.songTime / timeIncrement) * timeIncrement);
        float pixelsPerSecond = (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / Note.NOTE_DURATION;
        int numVisible = Mathf.CeilToInt(Note.NOTE_DURATION / timeIncrement);
        float closest = int.MaxValue;
        float songTimeAtClosest = 0;
        for (int i = 0; i < numVisible; i += 1) {
            float gridY = (VoezEditor.windowRes.y * (1f - Track.TRACK_SCREEN_HEIGHT)) - (offset * pixelsPerSecond) + (i * timeIncrement * pixelsPerSecond);
            if (Mathf.Abs(gridY - y) < closest) {
                closest = Mathf.Abs(gridY - y);
                songTimeAtClosest = VoezEditor.Editor.songTime + timeIncrement*i;
            }
        }
        return songTimeAtClosest;
    }

    public float SnapToGridY(float y)
    {
        float timeIncrement = VoezEditor.Editor.GetBPMTimeIncrement();
        float offset = VoezEditor.Editor.songTime - (Mathf.Floor(VoezEditor.Editor.songTime / timeIncrement) * timeIncrement);
        float pixelsPerSecond = (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / Note.NOTE_DURATION;
        int numVisible = Mathf.CeilToInt(Note.NOTE_DURATION / timeIncrement);
        float closest = int.MaxValue;
        float closestValue = 0;
        for (int i=0; i<numVisible; i+=1) {
            float gridY = (VoezEditor.windowRes.y * (1f - Track.TRACK_SCREEN_HEIGHT)) - (offset * pixelsPerSecond) + (i * timeIncrement * pixelsPerSecond);
            if (Mathf.Abs(gridY - y) < closest) {
                closest = Mathf.Abs(gridY - y);
                closestValue = gridY;
            }
        }
        return closestValue;
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

            float timeIncrement = VoezEditor.Editor.GetBPMTimeIncrement();
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

    public override void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
    {
        foreach (FSprite fsprite in sGroup.sprites) {
            fsprite.RemoveFromContainer();
            VoezEditor.Editor.gridContainer.AddChild(fsprite);
        }
    }
}
