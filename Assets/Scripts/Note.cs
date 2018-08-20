using UnityEngine;
using System.Collections.Generic;

public class Note : CosmeticSprite {
    public ProjectData.NoteData data;
    public EditorProcess controller;
    public Track linkedTrack;
    public float noteProgress;
    public float holdProgress;
    public bool hovered;
    public int ID;
    public static float NOTE_DURATION = 1.5f; // number of seconds to transition from top of track to bottom of track
    public static float SELECTION_RADIUS = 85f;
    public float tempNoteX = -1000; // temporary X position of note if its linked track hasn't spawned yet.
    public List<HoldTick> holdTicks;

    public Note(EditorProcess parent, ProjectData.NoteData data)
    {
        this.data = data;
        controller = parent;
        linkedTrack = null;
        ID = this.data.id;
        if (data.type == ProjectData.NoteData.NoteType.HOLD) {
            holdTicks = new List<HoldTick>();
            for(float i=0.1f; i<=data.hold; i+=0.1f) {
                HoldTick tick = new HoldTick(this, data.time + i);
                holdTicks.Add(tick);
                controller.AddObject(tick);
            }
        }
    }

    public bool MouseOver
    {
        get {
            Vector3 mouse = Input.mousePosition;
            if (data.type == ProjectData.NoteData.NoteType.HOLD) {
                float pixelsPerSecond = (MainScript.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / NOTE_DURATION;
                float holdPixels = pixelsPerSecond * data.hold;
                float dx = Mathf.Abs(mouse.x - pos.x);
                float dy = Mathf.Abs(mouse.y - pos.y);
                float dx2 = Mathf.Abs(mouse.x - pos.x);
                float dy2 = Mathf.Abs(mouse.y - (pos.y + holdPixels));
                return (dx / SELECTION_RADIUS + dy / SELECTION_RADIUS) <= 0.5f || (dx2 / SELECTION_RADIUS + dy2 / SELECTION_RADIUS) <= 0.5f
                    || (mouse.y > pos.y && mouse.y < pos.y + holdPixels && mouse.x > pos.x - SELECTION_RADIUS * 0.5f && mouse.x < pos.x + SELECTION_RADIUS * 0.5f);
            } else {
                float dx = Mathf.Abs(mouse.x - pos.x);
                float dy = Mathf.Abs(mouse.y - pos.y);
                return (dx / SELECTION_RADIUS + dy / SELECTION_RADIUS) <= 0.5f;
            }
        }
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        float spawnTime = data.time - NOTE_DURATION;
        float hitTime = data.time;
        noteProgress = (controller.songTime - spawnTime) / (hitTime - spawnTime);
        holdProgress = (controller.songTime - spawnTime) / ((hitTime+data.hold) - spawnTime);

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

        if (hovered && (Input.GetKeyDown(KeyCode.Delete) || (Util.ShiftDown() && Input.GetMouseButton(1)))) {
            controller.project.DeleteNote(data.id);
            controller.RefreshSpawns();
        }

        if (linkedTrack != null) {
            pos.x = linkedTrack.pos.x;
            pos.y = MainScript.windowRes.y - (MainScript.windowRes.y * Track.TRACK_SCREEN_HEIGHT * noteProgress);
            if (noteProgress < 0f)
                slatedForDeletetion = true;
            if ((noteProgress > 1f && data.type != ProjectData.NoteData.NoteType.HOLD) || (holdProgress > 1f && data.type == ProjectData.NoteData.NoteType.HOLD)) {
                slatedForDeletetion = true;
                linkedTrack.flashEffectTime = 5;
            }
            if (linkedTrack.slatedForDeletetion)
                linkedTrack = null;

            // For hold tracks, determine whether hold ticks should be displayed or not
            if (holdTicks != null) {
                bool holdTicksAllSame = true;
                for(int i=0; i<holdTicks.Count; i+=1) {
                    if (holdTicks[i].desiredX < 0 || (i > 0 && holdTicks[i].desiredX != holdTicks[i - 1].desiredX)) {
                        holdTicksAllSame = false;
                        break;
                    }
                }
                if (holdTicksAllSame) {
                    for(int i=0; i<holdTicks.Count; i+=1) {
                        holdTicks[i].slatedForDeletetion = true;
                    }
                    holdTicks = null;
                }
            }
        }
    }

    public override void InitiateSprites(SpriteLeaser sLeaser)
    {
        if (data.type == ProjectData.NoteData.NoteType.HOLD)
            sLeaser.sprites = new FSprite[3];
        else
            sLeaser.sprites = new FSprite[1];

        if (data.type == ProjectData.NoteData.NoteType.CLICK) {
            sLeaser.sprites[0] = new FSprite("click");
        } else if (data.type == ProjectData.NoteData.NoteType.SLIDE) {
            sLeaser.sprites[0] = new FSprite("slide");
        } else if (data.type == ProjectData.NoteData.NoteType.SWIPE) {
            sLeaser.sprites[0] = new FSprite("swipe");
        } else if (data.type == ProjectData.NoteData.NoteType.HOLD) {
            sLeaser.sprites[0] = new FSprite("click");
            sLeaser.sprites[1] = new FSprite("Futile_White");
            sLeaser.sprites[1].color = Color.black;
            sLeaser.sprites[1].scaleX = Mathf.Sqrt(Mathf.Pow(sLeaser.sprites[0].width,2)*2f) / sLeaser.sprites[1].width;
            sLeaser.sprites[1].anchorY = 0f;
            sLeaser.sprites[2] = new FSprite("click");
            sLeaser.sprites[2].rotation = 45f;
        }

        if (data.dir == 0)
            sLeaser.sprites[0].rotation = 180f + 45f;
        else
            sLeaser.sprites[0].rotation = 45f;
    }

    public override void AddToContainer(SpriteLeaser sLeaser, FContainer newContainer)
    {
        foreach (FSprite fsprite in sLeaser.sprites)
            fsprite.RemoveFromContainer();
        if (data.type == ProjectData.NoteData.NoteType.HOLD) {
            newContainer.AddChild(sLeaser.sprites[1]);
            newContainer.AddChild(sLeaser.sprites[0]);
            newContainer.AddChild(sLeaser.sprites[2]);
        }
        else
            newContainer.AddChild(sLeaser.sprites[0]);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, float timeStacker)
    {
        if (lastPos == Vector2.zero || pos == Vector2.zero) {
            for (int i = 0; i < sLeaser.sprites.Length; i += 1)
                sLeaser.sprites[i].isVisible = false;
        } else {
            for (int i = 0; i < sLeaser.sprites.Length; i += 1)
                sLeaser.sprites[i].isVisible = true;
        }

        if (controller.musicPlayer.paused && !hovered && MouseOver) {
            for (int i = 0; i < sLeaser.sprites.Length; i += 1)
                sLeaser.sprites[i].color = Color.red;
            hovered = true;
        }
        else if (hovered && !MouseOver) {
            for (int i = 0; i < sLeaser.sprites.Length; i += 1) {
                if (i == 1)
                    sLeaser.sprites[i].color = Color.black;
                else
                    sLeaser.sprites[i].color = Color.white;
            }
            hovered = false;
        }

        sLeaser.sprites[0].x = Mathf.Lerp(lastPos.x, pos.x, timeStacker);
        sLeaser.sprites[0].y = Mathf.Lerp(lastPos.y, pos.y, timeStacker);

        if (data.type == ProjectData.NoteData.NoteType.HOLD) {
            float pixelsPerSecond = (MainScript.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / NOTE_DURATION;
            float holdPixels = pixelsPerSecond * data.hold;
            sLeaser.sprites[2].y = sLeaser.sprites[0].y + holdPixels;
            sLeaser.sprites[2].x = sLeaser.sprites[0].x;
            sLeaser.sprites[1].y = sLeaser.sprites[0].y;
            sLeaser.sprites[1].x = sLeaser.sprites[0].x;
            sLeaser.sprites[1].scaleY = (holdPixels) / sLeaser.sprites[1].element.sourceRect.height;
        }

        base.DrawSprites(sLeaser, timeStacker);
    }

    public class HoldTick : CosmeticSprite
    {
        public float time;
        public Note parentNote;
        public float desiredX = -1000;

        public HoldTick(Note parentNote, float time)
        {
            this.time = time;
            this.parentNote = parentNote;
        }

        public override void Update(bool eu)
        {
            float pixelsPerSecond = (MainScript.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / NOTE_DURATION;
            if (desiredX < 0 && parentNote.linkedTrack != null)
                desiredX = Util.ScreenPosX(parentNote.linkedTrack.GetXAtTime(time));
            pos.x = desiredX;
            pos.y = parentNote.pos.y + pixelsPerSecond * (time - parentNote.data.time);

            if (parentNote.slatedForDeletetion)
                slatedForDeletetion = true;
            base.Update(eu);
        }

        public override void InitiateSprites(SpriteLeaser sLeaser)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("holdTick");
        }

        public override void DrawSprites(SpriteLeaser sLeaser, float timeStacker)
        {
            if (lastPos == Vector2.zero || pos == Vector2.zero || parentNote.controller.songTime > time) {
              sLeaser.sprites[0].isVisible = false;
            } else {
              sLeaser.sprites[0].isVisible = true;
            }

            sLeaser.sprites[0].x = Mathf.Lerp(lastPos.x, pos.x, timeStacker);
            sLeaser.sprites[0].y = Mathf.Lerp(lastPos.y, pos.y, timeStacker);
            if (parentNote.hovered)
                sLeaser.sprites[0].color = Color.red;
            else
                sLeaser.sprites[0].color = Color.white;

            base.DrawSprites(sLeaser, timeStacker);
        }
    }
}