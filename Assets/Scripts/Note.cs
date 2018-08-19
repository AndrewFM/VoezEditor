using UnityEngine;
using System.Collections;

public class Note : CosmeticSprite {
    public ProjectData.NoteData data;
    public EditorProcess controller;
    public Track linkedTrack;
    public float noteProgress;
    public int ID;
    public static float NOTE_DURATION = 1.5f; // number of seconds to transition from top of track to bottom of track
    public float tempNoteX = -1000; // temporary X position of note if its linked track hasn't spawned yet.

    public Note(EditorProcess parent, ProjectData.NoteData data)
    {
        this.data = data;
        controller = parent;
        linkedTrack = null;
        ID = this.data.id;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        float spawnTime = data.time - NOTE_DURATION;
        float hitTime = data.time;
        noteProgress = (controller.songTime - spawnTime) / (hitTime - spawnTime);

        if (linkedTrack == null) {
            for (int i = 0; i < controller.activeTracks.Count; i += 1) {
                if (controller.activeTracks[i].ID == data.track) {
                    linkedTrack = controller.activeTracks[i];
                    break;
                }
            }
            if (linkedTrack == null && tempNoteX < 0) {
                for(int i=0; i<controller.project.tracks.Count; i+=1) {
                    if (controller.project.tracks[i].id == data.track) {
                        tempNoteX = Util.ScreenPosX(controller.project.tracks[i].x);
                        break;
                    }
                }
            }
            pos.x = tempNoteX;
        }

        if (linkedTrack != null) {
            pos.x = linkedTrack.pos.x;
            pos.y = MainScript.windowRes.y - (MainScript.windowRes.y * Track.TRACK_SCREEN_HEIGHT * noteProgress);
            if (noteProgress > 1f) {
                slatedForDeletetion = true;
                linkedTrack.flashEffectTime = 5;
            }
            if (linkedTrack.slatedForDeletetion)
                linkedTrack = null;
        }
    }

    public override void InitiateSprites(SpriteLeaser sLeaser)
    {
        sLeaser.sprites = new FSprite[1];
        if (data.type == ProjectData.NoteData.NoteType.CLICK) {
            sLeaser.sprites[0] = new FSprite("click");
        } else if (data.type == ProjectData.NoteData.NoteType.SLIDE) {
            sLeaser.sprites[0] = new FSprite("slide");
        } else if (data.type == ProjectData.NoteData.NoteType.SWIPE) {
            sLeaser.sprites[0] = new FSprite("swipe");
        } else {
            sLeaser.sprites[0] = new FSprite("click");
        }
        if (data.dir == 0)
            sLeaser.sprites[0].rotation = 180f + 45f;
        else
            sLeaser.sprites[0].rotation = 45f;
    }

    public override void DrawSprites(SpriteLeaser sLeaser, float timeStacker)
    {
        if (lastPos == Vector2.zero || pos == Vector2.zero)
            sLeaser.sprites[0].isVisible = false;
        else
            sLeaser.sprites[0].isVisible = true;

        sLeaser.sprites[0].x = Mathf.Lerp(lastPos.x, pos.x, timeStacker);
        sLeaser.sprites[0].y = Mathf.Lerp(lastPos.y, pos.y, timeStacker);
        base.DrawSprites(sLeaser, timeStacker);
    }
}