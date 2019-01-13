using UnityEngine;
using System.Collections.Generic;

public class Note : DrawableObject {
    public ProjectData.NoteData data;
    public Track linkedTrack;
    public float noteProgress;
    public float holdProgress;
    public bool hovered;
    public int ID;
    public float tempNoteX = -1000; // temporary X position of note if its linked track hasn't spawned yet.
    public List<HoldTick> holdTicks;
    private bool playedHitSound;
    public float lastTime = -1;
    public int quantizationInd;
    public static float SELECTION_RADIUS = 85f;
    public static float NOTE_DURATION = 1.1f; // number of seconds to transition from top of track to bottom of track
    public static float[] SCROLL_DURATIONS = {
        1.5f,   // 1x
        1.3f,   // 2x
        1.1f,   // 3x
        0.9f,   // 4x
        0.8f,   // 5x
        0.7f,   // 6x
        0.55f,  // 7x
        0.425f, // 8x
        0.3f,   // 9x
        0.2f,   // 10x
    };
    public static Color[] QUANTIZATION_COLORS = {
        Util.Color255(250, 119, 119), // 1x
        Util.Color255(119, 119, 250), // 1/2x
        Util.Color255(255, 255, 255), // 1/3x
        Util.Color255(245, 250, 119), // 1/4x
        Util.Color255(255, 119, 255), // 1/6x
        Util.Color255(119, 250, 248), // 1/8x
        Util.Color255(128, 128, 128), // 1/12x
        Util.Color255(122, 250, 119), // 1/16x
        Util.Color255(45, 45, 45), // other
    };

    public Note(ProjectData.NoteData data)
    {
        this.data = data;
        linkedTrack = null;
        ID = this.data.id;
        if (data.type == ProjectData.NoteData.NoteType.HOLD) {
            holdTicks = new List<HoldTick>();
            for(float i=0.1f; i<=data.hold; i+=0.1f) {
                HoldTick tick = new HoldTick(this, data.time + i);
                holdTicks.Add(tick);
                VoezEditor.Editor.AddObject(tick);
            }
        }
    }

