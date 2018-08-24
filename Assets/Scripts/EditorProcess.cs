using UnityEngine;
using System.Collections.Generic;

public class EditorProcess : MainLoopProcess {

    public List<UpdatableObject> updateList;
    public List<SpriteGroup> spriteGroups;
    public List<Track> activeTracks;
    public List<Note> activeNotes;
    public FContainer backgroundContainer;
    public FContainer tracksBottomContainer;
    public FContainer tracksTopContainer;
    public FContainer notesContainer;
    public FContainer ticksContainer;
    public FContainer gridContainer;
    public FContainer foregroundContainer;
    public ProjectData project;
    public NoteEditor noteEditor;
    public TrackEditor trackEditor;
    public MusicPlayer musicPlayer;
    public EditorUI ui;
    public ProjectData.NoteData.NoteType selectedNoteType = ProjectData.NoteData.NoteType.CLICK;
    public float selectedTimeSnap = 4;
    public float currentFrame;
    public float currentTime;
    public float songTime;
    public int tempNoteID = -1;
    public bool trackEditMode;
    public bool confirmBoxOpen;
    public bool init;

    public EditorProcess()
    {
        updateList = new List<UpdatableObject>();
        spriteGroups = new List<SpriteGroup>();
        activeNotes = new List<Note>();
        activeTracks = new List<Track>();

        // Setup drawing layers (order matters here)
        backgroundContainer = new FContainer();
        tracksBottomContainer = new FContainer();
        tracksTopContainer = new FContainer();
        notesContainer = new FContainer();
        ticksContainer = new FContainer();
        gridContainer = new FContainer();
        foregroundContainer = new FContainer();
        Futile.stage.AddChild(backgroundContainer);
        Futile.stage.AddChild(tracksBottomContainer);
        Futile.stage.AddChild(tracksTopContainer);
        Futile.stage.AddChild(notesContainer);
        Futile.stage.AddChild(ticksContainer);
        Futile.stage.AddChild(gridContainer);
        Futile.stage.AddChild(foregroundContainer);
    }

    public bool EditMode
    {
        get { return musicPlayer.hasStarted && musicPlayer.paused; }
    }

    public bool MenuOpen
    {
        get { return noteEditor != null || trackEditor != null || confirmBoxOpen; }
    }

    public void InitiateSong()
    {
        if (project.songClip != null)
            musicPlayer.PlayAudioClip(project.songClip);
        AddObject(new BackgroundImage(project.background));
    }

    public override void Update()
    {
        base.Update();
        
        // Post-Initialization
        if (!init) {
            init = true;
            project = new ProjectData();
            project.LoadFromActiveProject();
            musicPlayer = new MusicPlayer();
            ui = new EditorUI();
            InitiateSong();
            musicPlayer.PauseSong(); // wait for user to manually start the song with the play button
        }

        // Update all active objects
        musicPlayer.Update();
        ui.Update();
        int updateIndex = updateList.Count - 1;
        while (updateIndex >= 0) {
            UpdatableObject obj = updateList[updateIndex];
            if (obj.readyForDeletion)
                PurgeObject(obj);
            else
                obj.Update();
            updateIndex--;
        }

        if (!musicPlayer.source.isPlaying && musicPlayer.hasStarted && !musicPlayer.paused) {
            currentFrame = 0f;
            if (project.songClip != null)
                musicPlayer.PlayAudioClip(project.songClip);
        }

        if (musicPlayer.source.isPlaying)
            currentFrame += 1 * musicPlayer.playbackSpeed;

        // Frame Advancing while Paused
        if (EditMode && !MenuOpen && !ui.bpmButton.toggled) {
            float delta = GetBPMTimeIncrement() * framesPerSecond;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
                currentFrame = Mathf.Min(currentFrame + delta, musicPlayer.source.clip.length * framesPerSecond);
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
                currentFrame = Mathf.Max(currentFrame - delta, 0);
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
                currentFrame = Mathf.Min(currentFrame + (delta * 4f), musicPlayer.source.clip.length * framesPerSecond);
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
                currentFrame = Mathf.Max(currentFrame - (delta * 4f), 0);
        }

        currentTime = currentFrame / framesPerSecond;
        songTime = currentTime;
        musicPlayer.SyncTracker(songTime);

        // Spawn Tracks
        for (int i = 0; i < project.tracks.Count; i += 1)
            if (songTime >= project.tracks[i].start && songTime <= project.tracks[i].end && !TrackSpawned(project.tracks[i].id))
                AddObject(new Track(project.tracks[i]));

        // Spawn Notes
        for (int i = 0; i < project.notes.Count; i += 1) {
            if (songTime >= project.notes[i].time - Note.NOTE_DURATION && songTime <= project.notes[i].time + project.notes[i].hold && !NoteSpawned(project.notes[i].id)) {
                AddObject(new Note(project.notes[i]));
            }
        }

        // Find track mouse is hovering over (or track closest to mouse if hovering over multiple)
        for (int i = 0; i < activeTracks.Count; i += 1)
            activeTracks[i].activeHover = false;
        if (EditMode && !MenuOpen) {
            float nearestDist = int.MaxValue;
            Track nearestTrack = null;
            for (int i = 0; i < activeTracks.Count; i += 1) {
                float dist = Mathf.Abs(Input.mousePosition.x - activeTracks[i].pos.x);
                if (activeTracks[i].MouseOver && dist < nearestDist) {
                    nearestDist = dist;
                    nearestTrack = activeTracks[i];
                }
            }
            if (nearestTrack != null) {
                nearestTrack.activeHover = true;
                if (!trackEditMode) {
                    ui.trackAdder.notePreviewVisible = true;
                    ui.trackAdder.pos.x = nearestTrack.pos.x;
                    ui.trackAdder.pos.y = ui.grid.SnapToGridY(Input.mousePosition.y);
                }

                // Add New Note to Hovered Track
                if (Input.GetMouseButtonDown(0) && !HoveringOverAnyNote() && !ui.HoveringOverSubmenuItem() && !trackEditMode) {
                    float desiredSongTime = ui.grid.GetSongTimeAtGridY(Input.mousePosition.y);
                    desiredSongTime = Mathf.Clamp(desiredSongTime, 0f, musicPlayer.source.clip.length);
                    if (!TrackOccupiedAtTime(nearestTrack.ID, desiredSongTime)) {
                        ProjectData.NoteData newNote = new ProjectData.NoteData();
                        newNote.id = GetUniqueTempNoteID();
                        newNote.time = desiredSongTime;
                        newNote.track = nearestTrack.ID;
                        newNote.type = selectedNoteType;
                        project.AddNote(newNote);
                        RefreshAllNotes();
                    }
                }
            } else
                ui.trackAdder.notePreviewVisible = false;
        } else
            ui.trackAdder.notePreviewVisible = false;
    }

