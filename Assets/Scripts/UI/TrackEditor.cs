using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class TrackEditor : UIElement {
    public Button moveButton;
    public Button scaleButton;
    public Button colorButton;
    public FLabel valueLabel;
    public FLabel startLabel;
    public FLabel endLabel;
    public TransformationList keyframeEditor;
    public RectangleBorder border;
    public ProjectData.TrackData data;
    public static float WIDTH = 400f;
    public static float LINE_HEIGHT = 21f;
    public static float LINES_START = 60f;
    public static float BUTTON_SIZE = 38f;
    public static float HEIGHT = TransformationList.HEIGHT + 250f;
    public int selectedLine;
    public int numLines;
    public bool init;
    public static ProjectData.TrackTransformation.TransformType page; // static so last visited page persists after transformation editor close and across transfomration editors

    public int Spr_Back { get { return 0; } }
    public int Spr_Hover { get { return 1; } }
    public int Spr_Selector { get { return 2; } }

    public TrackEditor(Vector2 pos, ProjectData.TrackData data)
    {
        this.data = data;
        this.pos = pos;
        numLines = 3;

        valueLabel = new FLabel("Raleway16", "Value: ");
        startLabel = new FLabel("Raleway16", "Spawn Time: "+VoezEditor.Editor.BeatTimeStamp(data.start));
        endLabel = new FLabel("Raleway16", "Despawn Time: "+ VoezEditor.Editor.BeatTimeStamp(data.end));
    }

    public void SetPage(ProjectData.TrackTransformation.TransformType pageID)
    {
        selectedLine = 2;
        page = pageID;
        RefreshValueLabel();
        if (keyframeEditor != null)
            keyframeEditor.Destroy();
        keyframeEditor = new TransformationList(this, pageID, new Vector2(pos.x, pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * (0.5f + numLines) - 25 - TransformationList.HEIGHT * 0.5f));
        VoezEditor.Editor.AddObject(keyframeEditor);
        colorButton.toggled = false;
        moveButton.toggled = false;
        scaleButton.toggled = false;
        if (page == ProjectData.TrackTransformation.TransformType.COLOR)
            colorButton.toggled = true;
        else if (page == ProjectData.TrackTransformation.TransformType.MOVE)
            moveButton.toggled = true;
        else
            scaleButton.toggled = true;
    }

    public void RefreshValueLabel()
    {
        if (page == ProjectData.TrackTransformation.TransformType.COLOR) {
            valueLabel.text = "Color: " + ProjectData.colorNames[data.color];
            valueLabel.color = ProjectData.colors[data.color];
        } else
            valueLabel.color = Color.white;
        if (page == ProjectData.TrackTransformation.TransformType.MOVE)
            valueLabel.text = "Position: " + Mathf.FloorToInt(data.x * 100f).ToString() + "%";
        if (page == ProjectData.TrackTransformation.TransformType.SCALE)
            valueLabel.text = "Scale: " + data.size.ToString("0.00") + "x";
    }

    public bool MouseOver
    {
        get {
            Vector3 mouse = Input.mousePosition;
            return mouse.x >= pos.x - WIDTH * 0.5f && mouse.x <= pos.x + WIDTH * 0.5f && mouse.y >= pos.y - HEIGHT * 0.5f && mouse.y <= pos.y + HEIGHT * 0.5f;
        }
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[3];

        sGroup.sprites[Spr_Back] = new FSprite("Futile_White");
        sGroup.sprites[Spr_Back].color = Color.black;
        sGroup.sprites[Spr_Back].scaleX = (WIDTH + 5) / sGroup.sprites[Spr_Back].width;
        sGroup.sprites[Spr_Back].scaleY = (HEIGHT + 5) / sGroup.sprites[Spr_Back].height;
        sGroup.sprites[Spr_Back].alpha = 0.8f;

        sGroup.sprites[Spr_Selector] = new FSprite("Futile_White");
        sGroup.sprites[Spr_Selector].scaleX = WIDTH / sGroup.sprites[Spr_Selector].width;
        sGroup.sprites[Spr_Selector].scaleY = LINE_HEIGHT / sGroup.sprites[Spr_Selector].height;
        sGroup.sprites[Spr_Selector].alpha = 0.5f;
        sGroup.sprites[Spr_Selector].color = Color.red;
        sGroup.sprites[Spr_Selector].anchorY = 1f;

        sGroup.sprites[Spr_Hover] = new FSprite("Futile_White");
        sGroup.sprites[Spr_Hover].scaleX = WIDTH / sGroup.sprites[Spr_Hover].width;
        sGroup.sprites[Spr_Hover].scaleY = LINE_HEIGHT / sGroup.sprites[Spr_Hover].height;
        sGroup.sprites[Spr_Hover].alpha = 0.8f;
        sGroup.sprites[Spr_Hover].color = Color.gray;
        sGroup.sprites[Spr_Hover].anchorY = 1f;
    }

    public override void Update()
    {
        if (!init) {
            init = true;
            moveButton = new Button("move", new Vector2(pos.x - WIDTH * 0.5f + 20f + 5f + BUTTON_SIZE * 0.5f, pos.y + HEIGHT * 0.5f - 15f - BUTTON_SIZE * 0.5f), BUTTON_SIZE, false);
            scaleButton = new Button("scale", new Vector2(pos.x - WIDTH * 0.5f + 20f + 10f + BUTTON_SIZE * 0.5f + BUTTON_SIZE, pos.y + HEIGHT * 0.5f - 15f - BUTTON_SIZE * 0.5f), BUTTON_SIZE, false);
            colorButton = new Button("color", new Vector2(pos.x - WIDTH * 0.5f + 20f + 15f + BUTTON_SIZE * 0.5f + BUTTON_SIZE * 2f, pos.y + HEIGHT * 0.5f - 15f - BUTTON_SIZE * 0.5f), BUTTON_SIZE, false);
            VoezEditor.Editor.AddObject(moveButton);
            VoezEditor.Editor.AddObject(scaleButton);
            VoezEditor.Editor.AddObject(colorButton);

            border = new RectangleBorder(new Rect(new Vector2(pos.x - WIDTH * 0.5f + 5, pos.y - HEIGHT * 0.5f + 5), new Vector2(WIDTH - 10, HEIGHT - 10)), 3f);
            VoezEditor.Editor.AddObject(border);
            SetPage(page);
        }
        border.pos = pos;

        if (InputManager.leftMousePushed && !MouseOver)
            Destroy();
        //if (!VoezEditor.Editor.musicPlayer.paused)
        //    Destroy();

        if (moveButton.clicked) {
            SetPage(ProjectData.TrackTransformation.TransformType.MOVE);
            moveButton.clicked = false;
        }
        if (scaleButton.clicked) {
            SetPage(ProjectData.TrackTransformation.TransformType.SCALE);
            scaleButton.clicked = false;
        }
        if (colorButton.clicked) {
            SetPage(ProjectData.TrackTransformation.TransformType.COLOR);
            colorButton.clicked = false;
        }

        float delta = 0;
        float baseDelta = VoezEditor.Editor.GetBPMTimeIncrement();
        if (!VoezEditor.Editor.ui.bpmButton.toggled) {
            if (InputManager.UpTick())
                delta = 1f;
            if (InputManager.DownTick())
                delta = -1f;
            if (InputManager.RightTick())
                delta = 4f;
            if (InputManager.LeftTick())
                delta = -4f;
        }

        if (keyframeEditor.transSelected >= 0)
            selectedLine = -1;

        if (delta != 0) {
            if (selectedLine == 0) {
                if (data.move.Count == 0 && data.colorChange.Count == 0 && data.scale.Count == 0)
                    data.start = Mathf.Clamp(data.start + delta * VoezEditor.Editor.GetBPMTimeIncrement(), 0f, data.end);
                else {
                    data.start = Mathf.Clamp(data.start + delta * VoezEditor.Editor.GetBPMTimeIncrement(), 0f,
                                             Mathf.Min(data.move.Count == 0 ? int.MaxValue : data.move[0].start, 
                                                       data.scale.Count == 0 ? int.MaxValue : data.scale[0].start,
                                                       data.colorChange.Count == 0 ? int.MaxValue : data.colorChange[0].start));
                }
                startLabel.text = "Spawn Time: " + VoezEditor.Editor.BeatTimeStamp(data.start);
                VoezEditor.Editor.JumpToTime(data.start);
            }
            if (selectedLine == 1) {
                if (data.move.Count == 0 && data.colorChange.Count == 0 && data.scale.Count == 0)
                    data.end = Mathf.Clamp(data.end + delta * VoezEditor.Editor.GetBPMTimeIncrement(), data.start, VoezEditor.Editor.musicPlayer.source.clip.length);
                else {
                    data.end = Mathf.Clamp(data.end + delta * VoezEditor.Editor.GetBPMTimeIncrement(),
                                           Mathf.Max(data.move.Count == 0 ? 0 : data.move[data.move.Count-1].end,
                                                     data.scale.Count == 0 ? 0 : data.scale[data.scale.Count-1].end,
                                                     data.colorChange.Count == 0 ? 0 : data.colorChange[data.colorChange.Count-1].end),
                                           VoezEditor.Editor.musicPlayer.source.clip.length);
                }
                endLabel.text = "Despawn Time: " + VoezEditor.Editor.BeatTimeStamp(data.end);
                VoezEditor.Editor.JumpToTime(data.end);
            }
            if (selectedLine == 2) {
                if (page == ProjectData.TrackTransformation.TransformType.COLOR) {
                    data.color += (int)Mathf.Sign(delta);
                    if (data.color < 0)
                        data.color = ProjectData.colors.Length - 1;
                    if (data.color > ProjectData.colors.Length - 1)
                        data.color = 0;
                } else if (page == ProjectData.TrackTransformation.TransformType.SCALE)
                    data.size = Mathf.Clamp(data.size + 0.1f * delta, 0f, 10f);
                else if (page == ProjectData.TrackTransformation.TransformType.MOVE)
                    data.x = Mathf.RoundToInt(Mathf.Clamp(data.x + 0.01f * delta, 0f, 1f)*100f)/100f;
                RefreshValueLabel();
            }
            if (selectedLine <= 1)
                VoezEditor.Editor.RefreshTrack(data.id);
        }

        // Mouse Slide Value Editing
        if (Input.GetMouseButton(1) && !VoezEditor.Editor.ui.HoveringOverSubmenuItem()) {
            if (selectedLine == 2 && page == ProjectData.TrackTransformation.TransformType.MOVE) {
                VoezEditor.Editor.ui.trackAdder.previewScale = 1f; 
                VoezEditor.Editor.ui.trackAdder.previewX = -1f; // will default to following the mouse
                data.x = Util.InvScreenPosX(VoezEditor.Editor.ui.trackAdder.pos.x);
                RefreshValueLabel();
            }
            if (selectedLine == 2 && page == ProjectData.TrackTransformation.TransformType.SCALE) {
                Track myTrack = null;
                for(int i=0; i<VoezEditor.Editor.activeTracks.Count; i+=1) {
                    if (VoezEditor.Editor.activeTracks[i].ID == data.id) {
                        myTrack = VoezEditor.Editor.activeTracks[i];
                        break;
                    }
                }
                if (myTrack == null)
                    VoezEditor.Editor.ui.trackAdder.previewX = data.x;
                else
                    VoezEditor.Editor.ui.trackAdder.previewX = Util.InvScreenPosX(myTrack.pos.x);
                VoezEditor.Editor.ui.trackAdder.previewScale = Mathf.Abs(Input.mousePosition.x - Util.ScreenPosX(VoezEditor.Editor.ui.trackAdder.previewX))/(VoezEditor.windowRes.x*Track.TRACK_SCREEN_WIDTH);
                VoezEditor.Editor.ui.trackAdder.previewScale = Mathf.Clamp(VoezEditor.Editor.ui.trackAdder.previewScale*2f, 0f, 10f);
                data.size = VoezEditor.Editor.ui.trackAdder.previewScale;
                RefreshValueLabel();
            }
        }
        else if (Input.GetMouseButtonUp(1)) {
            VoezEditor.Editor.ui.trackAdder.previewScale = -1f;
            VoezEditor.Editor.ui.trackAdder.previewX = -1f;
        }

        base.Update();
    }

    public override void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
    {
        foreach (FSprite fsprite in sGroup.sprites) {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
        newContainer.AddChild(startLabel);
        newContainer.AddChild(endLabel);
        newContainer.AddChild(valueLabel);
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        Vector2 drawPos = new Vector2(Mathf.Lerp(lastPos.x, pos.x, frameProgress), Mathf.Lerp(lastPos.y, pos.y, frameProgress));
        sGroup.sprites[Spr_Back].x = drawPos.x;
        sGroup.sprites[Spr_Back].y = drawPos.y;

        Vector3 mouse = Input.mousePosition;
        sGroup.sprites[Spr_Hover].isVisible = false;
        for (int i = 0; i < numLines; i += 1) {
            if (mouse.x >= pos.x - WIDTH * 0.5f && mouse.x <= pos.x + WIDTH * 0.5f && mouse.y < pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * i && mouse.y >= pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * (i + 1)) {
                sGroup.sprites[Spr_Hover].isVisible = true;
                sGroup.sprites[Spr_Hover].x = drawPos.x;
                sGroup.sprites[Spr_Hover].y = drawPos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * i;

                if (InputManager.leftMousePushed) {
                    selectedLine = i;
                    keyframeEditor.transSelected = -1;
                    
                    // Jump playback time to the time being referenced/edited
                    if (selectedLine == 0 || selectedLine == 2)
                        VoezEditor.Editor.JumpToTime(data.start);
                    if (selectedLine == 1)
                        VoezEditor.Editor.JumpToTime(data.end);
                }
            }
        }

        sGroup.sprites[Spr_Selector].x = drawPos.x;
        sGroup.sprites[Spr_Selector].y = drawPos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * selectedLine;
        if (selectedLine < 0)
            sGroup.sprites[Spr_Selector].isVisible = false;
        else
            sGroup.sprites[Spr_Selector].isVisible = true;

        startLabel.x = pos.x - WIDTH * 0.5f + 20f + startLabel.textRect.width * 0.5f;
        endLabel.x = pos.x - WIDTH * 0.5f + 20f + endLabel.textRect.width * 0.5f;
        valueLabel.x = pos.x - WIDTH * 0.5f + 20f + valueLabel.textRect.width * 0.5f;

        startLabel.y = pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * 0.5f;
        endLabel.y = pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * 1.5f;
        valueLabel.y = pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * 2.5f;

        base.DrawSprites(sGroup, frameProgress);
    }

    public override void Destroy()
    {
        base.Destroy();
        moveButton.Destroy();
        scaleButton.Destroy();
        colorButton.Destroy();
        keyframeEditor.Destroy();
        border.Destroy();
        startLabel.RemoveFromContainer();
        endLabel.RemoveFromContainer();
        valueLabel.RemoveFromContainer();
        VoezEditor.Editor.trackEditor = null;
        VoezEditor.Editor.ui.trackAdder.previewScale = -1f;
        VoezEditor.Editor.ui.trackAdder.previewX = -1f;
    }
}
