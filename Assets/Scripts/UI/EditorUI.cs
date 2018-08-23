using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EditorUI {
    public Button playButton;
    public Button playbackTimeButton;
    public Button notesButton;
    public Button gridButton;
    public Button saveButton;
    public Button bpmButton;
    public Button[] snapTimes;
    public Button[] playbackTimes;
    public Button[] noteTypes;
    public Slider playbackSlider;
    public SnapGrid grid;
    public TrackAddPreview trackAdder;
    public DropshadowLabel playbackTimeLabel;

	public EditorUI()
    {
        float bbSize = 80f;
        float bbPad = 10f;
        float bbOrigin = bbPad + bbSize * 0.5f;

        playButton = new Button("play", new Vector2(bbOrigin, bbOrigin), bbSize, false);
        playbackTimeButton = new Button("time", new Vector2(bbOrigin + bbSize + bbPad, bbOrigin), bbSize, false);
        notesButton = new Button("notes", new Vector2(bbOrigin + bbSize * 2 + bbPad * 2, bbOrigin), bbSize, false);
        gridButton = new Button("grid", new Vector2(bbOrigin + bbSize * 3 + bbPad * 3, bbOrigin), bbSize, false);
        bpmButton = new Button("Raleway24", "BPM"+Environment.NewLine+VoezEditor.Editor.project.songBPM.ToString(), new Vector2(bbOrigin + bbSize * 4 + bbPad * 4, bbOrigin), bbSize, false);
        saveButton = new Button("save", new Vector2(bbOrigin + bbSize * 5 + bbPad * 5, bbOrigin), bbSize, false);
        VoezEditor.Editor.AddObject(playButton);
        VoezEditor.Editor.AddObject(playbackTimeButton);
        VoezEditor.Editor.AddObject(notesButton);
        VoezEditor.Editor.AddObject(gridButton);
        VoezEditor.Editor.AddObject(bpmButton);
        VoezEditor.Editor.AddObject(saveButton);

        playbackTimes = new Button[4];
        for(int i=0; i<playbackTimes.Length; i+=1) {
            string buttonText = "0.25x";
            if (i == 1)
                buttonText = "0.5x";
            if (i == 2)
                buttonText = "1.0x";
            if (i == 3)
                buttonText = "2.0x";
            playbackTimes[i] = new Button("Raleway24", buttonText
                , new Vector2(playButton.pos.x + i*69f + 32f, playButton.pos.y + bbSize * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
            playbackTimes[i].visible = false;
            if (i == 2)
                playbackTimes[i].toggled = true;
            VoezEditor.Editor.AddObject(playbackTimes[i]);
        }

        noteTypes = new Button[4];
        for(int i=0; i< noteTypes.Length; i+=1) {
            string noteSymbol = "click";
            if (i == 1)
                noteSymbol = "slide";
            if (i == 2)
                noteSymbol = "swipe";
            if (i == 3)
                noteSymbol = "track";
            noteTypes[i] = new Button(noteSymbol
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + bbSize * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
            noteTypes[i].visible = false;
            if (i != 3)
                noteTypes[i].mySymbol.rotation = 45f;
            else
                noteTypes[i].mySymbol.scale = 1.5f;
            if (i == 0)
                noteTypes[i].toggled = true;
            VoezEditor.Editor.AddObject(noteTypes[i]);
        }

        snapTimes = new Button[6];
        for (int i = 0; i < snapTimes.Length; i += 1) {
            string buttonText = "Off";
            if (i == 1)
                buttonText = "1/16";
            if (i == 2)
                buttonText = "1/8";
            if (i == 3)
                buttonText = "1/4";
            if (i == 4)
                buttonText = "1/2";
            if (i == 5)
                buttonText = "1";
            snapTimes[i] = new Button("Raleway24", buttonText
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + bbSize * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
            snapTimes[i].visible = false;
            if (i == 3)
                snapTimes[i].toggled = true;
            VoezEditor.Editor.AddObject(snapTimes[i]);
        }

        float sliderStart = bbPad * 7 + bbSize * 6 + 44f;
        float sliderEnd = VoezEditor.windowRes.x - 230f;
        playbackSlider = new Slider(new Vector2((sliderStart + sliderEnd) * 0.5f, bbPad + bbSize * 0.5f), sliderEnd - sliderStart);
        VoezEditor.Editor.AddObject(playbackSlider);
        grid = new SnapGrid();
        VoezEditor.Editor.AddObject(grid);
        trackAdder = new TrackAddPreview();
        VoezEditor.Editor.AddObject(trackAdder);
        playbackTimeLabel = new DropshadowLabel("Raleway24", "00:00/00:00", new Vector2(VoezEditor.windowRes.x - 110f, bbPad + bbSize * 0.5f), new Vector2(2f, -2f));
        VoezEditor.Editor.AddObject(playbackTimeLabel);
    }

    public bool HoveringOverSubmenuItem()
    {
        for (int i = 0; i < noteTypes.Length; i += 1)
            if (noteTypes[i].visible && noteTypes[i].MouseOver)
                return true;
        for (int i = 0; i < playbackTimes.Length; i += 1)
            if (playbackTimes[i].visible && playbackTimes[i].MouseOver)
                return true;
        for (int i = 0; i < snapTimes.Length; i += 1)
            if (snapTimes[i].visible && snapTimes[i].MouseOver)
                return true;
        if (playbackSlider.MouseOver || playButton.MouseOver || playbackTimeButton.MouseOver || notesButton.MouseOver || gridButton.MouseOver || saveButton.MouseOver || bpmButton.MouseOver)
            return true;
        return false;
    }

    public void Update()
    {
        // Handle Playback Slider Dragged
        if (playbackSlider.progressUpdate) {
            VoezEditor.Editor.musicPlayer.source.time = VoezEditor.Editor.musicPlayer.source.clip.length * Mathf.Clamp(playbackSlider.progress, 0f, 0.99f);
            VoezEditor.Editor.currentFrame = (int)(VoezEditor.Editor.musicPlayer.source.time * VoezEditor.Editor.framesPerSecond);
            grid.SnapPlaytimeToGrid();
            playbackSlider.progressUpdate = false;
        }

        // Update timestamp for current song time
        if (VoezEditor.Editor.musicPlayer.source.clip != null) {
            playbackSlider.allowScrubbing = true;
            playbackSlider.progress = VoezEditor.Editor.songTime / VoezEditor.Editor.musicPlayer.source.clip.length;
            if (!playbackSlider.clicked)
                // If playback slider is not being dragged, show current song time.
                playbackTimeLabel.SetText(Util.MinuteTimeStampFromSeconds((int)VoezEditor.Editor.songTime).ToString() + "/" + Util.MinuteTimeStampFromSeconds((int)VoezEditor.Editor.musicPlayer.source.clip.length).ToString());
            else
                // If playback slider is being dragged, show song time at slider's current position
                playbackTimeLabel.SetText(Util.MinuteTimeStampFromSeconds((int)(VoezEditor.Editor.musicPlayer.source.clip.length * playbackSlider.pendingProgress)).ToString() + "/" + Util.MinuteTimeStampFromSeconds((int)VoezEditor.Editor.musicPlayer.source.clip.length).ToString());
        } else
            playbackSlider.allowScrubbing = false;

        // Play/Pause
        if (playButton.clicked || Input.GetKeyDown(KeyCode.Space)) {
            if (VoezEditor.Editor.musicPlayer.paused) {
                VoezEditor.Editor.musicPlayer.ResumeSong();
                playButton.mySymbol.element = Futile.atlasManager.GetElementWithName("pause");
            } else {
                VoezEditor.Editor.musicPlayer.PauseSong();
                playButton.mySymbol.element = Futile.atlasManager.GetElementWithName("play");
                grid.SnapPlaytimeToGrid();
            }
            playButton.clicked = false;
        }

        // Toggle Loop Point
        if (playbackSlider.rightClicked) {
            if (playbackSlider.loopPoint >= 0f)
                playbackSlider.loopPoint = -1;
            else
                playbackSlider.loopPoint = Mathf.Clamp(playbackSlider.progress, 0f, 0.99f);
            playbackSlider.rightClicked = false;
        }

        // Jump Playback To Loop Point
        if (playbackSlider.loopPoint >= 0f && (playButton.rightClicked || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(2))) {
            VoezEditor.Editor.musicPlayer.source.time = VoezEditor.Editor.musicPlayer.source.clip.length * Mathf.Clamp(playbackSlider.loopPoint, 0f, 0.99f);
            VoezEditor.Editor.currentFrame = (int)(VoezEditor.Editor.musicPlayer.source.time * VoezEditor.Editor.framesPerSecond);
            grid.SnapPlaytimeToGrid();
            playButton.rightClicked = false;
        }

        // BPM Editing
        if (bpmButton.clicked) {
            bpmButton.toggled = !bpmButton.toggled;
            bpmButton.clicked = false;
        }
        if (bpmButton.toggled) {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
                VoezEditor.Editor.project.songBPM += 1;
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
                VoezEditor.Editor.project.songBPM -= 1;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
                VoezEditor.Editor.project.songBPM += 5;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
                VoezEditor.Editor.project.songBPM -= 5;
            VoezEditor.Editor.project.songBPM = Mathf.Clamp(VoezEditor.Editor.project.songBPM, 10, 250);
            bpmButton.myText.text = "BPM" + Environment.NewLine + VoezEditor.Editor.project.songBPM.ToString();
        }

        // Open Playback Speed Menu
        if (playbackTimeButton.clicked) {
            if (notesButton.toggled && !playbackTimeButton.toggled)
                notesButton.clicked = true; // Notes Selection Menu is already open, close it.
            if (gridButton.toggled && !playbackTimeButton.toggled)
                gridButton.clicked = true; // Grid Snap Menu is already open, close it.

            for (int i = 0; i < playbackTimes.Length; i += 1)
                playbackTimes[i].visible = !playbackTimes[i].visible;
            playbackTimeButton.toggled = playbackTimes[0].visible;
            playbackTimeButton.clicked = false;
        }

        // Open Notes Selection Menu
        if (notesButton.clicked) {
            if (playbackTimeButton.toggled && !notesButton.toggled)
                playbackTimeButton.clicked = true; // Playback Speed Menu is already open, close it.
            if (gridButton.toggled && !notesButton.toggled)
                gridButton.clicked = true; // Grid Snap Menu is already open, close it.

            for (int i = 0; i < noteTypes.Length; i += 1)
                noteTypes[i].visible = !noteTypes[i].visible;
            notesButton.toggled = noteTypes[0].visible;
            notesButton.clicked = false;
        }

        // Open Grid Snap Menu
        if (gridButton.clicked) {
            if (notesButton.toggled && !gridButton.toggled)
                notesButton.clicked = true; // Notes Selection Menu is already open, close it.
            if (playbackTimeButton.toggled && !gridButton.toggled)
                playbackTimeButton.clicked = true; // Playback Speed Menu is already open, close it.

            for (int i = 0; i < snapTimes.Length; i += 1)
                snapTimes[i].visible = !snapTimes[i].visible;
            gridButton.toggled = snapTimes[0].visible;
            gridButton.clicked = false;
        }

        // Save Project
        if (saveButton.clicked) {
            VoezEditor.Editor.project.ExportActiveProject();
            saveButton.clicked = false;
        }

        // Set Playback Speed
        for(int i=0; i<playbackTimes.Length; i+=1) {
            if (playbackTimes[i].clicked) {
                for (int j = 0; j < playbackTimes.Length; j += 1)
                    playbackTimes[j].toggled = false;
                playbackTimes[i].toggled = true;
                if (i == 0)
                    VoezEditor.Editor.musicPlayer.playbackSpeed = 0.25f;
                else if (i == 1)
                    VoezEditor.Editor.musicPlayer.playbackSpeed = 0.5f;
                else if (i == 2)
                    VoezEditor.Editor.musicPlayer.playbackSpeed = 1.0f;
                else if (i == 3)
                    VoezEditor.Editor.musicPlayer.playbackSpeed = 2.0f;
                playbackTimes[i].clicked = false;
            }
        }

        // Set Selected Note Type
        for (int i = 0; i < noteTypes.Length; i += 1) {
            if (noteTypes[i].clicked) {
                for (int j = 0; j < noteTypes.Length; j += 1)
                    noteTypes[j].toggled = false;
                noteTypes[i].toggled = true;
                noteTypes[i].clicked = false;
                if (i == 0)
                    VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.CLICK;
                else if (i == 1)
                    VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SLIDE;
                else if (i == 2)
                    VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SWIPE;

                if (i == 3)
                    VoezEditor.Editor.trackEditMode = true;
                else
                    VoezEditor.Editor.trackEditMode = false;

                // Automatically close the note type selector after a choice has been made.
                for (int j = 0; j < noteTypes.Length; j += 1)
                    noteTypes[j].visible = !noteTypes[j].visible;
                notesButton.toggled = noteTypes[0].visible;
            }
        }

        // Set Grid Snapping
        for (int i = 0; i < snapTimes.Length; i += 1) {
            if (snapTimes[i].clicked) {
                for (int j = 0; j < snapTimes.Length; j += 1)
                    snapTimes[j].toggled = false;
                snapTimes[i].toggled = true;
                if (i == 0)
                    VoezEditor.Editor.selectedTimeSnap = 0;
                else if (i == 1)
                    VoezEditor.Editor.selectedTimeSnap = 16;
                else if (i == 2)
                    VoezEditor.Editor.selectedTimeSnap = 8;
                else if (i == 3)
                    VoezEditor.Editor.selectedTimeSnap = 4;
                else if (i == 4)
                    VoezEditor.Editor.selectedTimeSnap = 2;
                else if (i == 5)
                    VoezEditor.Editor.selectedTimeSnap = 1;
                snapTimes[i].clicked = false;
                grid.SnapPlaytimeToGrid();
            }
        }
    }
}