    public void RefreshAllTracks()
    {
        for (int i = 0; i < activeTracks.Count; i += 1)
            activeTracks[i].Destroy();
    }

    public void RefreshAllNotes()
    {
        for (int i = 0; i < activeNotes.Count; i += 1)
            activeNotes[i].Destroy();
    }

    public void RefreshNote(int id)
    {
        for (int i = 0; i < activeNotes.Count; i += 1) {
            if (activeNotes[i].ID == id) {
                activeNotes[i].Destroy();
                break;
            }
        }
    }

    public void RefreshTrack(int id)
    {
        for (int i = 0; i < activeTracks.Count; i += 1) {
            if (activeTracks[i].ID == id) {
                activeTracks[i].Destroy();
                break;
            }
        }
    }

    public bool HoveringOverAnyNote()
    {
        for (int i = 0; i < activeNotes.Count; i += 1)
            if (activeNotes[i].hovered)
                return true;
        return false;
    }

    public bool TrackOccupiedAtTime(int trackID, float time)
    {
        float timePadding = 1f / framesPerSecond;
        for(int i=0; i<project.notes.Count; i+=1) {
            if (project.notes[i].track == trackID) {
                if (project.notes[i].type == ProjectData.NoteData.NoteType.HOLD && time >= project.notes[i].time - timePadding && time <= project.notes[i].time + project.notes[i].hold + timePadding)
                    return true;
                if (project.notes[i].type != ProjectData.NoteData.NoteType.HOLD && time >= project.notes[i].time - timePadding && time <= project.notes[i].time + timePadding)
                    return true;
            }
        }
        return false;
    }

    public int GetUniqueTempNoteID()
    {
        tempNoteID -= 1;
        return tempNoteID;
    }

    public bool TrackSpawned(int id)
    {
        for(int i=0; i<activeTracks.Count; i+=1)
            if (activeTracks[i].ID == id)
                return true;
        return false;
    }

    public bool NoteSpawned(int id)
    {
        for (int i = 0; i < activeNotes.Count; i += 1)
            if (activeNotes[i].ID == id)
                return true;
        return false;
    }

    public void JumpToTime(float seconds)
    {
        musicPlayer.source.time = seconds;
        currentFrame = Mathf.CeilToInt(VoezEditor.Editor.musicPlayer.source.time * VoezEditor.Editor.framesPerSecond);
    }

    public float GetBPMTimeIncrement()
    {
        if (selectedTimeSnap == 0)
            return 1f / framesPerSecond;
        float timeIncrement = 0;
        if (project.songBPM > 0) {
            float secondsPerBeat = 60f / project.songBPM;
            timeIncrement = secondsPerBeat / selectedTimeSnap; // BPM data available; set time snap to match BPM
        } else
            timeIncrement = 1f / selectedTimeSnap; // No BPM data; treat time snap as beats per second -- ie: 60 BPM
        return timeIncrement;
    }

    public void AddObject(UpdatableObject obj)
    {
        this.updateList.Add(obj);
        if (obj is IDrawable) {
            SpriteGroup group = new SpriteGroup(obj as IDrawable);
            spriteGroups.Add(group);
            if (obj is UIElement)
                group.AddSpritesToContainer(foregroundContainer);
            else
                group.AddSpritesToContainer(backgroundContainer);
        }
        if (obj is Track)
            activeTracks.Add(obj as Track);
        if (obj is Note)
            activeNotes.Add(obj as Note);
    }

    private void PurgeObject(UpdatableObject obj)
    {
        updateList.Remove(obj);
        if (obj is IDrawable) {
            for (int i = spriteGroups.Count - 1; i >= 0; i--) {
                if (spriteGroups[i].drawableObject == obj) {
                    spriteGroups[i].CleanSpritesAndRemove();
                    break;
                }
            }
        }
        if (obj is Track)
            activeTracks.Remove(obj as Track);
        if (obj is Note)
            activeNotes.Remove(obj as Note);
    }

    public override void DrawUpdate(float frameProgress)
    {
        base.DrawUpdate(frameProgress);
        for (int i = spriteGroups.Count - 1; i >= 0; i--) {
            this.spriteGroups[i].Update(frameProgress);
            if (this.spriteGroups[i].deleteMeNextFrame)
                this.spriteGroups.RemoveAt(i);
        }
    }
}