    public bool MouseOver
    {
        get {
            Vector3 mouse = Input.mousePosition;
            if (data.type == ProjectData.NoteData.NoteType.HOLD) {
                float pixelsPerSecond = (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / NOTE_DURATION;
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

    public override void Update()
    {
        base.Update();
        // Calculate quantization
        if (data.time != lastTime) {
            float beats = VoezEditor.Editor.SecondsToBeats(data.time);
            double precBeats = double.Parse(beats.ToString("0.####"));
            int precBeatsMod = (int)(precBeats * 10000) % 10000;
            if (precBeatsMod == 0) // 1x
                quantizationInd = 0;
            else if (precBeatsMod == 5000) // 1/2x
                quantizationInd = 1;
            else if (precBeatsMod == 3333 || precBeatsMod == 6666 || precBeatsMod == 6667) // 1/3x
                quantizationInd = 2;
            else if (precBeatsMod == 2500 || precBeatsMod == 7500) // 1/4x
                quantizationInd = 3;
            else if (precBeatsMod == 1666 || precBeatsMod == 1667 || precBeatsMod == 8333) // 1/6x
                quantizationInd = 4;
            else if (precBeatsMod % 1250 == 0) // 1/8x
                quantizationInd = 5;
            else if (precBeatsMod == 833 || precBeatsMod == 4166 || precBeatsMod == 4167 || precBeatsMod == 5833 || precBeatsMod == 9166 || precBeatsMod == 9167) // 1/12x
                quantizationInd = 6;
            else if (precBeatsMod % 625 == 0) // 1/16x
                quantizationInd = 7;
            else {
                quantizationInd = 8;
            }
        }
        lastTime = data.time;

        // Determine note time
        float spawnTime = data.time - NOTE_DURATION;
        float hitTime = data.time;
        noteProgress = (VoezEditor.Editor.songTime - spawnTime) / (hitTime - spawnTime);
        holdProgress = (VoezEditor.Editor.songTime - spawnTime) / ((hitTime+data.hold) - spawnTime);

        if (linkedTrack == null) {
            for (int i = 0; i < VoezEditor.Editor.activeTracks.Count; i += 1) {
                if (VoezEditor.Editor.activeTracks[i].ID == data.track) {
                    linkedTrack = VoezEditor.Editor.activeTracks[i];
                    break;
                }
            }
            /*if (linkedTrack == null && tempNoteX < 0) {
                for(int i=0; i< VoezEditor.Editor.project.tracks.Count; i+=1) {
                    if (VoezEditor.Editor.project.tracks[i].id == data.track) {
                        tempNoteX = Util.ScreenPosX(VoezEditor.Editor.project.tracks[i].x);
                        break;
                    }
                }
            }*/
            pos.x = tempNoteX;
        }

        if (VoezEditor.Editor.EditMode && !VoezEditor.Editor.MenuOpen && !VoezEditor.Editor.trackEditMode) {
            // Delete Note
            if (hovered && (InputManager.delPushed || InputManager.rightMousePushed || (Util.ShiftDown() && Input.GetMouseButton(1)))) {
                VoezEditor.Editor.project.DeleteNote(data.id);
                VoezEditor.Editor.RefreshAllNotes();
            }
            // Edit Note
            if (MouseOver && InputManager.leftMousePushed && !VoezEditor.Editor.ui.HoveringOverSubmenuItem()) {
                float noteEditWindowX = 0f;
                if (pos.x > VoezEditor.windowRes.x * 0.5f)
                    noteEditWindowX = pos.x - NoteEditor.WIDTH * 0.5f - 64f;
                else
                    noteEditWindowX = pos.x + NoteEditor.WIDTH * 0.5f + 64f;
                VoezEditor.Editor.noteEditor = new NoteEditor(new Vector2(noteEditWindowX, VoezEditor.windowRes.y * 0.5f), data);
                VoezEditor.Editor.AddObject(VoezEditor.Editor.noteEditor);
                VoezEditor.Editor.ui.bpmButton.toggled = false;
            }
        }

        if (VoezEditor.Editor.musicPlayer.source != null && !VoezEditor.Editor.musicPlayer.source.isPlaying)
            playedHitSound = false;

        if (linkedTrack != null) {
            tempNoteX = -1000;
            pos.x = linkedTrack.pos.x;
            pos.y = VoezEditor.windowRes.y - (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT * noteProgress);
            if (noteProgress < 0f)
                Destroy();
            if ((noteProgress > 1f && data.type != ProjectData.NoteData.NoteType.HOLD) || (holdProgress > 1f && data.type == ProjectData.NoteData.NoteType.HOLD)) {
                linkedTrack.flashEffectTime = 5;
                if (VoezEditor.Editor.hitSoundsEnabled && !VoezEditor.Editor.EditMode && !playedHitSound) {
                    if (data.type == ProjectData.NoteData.NoteType.HOLD)
                        VoezEditor.Editor.sfxPlayer.ReleaseHitSound();
                    else if (data.type == ProjectData.NoteData.NoteType.SLIDE)
                        VoezEditor.Editor.sfxPlayer.SlideHitSound();
                    else {
                        VoezEditor.Editor.sfxPlayer.ClickHitSound();
                    }
                    playedHitSound = true;
                }
                Destroy();
            }
            if (noteProgress > 1f && data.type == ProjectData.NoteData.NoteType.HOLD && !playedHitSound && !VoezEditor.Editor.EditMode) {
                if (VoezEditor.Editor.hitSoundsEnabled)
                    VoezEditor.Editor.sfxPlayer.ClickHitSound();
                playedHitSound = true;
            }
            if (noteProgress < 1)
                playedHitSound = false;
            if (linkedTrack.readyForDeletion)
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
                        holdTicks[i].Destroy();
                    }
                    holdTicks = null;
                }
            }
        }
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        if (data.type == ProjectData.NoteData.NoteType.HOLD)
            sGroup.sprites = new FSprite[3];
        else
            sGroup.sprites = new FSprite[1];

        if (data.type == ProjectData.NoteData.NoteType.CLICK) {
            sGroup.sprites[0] = new FSprite("click");
        } else if (data.type == ProjectData.NoteData.NoteType.SLIDE) {
            sGroup.sprites[0] = new FSprite("slide");
        } else if (data.type == ProjectData.NoteData.NoteType.SWIPE) {
            sGroup.sprites[0] = new FSprite("swipe");
        } else if (data.type == ProjectData.NoteData.NoteType.HOLD) {
            sGroup.sprites[0] = new FSprite("click");
            sGroup.sprites[1] = new FSprite("Futile_White");
            sGroup.sprites[1].color = Color.black;
            sGroup.sprites[1].scaleX = Mathf.Sqrt(Mathf.Pow(sGroup.sprites[0].width,2)*2f) / sGroup.sprites[1].width;
            sGroup.sprites[1].anchorY = 0f;
            sGroup.sprites[2] = new FSprite("click");
            sGroup.sprites[2].rotation = 45f;
        }

        if (data.dir == 0)
            sGroup.sprites[0].rotation = 180f + 45f;
        else
            sGroup.sprites[0].rotation = 45f;
    }

    public override void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
    {
        foreach (FSprite fsprite in sGroup.sprites)
            fsprite.RemoveFromContainer();
        FContainer myContainer = VoezEditor.Editor.notesContainer;
        if (data.type == ProjectData.NoteData.NoteType.HOLD) {
            myContainer.AddChild(sGroup.sprites[1]);
            myContainer.AddChild(sGroup.sprites[0]);
            myContainer.AddChild(sGroup.sprites[2]);
        }
        else
            myContainer.AddChild(sGroup.sprites[0]);
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        if (readyForDeletion || VoezEditor.Editor == null)
            return;

        Color useColor = QUANTIZATION_COLORS[0];
        if (VoezEditor.Editor.quantizationEnabled)
            useColor = QUANTIZATION_COLORS[quantizationInd];

        if (lastPos == Vector2.zero || pos == Vector2.zero) {
            for (int i = 0; i < sGroup.sprites.Length; i += 1)
                sGroup.sprites[i].isVisible = false;
        } else {
            for (int i = 0; i < sGroup.sprites.Length; i += 1)
                sGroup.sprites[i].isVisible = true;
        }

        bool strictHighlightCondition = (VoezEditor.Editor.noteEditor != null && VoezEditor.Editor.noteEditor.data.id == ID);
        bool highlightCondition = (MouseOver && !VoezEditor.Editor.MenuOpen) || strictHighlightCondition;
        if (VoezEditor.Editor.EditMode && highlightCondition && !VoezEditor.Editor.trackEditMode && !VoezEditor.Editor.ui.HoveringOverSubmenuItem()) {
            for (int i = 0; i < sGroup.sprites.Length; i += 1)
                sGroup.sprites[i].color = Color.red;
            hovered = true;
        }
        else if (hovered && !strictHighlightCondition) {
            for (int i = 0; i < sGroup.sprites.Length; i += 1) {
                if (i == 1)
                    sGroup.sprites[i].color = Color.black;
                else
                    sGroup.sprites[i].color = Color.white;
            }
            hovered = false;
        }

        sGroup.sprites[0].x = Mathf.Lerp(lastPos.x, pos.x, frameProgress);
        sGroup.sprites[0].y = Mathf.Lerp(lastPos.y, pos.y, frameProgress);
        if ((data.type == ProjectData.NoteData.NoteType.HOLD || data.type == ProjectData.NoteData.NoteType.CLICK) && sGroup.sprites[0].color != Color.red)
        sGroup.sprites[0].color = useColor;

        if (data.type == ProjectData.NoteData.NoteType.HOLD && sGroup.sprites.Length > 2) {
            float pixelsPerSecond = (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / NOTE_DURATION;
            float holdPixels = pixelsPerSecond * data.hold;
            sGroup.sprites[2].y = sGroup.sprites[0].y + holdPixels;
            sGroup.sprites[2].x = sGroup.sprites[0].x;
            sGroup.sprites[1].y = sGroup.sprites[0].y;
            sGroup.sprites[1].x = sGroup.sprites[0].x;
            sGroup.sprites[1].scaleY = (holdPixels) / sGroup.sprites[1].element.sourceRect.height;
            if (sGroup.sprites[2].color != Color.red)
                sGroup.sprites[2].color = useColor;
        }

        base.DrawSprites(sGroup, frameProgress);
    }

    public class HoldTick : DrawableObject
    {
        public float time;
        public Note parentNote;
        public float desiredX = -1000;

        public HoldTick(Note parentNote, float time)
        {
            this.time = time;
            this.parentNote = parentNote;
        }

        public override void Update()
        {
            float pixelsPerSecond = (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / NOTE_DURATION;
            if (desiredX < 0 && parentNote.linkedTrack != null)
                desiredX = Util.ScreenPosX(parentNote.linkedTrack.GetXAtTime(time));
            pos.x = desiredX;
            pos.y = parentNote.pos.y + pixelsPerSecond * (time - parentNote.data.time);

            if (parentNote.readyForDeletion)
                Destroy();
            base.Update();
        }

        public override void InitiateSprites(SpriteGroup sGroup)
        {
            sGroup.sprites = new FSprite[1];
            sGroup.sprites[0] = new FSprite("holdTick");
        }

        public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
        {
            if (lastPos == Vector2.zero || pos == Vector2.zero || VoezEditor.Editor.songTime > time) {
              sGroup.sprites[0].isVisible = false;
            } else {
              sGroup.sprites[0].isVisible = true;
            }

            sGroup.sprites[0].x = Mathf.Lerp(lastPos.x, pos.x, frameProgress);
            sGroup.sprites[0].y = Mathf.Lerp(lastPos.y, pos.y, frameProgress);
            if (parentNote.hovered)
                sGroup.sprites[0].color = Color.red;
            else
                sGroup.sprites[0].color = Color.white;

            base.DrawSprites(sGroup, frameProgress);
        }

        public override void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
        {
            foreach (FSprite fsprite in sGroup.sprites) 
            { 
                fsprite.RemoveFromContainer();
                VoezEditor.Editor.ticksContainer.AddChild(sGroup.sprites[0]);
            }
        }
    }
}