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
    public static float BUTTON_SIZE = 80f;
    public static float BUTTON_PADDING = 10f;

	public EditorUI()
    {
        float bbOrigin = BUTTON_PADDING + BUTTON_SIZE * 0.5f;

        playButton = new Button("play", new Vector2(bbOrigin, bbOrigin), BUTTON_SIZE, false);
        playbackTimeButton = new Button("time", new Vector2(bbOrigin + BUTTON_SIZE + BUTTON_PADDING, bbOrigin), BUTTON_SIZE, false);
        notesButton = new Button("click", new Vector2(bbOrigin + BUTTON_SIZE * 2 + BUTTON_PADDING * 2, bbOrigin), BUTTON_SIZE, false);
        notesButton.mySymbol.rotation = 45f;
        gridButton = new Button("grid", new Vector2(bbOrigin + BUTTON_SIZE * 3 + BUTTON_PADDING * 3, bbOrigin), BUTTON_SIZE, false);
        bpmButton = new Button("Raleway32", "BPM"+Environment.NewLine+VoezEditor.Editor.project.songBPM.ToString(), new Vector2(bbOrigin + BUTTON_SIZE * 4 + BUTTON_PADDING * 4, bbOrigin), BUTTON_SIZE, false);
        saveButton = new Button("save", new Vector2(bbOrigin + BUTTON_SIZE * 5 + BUTTON_PADDING * 5, bbOrigin), BUTTON_SIZE, false);
        VoezEditor.Editor.AddObject(playButton);
        VoezEditor.Editor.AddObject(playbackTimeButton);
        VoezEditor.Editor.AddObject(notesButton);
        VoezEditor.Editor.AddObject(gridButton);
        VoezEditor.Editor.AddObject(bpmButton);
        VoezEditor.Editor.AddObject(saveButton);

        float sliderStart = BUTTON_PADDING * 7 + BUTTON_SIZE * 6 + 44f;
        float sliderEnd = VoezEditor.windowRes.x - 160f;
        playbackSlider = new Slider(new Vector2((sliderStart + sliderEnd) * 0.5f, BUTTON_PADDING + BUTTON_SIZE * 0.5f), sliderEnd - sliderStart);
        VoezEditor.Editor.AddObject(playbackSlider);
        grid = new SnapGrid();
        VoezEditor.Editor.AddObject(grid);
        trackAdder = new TrackAddPreview();
        VoezEditor.Editor.AddObject(trackAdder);
        playbackTimeLabel = new DropshadowLabel("Raleway32", "0.000", new Vector2(VoezEditor.windowRes.x - 75f, BUTTON_PADDING + BUTTON_SIZE * 0.5f), new Vector2(2f, -2f));
        VoezEditor.Editor.AddObject(playbackTimeLabel);
    }

    public bool HoveringOverSubmenuItem()
    {
        if (noteTypes != null) {
            for (int i = 0; i < noteTypes.Length; i += 1)
                if (noteTypes[i].visible && noteTypes[i].MouseOver)
                    return true;
        }
        if (playbackTimes != null) {
            for (int i = 0; i < playbackTimes.Length; i += 1)
                if (playbackTimes[i].visible && playbackTimes[i].MouseOver)
                    return true;
        }
        if (snapTimes != null) {
            for (int i = 0; i < snapTimes.Length; i += 1)
                if (snapTimes[i].visible && snapTimes[i].MouseOver)
                    return true;
        }
        if (playbackSlider.MouseOver || playButton.MouseOver || playbackTimeButton.MouseOver || notesButton.MouseOver || gridButton.MouseOver || saveButton.MouseOver || bpmButton.MouseOver)
            return true;
        return false;
    }

    public void SpawnPlaybackTimeButtons()
    {
        playbackTimes = new Button[4];
        for (int i = 0; i < playbackTimes.Length; i += 1) {
            string buttonText = "0.25x";
            if (i == 1)
                buttonText = "0.5x";
            if (i == 2)
                buttonText = "1.0x";
            if (i == 3)
                buttonText = "2.0x";
            playbackTimes[i] = new Button("Raleway32", buttonText
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + BUTTON_SIZE * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
            playbackTimes[i].visible = true;
            if (i == 0 && VoezEditor.Editor.musicPlayer.playbackSpeed == 0.25f)
                playbackTimes[i].toggled = true;
            if (i == 1 && VoezEditor.Editor.musicPlayer.playbackSpeed == 0.5f)
                playbackTimes[i].toggled = true;
            if (i == 2 && VoezEditor.Editor.musicPlayer.playbackSpeed == 1.0f)
                playbackTimes[i].toggled = true;
            if (i == 3 && VoezEditor.Editor.musicPlayer.playbackSpeed == 2.0f)
                playbackTimes[i].toggled = true;
            VoezEditor.Editor.AddObject(playbackTimes[i]);
        }
    }

    public void DespawnPlaybackTimeButtons()
    {
        for(int i=0; i<playbackTimes.Length; i+=1)
            playbackTimes[i].Destroy();
        playbackTimes = null;
    }

    public void SpawnNoteButtons()
    {
        noteTypes = new Button[4];
        for (int i = 0; i < noteTypes.Length; i += 1) {
            string noteSymbol = "click";
            if (i == 1)
                noteSymbol = "slide";
            if (i == 2)
                noteSymbol = "swipe";
            if (i == 3)
                noteSymbol = "track";
            noteTypes[i] = new Button(noteSymbol
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + BUTTON_SIZE * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
            noteTypes[i].visible = true;
            if (i != 3)
                noteTypes[i].mySymbol.rotation = 45f;
            else
                noteTypes[i].mySymbol.scale = 1.5f;
            if (i == 0 && VoezEditor.Editor.selectedNoteType == ProjectData.NoteData.NoteType.CLICK && !VoezEditor.Editor.trackEditMode)
                noteTypes[i].toggled = true;
            if (i == 1 && VoezEditor.Editor.selectedNoteType == ProjectData.NoteData.NoteType.SLIDE && !VoezEditor.Editor.trackEditMode)
                noteTypes[i].toggled = true;
            if (i == 2 && VoezEditor.Editor.selectedNoteType == ProjectData.NoteData.NoteType.SWIPE && !VoezEditor.Editor.trackEditMode)
                noteTypes[i].toggled = true;
            if (i == 3 && VoezEditor.Editor.trackEditMode)
                noteTypes[i].toggled = true;
            VoezEditor.Editor.AddObject(noteTypes[i]);
        }
    }

    public void DespawnNoteButtons()
    {
        for (int i = 0; i < noteTypes.Length; i += 1)
            noteTypes[i].Destroy();
        noteTypes = null;
    }

    public void SpawnGridButtons()
    {
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
            snapTimes[i] = new Button("Raleway32", buttonText
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + BUTTON_SIZE * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
            snapTimes[i].visible = true;
            if (i == 0 && VoezEditor.Editor.selectedTimeSnap == 0)
                snapTimes[i].toggled = true;
            if (i == 1 && VoezEditor.Editor.selectedTimeSnap == 16)
                snapTimes[i].toggled = true;
            if (i == 2 && VoezEditor.Editor.selectedTimeSnap == 8)
                snapTimes[i].toggled = true;
            if (i == 3 && VoezEditor.Editor.selectedTimeSnap == 4)
                snapTimes[i].toggled = true;
            if (i == 4 && VoezEditor.Editor.selectedTimeSnap == 2)
                snapTimes[i].toggled = true;
            if (i == 5 && VoezEditor.Editor.selectedTimeSnap == 1)
                snapTimes[i].toggled = true;
            VoezEditor.Editor.AddObject(snapTimes[i]);
        }
    }

    public void DespawnGridButtons()
    {
        for (int i = 0; i < snapTimes.Length; i += 1)
            snapTimes[i].Destroy();
        snapTimes = null;
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
                playbackTimeLabel.SetText(VoezEditor.Editor.songTime.ToString("0.000"));
            else
                // If playback slider is being dragged, show song time at slider's current position
                playbackTimeLabel.SetText((VoezEditor.Editor.musicPlayer.source.clip.length * playbackSlider.pendingProgress).ToString("0.000"));
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
            float lastBPM = VoezEditor.Editor.project.songBPM;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
                VoezEditor.Editor.project.songBPM += 1;
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
                VoezEditor.Editor.project.songBPM -= 1;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
                VoezEditor.Editor.project.songBPM += 5;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
                VoezEditor.Editor.project.songBPM -= 5;
            VoezEditor.Editor.project.songBPM = Mathf.Clamp(VoezEditor.Editor.project.songBPM, 10, 250);
            if (VoezEditor.Editor.project.songBPM != lastBPM && VoezEditor.Editor.EditMode)
                grid.SnapPlaytimeToGrid();
            bpmButton.myText.text = "BPM" + Environment.NewLine + VoezEditor.Editor.project.songBPM.ToString();
        }

        // BPM Pulsing in BPM Edit Mode
        if (bpmButton.toggled && VoezEditor.Editor.musicPlayer.source.isPlaying) {
            float timeIncrement = 0;
            if (VoezEditor.Editor.project.songBPM > 0) {
                float secondsPerBeat = 60f / VoezEditor.Editor.project.songBPM;
                timeIncrement = secondsPerBeat; // BPM data available; set time snap to match BPM
            } else
                timeIncrement = 1f; // No BPM data; treat time snap as beats per second -- ie: 60 BPM
            float offset = VoezEditor.Editor.songTime - (Mathf.Floor(VoezEditor.Editor.songTime / timeIncrement) * timeIncrement);

            if (offset <= 1f / VoezEditor.Editor.framesPerSecond) {
                playbackSlider.pulseFlashEffectTime = 3;
                VoezEditor.Editor.bg.pulseFlashEffectTime = 3;
            }
        }

        // Open Playback Speed Menu
        if (playbackTimeButton.clicked) {
            if (notesButton.toggled && !playbackTimeButton.toggled)
                notesButton.clicked = true; // Notes Selection Menu is already open, close it.
            if (gridButton.toggled && !playbackTimeButton.toggled)
                gridButton.clicked = true; // Grid Snap Menu is already open, close it.

            if (playbackTimes == null)
                SpawnPlaybackTimeButtons();
            else
                DespawnPlaybackTimeButtons();
            playbackTimeButton.toggled = playbackTimes != null;
            playbackTimeButton.clicked = false;
        }

        // Open Notes Selection Menu
        if (notesButton.clicked) {
            if (playbackTimeButton.toggled && !notesButton.toggled)
                playbackTimeButton.clicked = true; // Playback Speed Menu is already open, close it.
            if (gridButton.toggled && !notesButton.toggled)
                gridButton.clicked = true; // Grid Snap Menu is already open, close it.

            if (noteTypes == null)
                SpawnNoteButtons();
            else
                DespawnNoteButtons();
            notesButton.toggled = noteTypes != null;
            notesButton.clicked = false;
        }

        // Open Grid Snap Menu
        if (gridButton.clicked) {
            if (notesButton.toggled && !gridButton.toggled)
                notesButton.clicked = true; // Notes Selection Menu is already open, close it.
            if (playbackTimeButton.toggled && !gridButton.toggled)
                playbackTimeButton.clicked = true; // Playback Speed Menu is already open, close it.

            if (snapTimes == null)
                SpawnGridButtons();
            else
                DespawnGridButtons();
            gridButton.toggled = snapTimes != null;
            gridButton.clicked = false;
        }

        // Save Project
        if (saveButton.clicked) {
            VoezEditor.Editor.project.ExportActiveProject();
            saveButton.clicked = false;
        }

        // Set Playback Speed
        if (Input.GetKeyDown(KeyCode.Alpha1) && !Util.ShiftDown()) {
            if (playbackTimes != null)
                playbackTimes[0].clicked = true;
            else
                VoezEditor.Editor.musicPlayer.playbackSpeed = 0.25f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !Util.ShiftDown()) {
            if (playbackTimes != null)
                playbackTimes[1].clicked = true;
            else
                VoezEditor.Editor.musicPlayer.playbackSpeed = 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && !Util.ShiftDown()) {
            if (playbackTimes != null)
                playbackTimes[2].clicked = true;
            else
                VoezEditor.Editor.musicPlayer.playbackSpeed = 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && !Util.ShiftDown()) {
            if (playbackTimes != null)
                playbackTimes[3].clicked = true;
            else
                VoezEditor.Editor.musicPlayer.playbackSpeed = 2.0f;
        }
        if (playbackTimes != null) {
            for (int i = 0; i < playbackTimes.Length; i += 1) {
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

                    // Automatically close the playback time selector after a choice has been made.
                    if (playbackTimeButton.toggled) {
                        for (int j = 0; j < playbackTimes.Length; j += 1)
                            playbackTimes[j].visible = false;
                        playbackTimeButton.toggled = false;
                    }
                }
            }
        }

        // Set Selected Note Type
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (noteTypes != null)
                noteTypes[0].clicked = true;
            else {
                VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.CLICK;
                notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("click");
                VoezEditor.Editor.trackEditMode = false;
                notesButton.mySymbol.rotation = 45f + 180f;
            }
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            if (noteTypes != null)
                noteTypes[1].clicked = true;
            else {
                VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SLIDE;
                notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("slide");
                VoezEditor.Editor.trackEditMode = false;
                notesButton.mySymbol.rotation = 45f + 180f;
            }
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            if (noteTypes != null)
                noteTypes[2].clicked = true;
            else {
                VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SWIPE;
                notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("swipe");
                VoezEditor.Editor.trackEditMode = false;
                notesButton.mySymbol.rotation = 45f + 180f;
            }
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            if (noteTypes != null)
                noteTypes[3].clicked = true;
            else {
                VoezEditor.Editor.trackEditMode = true;
                notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("track");
                notesButton.mySymbol.rotation = 0f;
            }
        }
        if (noteTypes != null) {
            for (int i = 0; i < noteTypes.Length; i += 1) {
                if (noteTypes[i].clicked) {
                    for (int j = 0; j < noteTypes.Length; j += 1)
                        noteTypes[j].toggled = false;
                    noteTypes[i].toggled = true;
                    noteTypes[i].clicked = false;
                    if (i == 0) {
                        VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.CLICK;
                        notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("click");
                    } else if (i == 1) {
                        VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SLIDE;
                        notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("slide");
                    } else if (i == 2) {
                        VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SWIPE;
                        notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("swipe");
                    }

                    if (i == 3) {
                        VoezEditor.Editor.trackEditMode = true;
                        notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("track");
                        notesButton.mySymbol.rotation = 0f;
                    } else {
                        VoezEditor.Editor.trackEditMode = false;
                        notesButton.mySymbol.rotation = 45f;
                    }

                    // Automatically close the note type selector after a choice has been made.
                    if (notesButton.toggled) {
                        for (int j = 0; j < noteTypes.Length; j += 1)
                            noteTypes[j].visible = false;
                        notesButton.toggled = false;
                    }
                }
            }
        }

        // Set Grid Snapping
        if (Input.GetKeyDown(KeyCode.Alpha1) && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[0].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[1].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 16;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[2].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[3].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[4].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[5].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 1;
        }
        if (snapTimes != null) {
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

                    // Automatically close the grid snap selector after a choice has been made.
                    if (gridButton.toggled) {
                        for (int j = 0; j < snapTimes.Length; j += 1)
                            snapTimes[j].visible = false;
                        gridButton.toggled = false;
                    }
                }
            }
        }
    }
}
