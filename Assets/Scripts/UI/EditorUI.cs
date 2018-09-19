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
    public Button scrollButton;
    public Button soundAssistButton;
    public Button[] snapTimes;
    public Button[] playbackTimes;
    public Button[] noteTypes;
    public Button[] scrollRates;
    public Button metronomeToggle;
    public Button hitSoundToggle;
    public Button quantizationToggle;
    public Button backButton;
    public Slider playbackSlider;
    public SnapGrid grid;
    public TrackAddPreview trackAdder;
    public DropshadowLabel playbackTimeLabel;
    public DropshadowLabel levelLabel;
    public static float BUTTON_SIZE = 75f;
    public static float BUTTON_PADDING = 5f;

	public EditorUI()
    {
        float bbOrigin = BUTTON_PADDING + BUTTON_SIZE * 0.5f;

        playButton = new Button("play", new Vector2(bbOrigin, bbOrigin), BUTTON_SIZE, false);
        playbackTimeButton = new Button("time", new Vector2(bbOrigin + BUTTON_SIZE + BUTTON_PADDING, bbOrigin), BUTTON_SIZE, false);
        notesButton = new Button("click", new Vector2(bbOrigin + BUTTON_SIZE * 2 + BUTTON_PADDING * 2, bbOrigin), BUTTON_SIZE, false);
        notesButton.mySymbol.rotation = 45f;
        notesButton.mySymbol.color = Note.QUANTIZATION_COLORS[0];
        gridButton = new Button("grid", new Vector2(bbOrigin + BUTTON_SIZE * 3 + BUTTON_PADDING * 3, bbOrigin), BUTTON_SIZE, false);
        scrollButton = new Button("scroll", new Vector2(bbOrigin + BUTTON_SIZE * 4 + BUTTON_PADDING * 4, bbOrigin), BUTTON_SIZE, false);
        soundAssistButton = new Button("metronome", new Vector2(bbOrigin + BUTTON_SIZE * 5 + BUTTON_PADDING * 5, bbOrigin), BUTTON_SIZE, false);
        bpmButton = new Button("Raleway32", "BPM"+Environment.NewLine+VoezEditor.Editor.project.songBPM.ToString(), new Vector2(bbOrigin + BUTTON_SIZE * 6 + BUTTON_PADDING * 6, bbOrigin), BUTTON_SIZE, false);
        saveButton = new Button("save", new Vector2(bbOrigin + BUTTON_SIZE * 7 + BUTTON_PADDING * 7, bbOrigin), BUTTON_SIZE, false);
        VoezEditor.Editor.AddObject(playButton);
        VoezEditor.Editor.AddObject(playbackTimeButton);
        VoezEditor.Editor.AddObject(notesButton);
        VoezEditor.Editor.AddObject(gridButton);
        VoezEditor.Editor.AddObject(scrollButton);
        VoezEditor.Editor.AddObject(soundAssistButton);
        VoezEditor.Editor.AddObject(bpmButton);
        VoezEditor.Editor.AddObject(saveButton);

        float sliderStart = BUTTON_PADDING * 9 + BUTTON_SIZE * 8 + 44f;
        float sliderEnd = VoezEditor.windowRes.x - 160f;
        playbackSlider = new Slider(new Vector2((sliderStart + sliderEnd) * 0.5f, BUTTON_PADDING + BUTTON_SIZE * 0.5f), sliderEnd - sliderStart);
        VoezEditor.Editor.AddObject(playbackSlider);
        grid = new SnapGrid();
        VoezEditor.Editor.AddObject(grid);
        trackAdder = new TrackAddPreview();
        VoezEditor.Editor.AddObject(trackAdder);
        playbackTimeLabel = new DropshadowLabel("Raleway32", BeatTimeStamp(0), new Vector2(VoezEditor.windowRes.x - 75f, BUTTON_PADDING + BUTTON_SIZE * 0.5f), new Vector2(2f, -2f));
        VoezEditor.Editor.AddObject(playbackTimeLabel);

        string levelText = "";
        if (VoezEditor.editType == "easy")
            levelText = "Easy Lv." + VoezEditor.Editor.project.easyLevel.ToString();
        else if (VoezEditor.editType == "hard")
            levelText = "Hard Lv." + VoezEditor.Editor.project.hardLevel.ToString();
        else if (VoezEditor.editType == "extra")
            levelText = "Special Lv." + VoezEditor.Editor.project.extraLevel.ToString();
        levelLabel = new DropshadowLabel("Raleway32", levelText, new Vector2(VoezEditor.windowRes.x - BUTTON_PADDING*1.5f - 24f, VoezEditor.windowRes.y - BUTTON_PADDING*1.5f - 24f), new Vector2(1f, -1f));
        levelLabel.normalText.alignment = FLabelAlignment.Right;
        if (VoezEditor.editType == "easy")
            levelLabel.normalText.color = ProjectsUI.EASY_COLOR;
        else if (VoezEditor.editType == "hard")
            levelLabel.normalText.color = ProjectsUI.HARD_COLOR;
        else if (VoezEditor.editType == "extra")
            levelLabel.normalText.color = ProjectsUI.SPECIAL_COLOR;
        levelLabel.shadowText.alignment = FLabelAlignment.Right;
        VoezEditor.Editor.AddObject(levelLabel);
        backButton = new Button("back", new Vector2(bbOrigin*1.5f, VoezEditor.windowRes.y - bbOrigin*1.5f), BUTTON_SIZE*1.5f, true);
        backButton.mySymbol.scale = 1.5f;
        VoezEditor.Editor.AddObject(backButton);
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
        if (scrollRates != null) {
            for (int i = 0; i < scrollRates.Length; i += 1)
                if (scrollRates[i].visible && scrollRates[i].MouseOver)
                    return true;
        }
        if (metronomeToggle != null && (metronomeToggle.visible && metronomeToggle.MouseOver))
            return true;
        if (hitSoundToggle != null && (hitSoundToggle.visible && hitSoundToggle.MouseOver))
            return true;
        if (quantizationToggle != null && quantizationToggle.visible && quantizationToggle.MouseOver)
            return true;
        if (playbackSlider.MouseOver || playButton.MouseOver || playbackTimeButton.MouseOver || notesButton.MouseOver 
            || gridButton.MouseOver || saveButton.MouseOver || bpmButton.MouseOver || scrollButton.MouseOver 
            || soundAssistButton.MouseOver || backButton.MouseOver)
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
            if (i == 0)
                noteTypes[i].mySymbol.color = Note.QUANTIZATION_COLORS[0];
            if (i != 3)
                noteTypes[i].mySymbol.rotation = 45f;
            else
                noteTypes[i].mySymbol.scale = 1.5f;
            if (i == 1)
                noteTypes[i].mySymbol.scale = 2.0f;
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

    public void SpawnScrollButtons()
    {
        scrollRates = new Button[10];
        for (int i = 0; i < scrollRates.Length; i += 1) {
            string buttonText = (i+1).ToString()+"x";
            scrollRates[i] = new Button("Raleway32", buttonText
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + BUTTON_SIZE * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
            scrollRates[i].visible = true;
            if (VoezEditor.Editor.selectedScrollRate == i+1)
                scrollRates[i].toggled = true;
            VoezEditor.Editor.AddObject(scrollRates[i]);
        }
    }

    public void DespawnScrollButtons()
    {
        for (int i = 0; i < scrollRates.Length; i += 1)
            scrollRates[i].Destroy();
        scrollRates = null;
    }

    public void SpawnSoundAssistButtons()
    {
        int i = 5;
        metronomeToggle = new Button("metronome"
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + BUTTON_SIZE * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
        metronomeToggle.toggled = VoezEditor.Editor.metronomeEnabled;
        metronomeToggle.mySymbol.scale = 1.5f;
        VoezEditor.Editor.AddObject(metronomeToggle);
        i = 6;
        hitSoundToggle = new Button("hitsound"
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + BUTTON_SIZE * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
        hitSoundToggle.toggled = VoezEditor.Editor.hitSoundsEnabled;
        VoezEditor.Editor.AddObject(hitSoundToggle);
        i = 7;
        quantizationToggle = new Button("quantization"
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + BUTTON_SIZE * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
        quantizationToggle.mySymbol.rotation = 45f;
        quantizationToggle.mySymbol.scale = 1.5f;
        quantizationToggle.toggled = VoezEditor.Editor.quantizationEnabled;
        VoezEditor.Editor.AddObject(quantizationToggle);
    }

    public void DespawnSoundAssistButtons()
    {
        metronomeToggle.Destroy();
        hitSoundToggle.Destroy();
        quantizationToggle.Destroy();
        metronomeToggle = null;
        hitSoundToggle = null;
        quantizationToggle = null;
    }

    public void UntoggleAllSubmenus()
    {
        if (notesButton.toggled)
            notesButton.clicked = true;
        if (playbackTimeButton.toggled)
            playbackTimeButton.clicked = true;
        if (gridButton.toggled)
            gridButton.clicked = true;
        if (scrollButton.toggled)
            scrollButton.clicked = true;
        if (soundAssistButton.toggled)
            soundAssistButton.clicked = true;
    }

    public string BeatTimeStamp(float seconds)
    {
        if (VoezEditor.Editor.EditMode)
            return String.Format("{0:0.###}", seconds) + Environment.NewLine + "(" + String.Format("{0:0.###}", VoezEditor.Editor.SecondsToBeats(seconds)) + ")";
        else
            return String.Format("{0:0.###}", seconds) + Environment.NewLine + "(" + Mathf.FloorToInt(VoezEditor.Editor.SecondsToBeats(seconds)).ToString() + ")";
    }

    public void Update()
    {
        if ((InputManager.leftMousePushed && !HoveringOverSubmenuItem()) || InputManager.anyPushed) {
            UntoggleAllSubmenus();
        }

        // Exit
        if (backButton.clicked) {
            VoezEditor.Editor.readyToShutDown = true;
            backButton.clicked = false;
        }

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
                playbackTimeLabel.SetText(BeatTimeStamp(VoezEditor.Editor.songTime));
            else
                // If playback slider is being dragged, show song time at slider's current position
                playbackTimeLabel.SetText(BeatTimeStamp(VoezEditor.Editor.musicPlayer.source.clip.length * playbackSlider.pendingProgress));
        } else
            playbackSlider.allowScrubbing = false;

        // Play/Pause
        if (playButton.clicked || InputManager.spacePushed) {
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
        if (playbackSlider.loopPoint >= 0f && (playButton.rightClicked || InputManager.returnPushed || InputManager.middleMousePushed)) {
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
            if (InputManager.UpTick())
                VoezEditor.Editor.project.songBPM += 1;
            if (InputManager.DownTick())
                VoezEditor.Editor.project.songBPM -= 1;
            if (InputManager.RightTick())
                VoezEditor.Editor.project.songBPM += 5;
            if (InputManager.LeftTick())
                VoezEditor.Editor.project.songBPM -= 5;
            VoezEditor.Editor.project.songBPM = Mathf.Clamp(VoezEditor.Editor.project.songBPM, 10, 250);
            if (VoezEditor.Editor.project.songBPM != lastBPM && VoezEditor.Editor.EditMode)
                grid.SnapPlaytimeToGrid();
            bpmButton.myText.text = "BPM" + Environment.NewLine + VoezEditor.Editor.project.songBPM.ToString();
        }
        if (bpmButton.toggled && VoezEditor.Editor.bpmPulse) { 
                playbackSlider.pulseFlashEffectTime = 3;
                VoezEditor.Editor.bg.pulseFlashEffectTime = 3;
        }

        // Open Playback Speed Menu
        if (playbackTimeButton.clicked) {
            UntoggleAllSubmenus();
            if (playbackTimes == null)
                SpawnPlaybackTimeButtons();
            else
                DespawnPlaybackTimeButtons();
            InputManager.ClearMouseInputs();
            playbackTimeButton.toggled = playbackTimes != null;
            playbackTimeButton.clicked = false;
        }

        // Open Notes Selection Menu
        if (notesButton.clicked) {
            UntoggleAllSubmenus();
            if (noteTypes == null)
                SpawnNoteButtons();
            else
                DespawnNoteButtons();
            InputManager.ClearMouseInputs();
            notesButton.toggled = noteTypes != null;
            notesButton.clicked = false;
        }

        // Open Grid Snap Menu
        if (gridButton.clicked) {
            UntoggleAllSubmenus();
            if (snapTimes == null)
                SpawnGridButtons();
            else
                DespawnGridButtons();
            InputManager.ClearMouseInputs();
            gridButton.toggled = snapTimes != null;
            gridButton.clicked = false;
        }

        // Open Scroll Rate Menu
        if (scrollButton.clicked) {
            UntoggleAllSubmenus();
            if (scrollRates == null)
                SpawnScrollButtons();
            else
                DespawnScrollButtons();
            InputManager.ClearMouseInputs();
            scrollButton.toggled = scrollRates != null;
            scrollButton.clicked = false;
        }

        // Open Sound Assist Options Menu
        if (soundAssistButton.clicked) {
            UntoggleAllSubmenus();
            if (metronomeToggle == null)
                SpawnSoundAssistButtons();
            else
                DespawnSoundAssistButtons();
            InputManager.ClearMouseInputs();
            soundAssistButton.toggled = metronomeToggle != null;
            soundAssistButton.clicked = false;
        }

        // Save Project
        if (saveButton.clicked) {
            VoezEditor.Editor.project.ExportActiveProject();
            saveButton.clicked = false;
        }

        // Set Playback Speed
        if (InputManager.onePushed && !Util.ShiftDown()) {
            if (playbackTimes != null)
                playbackTimes[0].clicked = true;
            else
                VoezEditor.Editor.musicPlayer.playbackSpeed = 0.25f;
        }
        if (InputManager.twoPushed && !Util.ShiftDown()) {
            if (playbackTimes != null)
                playbackTimes[1].clicked = true;
            else
                VoezEditor.Editor.musicPlayer.playbackSpeed = 0.5f;
        }
        if (InputManager.threePushed && !Util.ShiftDown()) {
            if (playbackTimes != null)
                playbackTimes[2].clicked = true;
            else
                VoezEditor.Editor.musicPlayer.playbackSpeed = 1.0f;
        }
        if (InputManager.fourPushed && !Util.ShiftDown()) {
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
                        DespawnPlaybackTimeButtons();
                        InputManager.ClearMouseInputs();
                        playbackTimeButton.toggled = false;
                        break;
                    }
                }
            }
        }

        // Set Selected Note Type
        if (InputManager.zPushed) {
            if (noteTypes != null)
                noteTypes[0].clicked = true;
            else {
                VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.CLICK;
                notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("click");
                notesButton.mySymbol.color = Note.QUANTIZATION_COLORS[0];
                VoezEditor.Editor.trackEditMode = false;
                notesButton.mySymbol.rotation = 45f + 180f;
            }
        }
        if (InputManager.xPushed) {
            if (noteTypes != null)
                noteTypes[1].clicked = true;
            else {
                VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SLIDE;
                notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("slide");
                notesButton.mySymbol.color = Color.white;
                VoezEditor.Editor.trackEditMode = false;
                notesButton.mySymbol.rotation = 45f + 180f;
            }
        }
        if (InputManager.cPushed) {
            if (noteTypes != null)
                noteTypes[2].clicked = true;
            else {
                VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SWIPE;
                notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("swipe");
                notesButton.mySymbol.color = Color.white;
                VoezEditor.Editor.trackEditMode = false;
                notesButton.mySymbol.rotation = 45f + 180f;
            }
        }
        if (InputManager.vPushed) {
            if (noteTypes != null)
                noteTypes[3].clicked = true;
            else {
                VoezEditor.Editor.trackEditMode = true;
                notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("track");
                notesButton.mySymbol.color = Color.white;
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
                        notesButton.mySymbol.color = Note.QUANTIZATION_COLORS[0];
                    } else if (i == 1) {
                        VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SLIDE;
                        notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("slide");
                        notesButton.mySymbol.color = Color.white;
                    } else if (i == 2) {
                        VoezEditor.Editor.selectedNoteType = ProjectData.NoteData.NoteType.SWIPE;
                        notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("swipe");
                        notesButton.mySymbol.color = Color.white;
                    }

                    if (i == 3) {
                        VoezEditor.Editor.trackEditMode = true;
                        notesButton.mySymbol.element = Futile.atlasManager.GetElementWithName("track");
                        notesButton.mySymbol.color = Color.white;
                        notesButton.mySymbol.rotation = 0f;
                    } else {
                        VoezEditor.Editor.trackEditMode = false;
                        notesButton.mySymbol.rotation = 45f;
                    }

                    // Automatically close the note type selector after a choice has been made.
                    if (notesButton.toggled) {
                        DespawnNoteButtons();
                        InputManager.ClearMouseInputs();
                        notesButton.toggled = false;
                        break;
                    }
                }
            }
        }

        // Set Grid Snapping
        if (InputManager.onePushed && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[0].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 0;
        }
        if (InputManager.twoPushed && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[1].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 16;
        }
        if (InputManager.threePushed && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[2].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 8;
        }
        if (InputManager.fourPushed && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[3].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 4;
        }
        if (InputManager.fivePushed && Util.ShiftDown()) {
            if (snapTimes != null)
                snapTimes[4].clicked = true;
            else
                VoezEditor.Editor.selectedTimeSnap = 2;
        }
        if (InputManager.sixPushed && Util.ShiftDown()) {
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
                        DespawnGridButtons();
                        InputManager.ClearMouseInputs();
                        gridButton.toggled = false;
                        break;
                    }
                }
            }
        }

        // Set Scroll Rate
        if (scrollRates != null) {
            for (int i = 0; i < scrollRates.Length; i += 1) {
                if (scrollRates[i].clicked) {
                    for (int j = 0; j < scrollRates.Length; j += 1)
                        scrollRates[j].toggled = false;
                    scrollRates[i].toggled = true;
                    VoezEditor.Editor.selectedScrollRate = i + 1;
                    Note.NOTE_DURATION = Note.SCROLL_DURATIONS[i];
                    VoezEditor.Editor.RefreshAllNotes();

                    // Automatically close the scroll rate selector after a choice has been made.
                    if (scrollButton.toggled) {
                        DespawnScrollButtons();
                        InputManager.ClearMouseInputs();
                        scrollButton.toggled = false;
                        break;
                    }
                }
            }
        }

        // Set Sound Assist Options
        if (metronomeToggle != null && metronomeToggle.clicked) {
            metronomeToggle.toggled = !metronomeToggle.toggled;
            VoezEditor.Editor.metronomeEnabled = metronomeToggle.toggled;
            metronomeToggle.clicked = false;
        }
        if (hitSoundToggle != null && hitSoundToggle.clicked) {
            hitSoundToggle.toggled = !hitSoundToggle.toggled;
            VoezEditor.Editor.hitSoundsEnabled = hitSoundToggle.toggled;
            hitSoundToggle.clicked = false;
        }
        if (quantizationToggle != null && quantizationToggle.clicked) {
            quantizationToggle.toggled = !quantizationToggle.toggled;
            VoezEditor.Editor.quantizationEnabled = quantizationToggle.toggled;
            quantizationToggle.clicked = false;
        }

    }
}
