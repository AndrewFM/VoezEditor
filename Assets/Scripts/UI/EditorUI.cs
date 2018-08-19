using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorUI {
    EditorProcess parent;
    Button playButton;
    Button playbackTimeButton;
    Button notesButton;
    Button saveButton;
    Button[] playbackTimes;
    Button[] noteTypes;
    Slider playbackSlider;
    DropshadowLabel playbackTimeLabel;

	public EditorUI(EditorProcess parent)
    {
        float bbSize = 80f;
        float bbPad = 10f;
        float bbOrigin = bbPad + bbSize * 0.5f;

        this.parent = parent;
        playButton = new Button("play", new Vector2(bbOrigin, bbOrigin), bbSize, false);
        playbackTimeButton = new Button("time", new Vector2(bbOrigin + bbSize + bbPad, bbOrigin), bbSize, false);
        notesButton = new Button("notes", new Vector2(bbOrigin + bbSize * 2 + bbPad * 2, bbOrigin), bbSize, false);
        saveButton = new Button("save", new Vector2(bbOrigin + bbSize * 3 + bbPad * 3, bbOrigin), bbSize, false);
        this.parent.AddObject(playButton);
        this.parent.AddObject(playbackTimeButton);
        this.parent.AddObject(notesButton);
        this.parent.AddObject(saveButton);

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
            this.parent.AddObject(playbackTimes[i]);
        }

        noteTypes = new Button[3];
        for(int i=0; i< noteTypes.Length; i+=1) {
            string noteSymbol = "click";
            if (i == 1)
                noteSymbol = "slide";
            if (i == 2)
                noteSymbol = "swipe";
            noteTypes[i] = new Button(noteSymbol
                , new Vector2(playButton.pos.x + i * 69f + 32f, playButton.pos.y + bbSize * 0.5f + 74f + (i % 2) * 69f)
                , 128f, true);
            noteTypes[i].visible = false;
            noteTypes[i].mySymbol.rotation = 45f;
            if (i == 0)
                noteTypes[i].toggled = true;
            this.parent.AddObject(noteTypes[i]);
        }

        float sliderStart = bbPad * 5 + bbSize * 4 + 64f;
        float sliderEnd = MainScript.windowRes.x - 250f;
        playbackSlider = new Slider(new Vector2((sliderStart + sliderEnd) * 0.5f, bbPad + bbSize * 0.5f), sliderEnd - sliderStart);
        this.parent.AddObject(playbackSlider);
        playbackTimeLabel = new DropshadowLabel("Raleway24", "00:00/00:00", new Vector2(MainScript.windowRes.x - 110f, bbPad + bbSize * 0.5f), new Vector2(2f, -2f));
        this.parent.AddObject(playbackTimeLabel);
    }

    public void Update()
    {
        if (parent.musicPlayer.source.clip != null) {
            playbackSlider.progress = parent.musicPlayer.source.time / parent.musicPlayer.source.clip.length;
            playbackTimeLabel.SetText(Util.MinuteTimeStampFromSeconds((int)parent.musicPlayer.source.time).ToString() + "/" + Util.MinuteTimeStampFromSeconds((int)parent.musicPlayer.source.clip.length).ToString());
        }

        if (playButton.clicked || Input.GetKeyDown(KeyCode.Space)) {
            if (parent.musicPlayer.paused) {
                parent.musicPlayer.ResumeSong();
                playButton.mySymbol.element = Futile.atlasManager.GetElementWithName("pause");
            } else {
                parent.musicPlayer.PauseSong();
                playButton.mySymbol.element = Futile.atlasManager.GetElementWithName("play");
            }
            playButton.clicked = false;
        }
        if (playbackTimeButton.clicked) {
            if (notesButton.toggled && !playbackTimeButton.toggled)
                notesButton.clicked = true;
            for (int i = 0; i < playbackTimes.Length; i += 1)
                playbackTimes[i].visible = !playbackTimes[i].visible;
            playbackTimeButton.toggled = playbackTimes[0].visible;
            playbackTimeButton.clicked = false;
        }
        if (notesButton.clicked) {
            if (playbackTimeButton.toggled && !notesButton.toggled)
                playbackTimeButton.clicked = true;
            for (int i = 0; i < noteTypes.Length; i += 1)
                noteTypes[i].visible = !noteTypes[i].visible;
            notesButton.toggled = noteTypes[0].visible;
            notesButton.clicked = false;
        }
        if (saveButton.clicked) {
            parent.project.ExportActiveProject();
            saveButton.clicked = false;
        }
        for(int i=0; i<playbackTimes.Length; i+=1) {
            if (playbackTimes[i].clicked) {
                for (int j = 0; j < playbackTimes.Length; j += 1)
                    playbackTimes[j].toggled = false;
                playbackTimes[i].toggled = true;
                if (i == 0)
                    parent.musicPlayer.playbackSpeed = 0.25f;
                if (i == 1)
                    parent.musicPlayer.playbackSpeed = 0.5f;
                if (i == 2)
                    parent.musicPlayer.playbackSpeed = 1.0f;
                if (i == 3)
                    parent.musicPlayer.playbackSpeed = 2.0f;
                playbackTimes[i].clicked = false;
            }
        }
        for (int i = 0; i < noteTypes.Length; i += 1) {
            if (noteTypes[i].clicked) {
                for (int j = 0; j < noteTypes.Length; j += 1)
                    noteTypes[j].toggled = false;
                noteTypes[i].toggled = true;
                noteTypes[i].clicked = false;
            }
        }
    }

}
