using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackAddPreview : UIElement {
    public float previewX = -1f;
    public float previewScale = -1f;
    public float baseWidth;

    public TrackAddPreview()
    {
    }

    public override void Update()
    {
        base.Update();
        float posPerc = Util.InvScreenPosX(Input.mousePosition.x);
        if (previewX >= 0)
            posPerc = previewX;
        pos.x = Util.ScreenPosX(posPerc);
        pos.y = VoezEditor.windowRes.y * (1.0f - Track.TRACK_SCREEN_HEIGHT);

        // Add Track
        if (Util.ShiftDown() && Input.GetMouseButtonUp(0) && VoezEditor.Editor.trackEditMode && !VoezEditor.Editor.MenuOpen && VoezEditor.Editor.EditMode) {
            ProjectData.TrackData newTrack = new ProjectData.TrackData();
            newTrack.id = VoezEditor.Editor.GetUniqueTempNoteID();
            newTrack.start = VoezEditor.Editor.songTime;

            float timeIncrement = VoezEditor.Editor.GetBPMTimeIncrement();
            newTrack.end = Mathf.Clamp(VoezEditor.Editor.songTime + timeIncrement * 10f, 0f, VoezEditor.Editor.musicPlayer.source.clip.length);
            newTrack.size = 1f;
            newTrack.x = posPerc;
            VoezEditor.Editor.project.AddTrack(newTrack);
            VoezEditor.Editor.RefreshAllTracks();
        }
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[1];
        sGroup.sprites[0] = new FSprite("trackGradient");
        baseWidth = sGroup.sprites[0].width;
        sGroup.sprites[0].scaleX = (VoezEditor.windowRes.x / sGroup.sprites[0].width) * Track.TRACK_SCREEN_WIDTH;
        sGroup.sprites[0].scaleY = (VoezEditor.windowRes.y / sGroup.sprites[0].height) * Track.TRACK_SCREEN_HEIGHT;
        sGroup.sprites[0].color = ProjectData.colors[0];
        sGroup.sprites[0].anchorY = 0f;
        sGroup.sprites[0].alpha = 1f;
        sGroup.sprites[0].isVisible = false; // Only visible when in track edit mode and user is trying to place down a new track.
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        Vector2 drawPos = new Vector2(Mathf.Lerp(lastPos.x, pos.x, frameProgress), Mathf.Lerp(lastPos.y, pos.y, frameProgress));
        sGroup.sprites[0].x = drawPos.x;
        sGroup.sprites[0].y = drawPos.y;

        if ((Util.ShiftDown() && Input.GetMouseButton(0) && VoezEditor.Editor.trackEditMode && !VoezEditor.Editor.MenuOpen && VoezEditor.Editor.EditMode)
            || previewX >= 0 || previewScale >= 0)
            sGroup.sprites[0].isVisible = true;
        else
            sGroup.sprites[0].isVisible = false;

        if (previewScale >= 0)
            sGroup.sprites[0].scaleX = (VoezEditor.windowRes.x / baseWidth) * Track.TRACK_SCREEN_WIDTH * previewScale;
        else
            sGroup.sprites[0].scaleX = (VoezEditor.windowRes.x / baseWidth) * Track.TRACK_SCREEN_WIDTH;

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
