using UnityEngine;
using System.Collections.Generic;

public class EditorProcess : MainLoopProcess {

    public List<UpdatableAndDeletable> updateList;
    public List<SpriteLeaser> spriteLeasers;
    public List<Track> activeTracks;
    public List<Note> activeNotes;
    public bool evenUpdate;
    public FContainer backgroundContainer;
    public FContainer tracksBottomContainer;
    public FContainer tracksTopContainer;
    public FContainer notesContainer;
    public FContainer foregroundContainer;
    public ProjectData project;
    public MusicPlayer musicPlayer;

    public int currentFrame;
    public float currentTime;
    public float songTime;

    public EditorProcess()
    {
        updateList = new List<UpdatableAndDeletable>();
        spriteLeasers = new List<SpriteLeaser>();
        activeNotes = new List<Note>();
        activeTracks = new List<Track>();

        // Setup drawing layers (order matters here)
        backgroundContainer = new FContainer();
        tracksBottomContainer = new FContainer();
        tracksTopContainer = new FContainer();
        notesContainer = new FContainer();
        foregroundContainer = new FContainer();
        Futile.stage.AddChild(backgroundContainer);
        Futile.stage.AddChild(tracksBottomContainer);
        Futile.stage.AddChild(tracksTopContainer);
        Futile.stage.AddChild(notesContainer);
        Futile.stage.AddChild(foregroundContainer);

        musicPlayer = new MusicPlayer();
        InitiateSong();        
    }

    public void InitiateSong()
    {
        project = new ProjectData();
        project.LoadFromActiveProject();
        if (project.songClip != null)
            musicPlayer.PlayAudioClip(project.songClip);
        if (project.background != null)
            AddObject(new BackgroundImage(project.background));
    }

    public override void Update()
    {
        base.Update();
        
        // Update all active objects
        musicPlayer.Update();
        evenUpdate = !evenUpdate;
        int updateIndex = this.updateList.Count - 1;
        while (updateIndex >= 0) {
            UpdatableAndDeletable updatableAndDeletable = updateList[updateIndex];
            if (updatableAndDeletable.slatedForDeletetion) {
                PurgeObject(updatableAndDeletable);
            } else {
                updatableAndDeletable.Update(evenUpdate);
            }
            updateIndex--;
        }

        if (musicPlayer.source.isPlaying)
            currentFrame += 1;
        currentTime = (float)currentFrame / (float)framesPerSecond;
        songTime = currentTime;

        // Spawn Tracks
        for (int i = 0; i < project.tracks.Count; i += 1)
            if (songTime >= project.tracks[i].start && songTime < project.tracks[i].end && !TrackSpawned(project.tracks[i].id))
                AddObject(new Track(this, project.tracks[i]));

        // Spawn Notes
        /*
        for (int i = 0; i < project.notes.Count; i += 1) {
            if (songTime >= project.notes[i].time - Note.NOTE_DURATION && songTime < project.notes[i].time && !NoteSpawned(project.notes[i].id)) {
                AddObject(new Note(this, project.notes[i]));
            }
        }
        */
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

    public void AddObject(UpdatableAndDeletable obj)
    {
        this.updateList.Add(obj);
        if (obj is IDrawable) {
            SpriteLeaser sL = new SpriteLeaser(obj as IDrawable);
            spriteLeasers.Add(sL);
            if (obj is Track)
                sL.AddSpritesToContainer(tracksBottomContainer);
            else if (obj is Note)
                sL.AddSpritesToContainer(notesContainer);
            else
                sL.AddSpritesToContainer(backgroundContainer);
        }
        if (obj is Track)
            activeTracks.Add(obj as Track);
        if (obj is Note)
            activeNotes.Add(obj as Note);
    }

    public void PurgeObject(UpdatableAndDeletable obj)
    {
        updateList.Remove(obj);
        if (obj is IDrawable) {
            for (int i = spriteLeasers.Count - 1; i >= 0; i--) {
                if (spriteLeasers[i].drawableObject == obj) {
                    spriteLeasers[i].CleanSpritesAndRemove();
                    break;
                }
            }
        }
        if (obj is Track)
            activeTracks.Remove(obj as Track);
        if (obj is Note)
            activeNotes.Remove(obj as Note);
    }

    public override void GrafUpdate(float timeStacker)
    {
        base.GrafUpdate(timeStacker);
        for (int i = spriteLeasers.Count - 1; i >= 0; i--) {
            this.spriteLeasers[i].Update(timeStacker);
            if (this.spriteLeasers[i].deleteMeNextFrame)
                this.spriteLeasers.RemoveAt(i);
        }
    }
}
