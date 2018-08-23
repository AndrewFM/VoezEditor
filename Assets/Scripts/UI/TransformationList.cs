using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TransformationList : UIElement {
    public TrackEditor parent;
    public List<ProjectData.TrackTransformation> transList;
    public TransformationItem[] transUIElems;
    public Button addButton;
    public Button deleteButton;
    public Button nextPageButton;
    public Button prevPageButton;
    public Button copyButton;
    public Button pasteButton;
    public Button mirrorButton;
    public FLabel pageLabel;
    public FLabel titleLabel;
    public RectangleBorder border;
    public ProjectData.TrackTransformation.TransformType type;
    public bool init;
    public static float WIDTH = 350f;
    public static float LINE_HEIGHT = 21f;
    public static float LINES_START = 10f;
    public static int TRANS_PER_PAGE = 4;
    public static float HEIGHT = LINE_HEIGHT * 4 * TRANS_PER_PAGE + LINES_START * 2;
    public int transSelected;
    public int itemSelected;
    public int page;

    public TransformationList(TrackEditor parent, ProjectData.TrackTransformation.TransformType type, Vector2 pos) {
        this.type = type;
        this.pos = pos;
        this.parent = parent;
        if (type == ProjectData.TrackTransformation.TransformType.MOVE)
            transList = parent.data.move;
        else if (type == ProjectData.TrackTransformation.TransformType.SCALE)
            transList = parent.data.scale;
        else
            transList = parent.data.colorChange;
        transUIElems = new TransformationItem[TRANS_PER_PAGE];
        transSelected = -1;
        pageLabel = new FLabel("Raleway16", "Page");
        titleLabel = new FLabel("Raleway24", "Keyframes:");
    }

    public int TotalPages
    {
        get { return Mathf.FloorToInt((transList.Count - 1) / transUIElems.Length) + 1;  }
    }

    public override void Update()
    {
        // Post-Init
        if (!init) {
            float buttonSize = 38f;
            float baseX = pos.x - WIDTH * 0.5f + 10 + buttonSize * 0.5f;
            float baseY = pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * (transUIElems.Length * 4) - 20 - buttonSize * 0.5f;
            float spacing = 5;
            prevPageButton = new Button("prev", new Vector2(baseX, baseY), buttonSize, false);
            nextPageButton = new Button("next", new Vector2(baseX + (buttonSize + spacing) * 1f, baseY), buttonSize, false);
            addButton = new Button("add", new Vector2(baseX + (buttonSize + spacing) * 2f, baseY), buttonSize, false);
            deleteButton = new Button("delete", new Vector2(baseX + (buttonSize + spacing) * 3f, baseY), buttonSize, false);
            copyButton = new Button("copy", new Vector2(baseX + (buttonSize + spacing) * 4f, baseY), buttonSize, false);
            pasteButton = new Button("paste", new Vector2(baseX + (buttonSize + spacing) * 5f, baseY), buttonSize, false);
            pageLabel.x = pos.x;
            pageLabel.y = baseY - buttonSize * 0.5f - spacing - pageLabel.textRect.height * 0.5f;
            titleLabel.x = pos.x;
            titleLabel.y = pos.y + HEIGHT * 0.5f + LINE_HEIGHT;
            VoezEditor.Editor.AddObject(prevPageButton);
            VoezEditor.Editor.AddObject(nextPageButton);
            VoezEditor.Editor.AddObject(addButton);
            VoezEditor.Editor.AddObject(deleteButton);
            VoezEditor.Editor.AddObject(copyButton);
            VoezEditor.Editor.AddObject(pasteButton);
            if (type == ProjectData.TrackTransformation.TransformType.MOVE) {
                mirrorButton = new Button("mirror", new Vector2(baseX + (buttonSize + spacing) * 6f, baseY), buttonSize, false);
                VoezEditor.Editor.AddObject(mirrorButton);
            }

            border = new RectangleBorder(new Rect(new Vector2(pos.x - WIDTH * 0.5f, pos.y - HEIGHT * 0.5f), new Vector2(WIDTH, HEIGHT)), 3f);
            VoezEditor.Editor.AddObject(border);
            RefreshPages();
            init = true;
        }

        // Edit Values
        if (transSelected >= 0 && transUIElems[transSelected % transUIElems.Length] != null) {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
                transUIElems[transSelected % transUIElems.Length].UpdateValue(itemSelected, 1);
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
                transUIElems[transSelected % transUIElems.Length].UpdateValue(itemSelected, -1);
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
                transUIElems[transSelected % transUIElems.Length].UpdateValue(itemSelected, 5);
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
                transUIElems[transSelected % transUIElems.Length].UpdateValue(itemSelected, -5);
        }

        // Mouse Slide Value Editing
        if (Input.GetMouseButton(1)) {
            if (transSelected >= 0 && itemSelected == 0 && type == ProjectData.TrackTransformation.TransformType.MOVE) {
                VoezEditor.Editor.ui.trackAdder.previewScale = 1f;
                VoezEditor.Editor.ui.trackAdder.previewX = -1f; // will default to following the mouse
                transUIElems[transSelected % transUIElems.Length].data.to = Util.InvScreenPosX(VoezEditor.Editor.ui.trackAdder.pos.x);
                transUIElems[transSelected % transUIElems.Length].RefreshLabelValues();
            }
            if (transSelected >= 0 && itemSelected == 0 && type == ProjectData.TrackTransformation.TransformType.SCALE) {
                Track myTrack = null;
                for (int i = 0; i < VoezEditor.Editor.activeTracks.Count; i += 1) {
                    if (VoezEditor.Editor.activeTracks[i].ID == parent.data.id) {
                        myTrack = VoezEditor.Editor.activeTracks[i];
                        break;
                    }
                }
                if (myTrack == null)
                    VoezEditor.Editor.ui.trackAdder.previewX = parent.data.x;
                else
                    VoezEditor.Editor.ui.trackAdder.previewX = Util.InvScreenPosX(myTrack.pos.x);
                VoezEditor.Editor.ui.trackAdder.previewScale = Mathf.Abs(Input.mousePosition.x - Util.ScreenPosX(VoezEditor.Editor.ui.trackAdder.previewX)) / (VoezEditor.windowRes.x * Track.TRACK_SCREEN_WIDTH);
                VoezEditor.Editor.ui.trackAdder.previewScale = Mathf.Clamp(VoezEditor.Editor.ui.trackAdder.previewScale * 2f, 0f, 10f);
                transUIElems[transSelected % transUIElems.Length].data.to = VoezEditor.Editor.ui.trackAdder.previewScale;
                transUIElems[transSelected % transUIElems.Length].RefreshLabelValues();
            }
        } else if (Input.GetMouseButtonUp(1)) {
            VoezEditor.Editor.RefreshTrack(parent.data.id);
            VoezEditor.Editor.ui.trackAdder.previewScale = -1f;
            VoezEditor.Editor.ui.trackAdder.previewX = -1f;
        }

        // Page Navigation
        if (prevPageButton.clicked) {
            if (page > 0) {
                if (transSelected >= 0)
                    transSelected = Mathf.Clamp(transSelected - transUIElems.Length, 0, transList.Count-1);
                else
                    page -= 1;
                RefreshPages();
            }
            prevPageButton.clicked = false;
        }
        if (nextPageButton.clicked) {
            if (page < TotalPages-1) {
                if (transSelected >= 0)
                    transSelected = Mathf.Clamp(transSelected + transUIElems.Length, 0, transList.Count - 1);
                else
                    page += 1;
                RefreshPages();
            }
            nextPageButton.clicked = false;
        }

        // Add and Delete Transformations
        if (addButton.clicked) {
            ProjectData.TrackTransformation transformation = new ProjectData.TrackTransformation();
            if (transSelected < 0)
                transSelected = transList.Count - 1; // If no item selected, default to adding at end of trans list.
            if (transList.Count == 0) {
                transformation.start = parent.data.start;
                transformation.end = parent.data.end;
            }
            else if (Util.ShiftDown()) {
                // Add Before selection
                if (transSelected == 0)
                    transformation.start = parent.data.start;
                else
                    transformation.start = transList[transSelected - 1].end;
                transformation.end = transList[transSelected].start;
            }
            else {
                // Add After Selection
                if (transSelected == transList.Count - 1)
                    transformation.end = parent.data.end;
                else
                    transformation.end = transList[transSelected + 1].start;
                transformation.start = transList[transSelected].end;
            }
            if (transformation.end - transformation.start != 0) {
                transList.Add(transformation);
                transSelected = Mathf.Clamp(transSelected + 1, 0, transList.Count - 1);
                ResortDataList();
                RefreshPages();
                VoezEditor.Editor.RefreshTrack(parent.data.id);
            }
            addButton.clicked = false;
        }
        if (deleteButton.clicked) {
            if (transList.Count > 0 && transSelected >= 0) {
                transList.RemoveAt(transSelected);
                transSelected -= 1;
                RefreshPages();
                VoezEditor.Editor.RefreshTrack(parent.data.id);
            }
            deleteButton.clicked = false;
        }

        // Copy and Paste Transformations
        if (copyButton.clicked) {
            VoezEditor.Editor.project.transformClipboard = ProjectData.DeepCopyTransformationList(transList);
            VoezEditor.Editor.project.clipboardContentsType = type;
            copyButton.clicked = false;
        }
        if (pasteButton.clicked) {
            if (VoezEditor.Editor.project.transformClipboard != null && VoezEditor.Editor.project.clipboardContentsType == type) {
                ProjectData.ReplaceTransformationList(VoezEditor.Editor.project.transformClipboard, transList);
                page = 0;
                transSelected = -1;
                RefreshPages();
                VoezEditor.Editor.RefreshTrack(parent.data.id);
            }
            pasteButton.clicked = false;
        }

        // Mirror X
        if (mirrorButton != null && mirrorButton.clicked) {
            parent.data.x = 1f - parent.data.x;
            for (int i = 0; i < transList.Count; i += 1)
                transList[i].to = 1f - transList[i].to;
            VoezEditor.Editor.RefreshTrack(parent.data.id);
            RefreshPages();
            parent.RefreshValueLabel();
            mirrorButton.clicked = false;
        }

        if (transSelected != -1)
            parent.selectedLine = -1;

        base.Update();
    }

    public int Spr_BigSelector { get { return 2; } }
    public int Spr_SmallSelector { get { return 3; } }
    public int Spr_BigHover { get { return 0; } }
    public int Spr_SmallHover { get { return 1; } }
    public int Spr_DividerLines { get { return 4;  } }
    public int TotalSprites { get { return Spr_DividerLines + (TRANS_PER_PAGE - 1); } }

    // The earliest the given keyframe is allowed to start at, so it doesn't overlap with its neighboring keyframe.
    public float StartTimeBound(ProjectData.TrackTransformation item)
    {
        int index = 0;
        for(int i=0; i< transList.Count; i+=1) {
            if (item == transList[i]) {
                index = i;
                break;
            }
        }
        if (index == 0)
            return parent.data.start;
        return transList[index-1].end;
    }

    // The latest the given keyframe is allowed to end at, so it doesn't overlap with its neighboring keyframe.
    public float EndTimeBound(ProjectData.TrackTransformation item)
    {
        int index = 0;
        for (int i = 0; i < transList.Count; i += 1) {
            if (item == transList[i]) {
                index = i;
                break;
            }
        }
        if (index == transList.Count - 1)
            return parent.data.end;
        return transList[index+1].start;
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[TotalSprites];

        sGroup.sprites[Spr_SmallSelector] = new FSprite("Futile_White");
        sGroup.sprites[Spr_SmallSelector].scaleX = WIDTH / sGroup.sprites[Spr_SmallSelector].width;
        sGroup.sprites[Spr_SmallSelector].scaleY = LINE_HEIGHT / sGroup.sprites[Spr_SmallSelector].height;
        sGroup.sprites[Spr_SmallSelector].alpha = 0.5f;
        sGroup.sprites[Spr_SmallSelector].color = Color.red;
        sGroup.sprites[Spr_SmallSelector].anchorY = 1f;

        sGroup.sprites[Spr_BigSelector] = new FSprite("Futile_White");
        sGroup.sprites[Spr_BigSelector].scaleX = WIDTH / sGroup.sprites[Spr_BigSelector].width;
        sGroup.sprites[Spr_BigSelector].scaleY = (LINE_HEIGHT*4) / sGroup.sprites[Spr_BigSelector].height;
        sGroup.sprites[Spr_BigSelector].alpha = 0.3f;
        sGroup.sprites[Spr_BigSelector].color = Color.red;
        sGroup.sprites[Spr_BigSelector].anchorY = 1f;

        sGroup.sprites[Spr_SmallHover] = new FSprite("Futile_White");
        sGroup.sprites[Spr_SmallHover].scaleX = WIDTH / sGroup.sprites[Spr_SmallHover].width;
        sGroup.sprites[Spr_SmallHover].scaleY = LINE_HEIGHT / sGroup.sprites[Spr_SmallHover].height;
        sGroup.sprites[Spr_SmallHover].alpha = 0.8f;
        sGroup.sprites[Spr_SmallHover].color = Color.gray;
        sGroup.sprites[Spr_SmallHover].anchorY = 1f;

        sGroup.sprites[Spr_BigHover] = new FSprite("Futile_White");
        sGroup.sprites[Spr_BigHover].scaleX = WIDTH / sGroup.sprites[Spr_BigHover].width;
        sGroup.sprites[Spr_BigHover].scaleY = (LINE_HEIGHT * 4) / sGroup.sprites[Spr_BigHover].height;
        sGroup.sprites[Spr_BigHover].alpha = 0.3f;
        sGroup.sprites[Spr_BigHover].color = Color.gray;
        sGroup.sprites[Spr_BigHover].anchorY = 1f;

        for(int i=Spr_DividerLines; i<Spr_DividerLines + (TRANS_PER_PAGE-1); i+=1) {
            sGroup.sprites[i] = new FSprite("Futile_White");
            sGroup.sprites[i].scaleX = WIDTH / sGroup.sprites[i].width;
            sGroup.sprites[i].scaleY = 2f / sGroup.sprites[i].height;
            sGroup.sprites[i].alpha = 0.8f;
            sGroup.sprites[i].anchorY = 1f;
        }
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        Vector2 drawPos = new Vector2(Mathf.Lerp(lastPos.x, pos.x, frameProgress), Mathf.Lerp(lastPos.y, pos.y, frameProgress));

        Vector3 mouse = Input.mousePosition;
        sGroup.sprites[Spr_SmallHover].isVisible = false;
        sGroup.sprites[Spr_BigHover].isVisible = false;
        for (int i = 0; i < 4 * transUIElems.Length; i += 1) {
            int trans = (page * transUIElems.Length) + Mathf.FloorToInt(i / 4);
            if (mouse.x >= pos.x - WIDTH * 0.5f && mouse.x <= pos.x + WIDTH * 0.5f && mouse.y < pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * i && mouse.y >= pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * (i+1)) {
                if (transUIElems[trans % transUIElems.Length] == null)
                    continue;
                int item = i % 4;
                
                sGroup.sprites[Spr_SmallHover].isVisible = true;
                sGroup.sprites[Spr_SmallHover].x = drawPos.x;
                sGroup.sprites[Spr_SmallHover].y = drawPos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * i;
                sGroup.sprites[Spr_BigHover].isVisible = true;
                sGroup.sprites[Spr_BigHover].x = drawPos.x;
                sGroup.sprites[Spr_BigHover].y = drawPos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * ((Mathf.FloorToInt(i / 4) * 4));

                if (Input.GetMouseButtonDown(0)) {
                    itemSelected = item;
                    transSelected = trans;
                }
            }
            if (i >= 4) {
                int dividerInd = Spr_DividerLines + Mathf.FloorToInt((i-4) / 4);
                if (transUIElems[trans % transUIElems.Length] == null)
                    sGroup.sprites[dividerInd].isVisible = false;
                else {
                    sGroup.sprites[dividerInd].isVisible = true;
                    sGroup.sprites[dividerInd].x = drawPos.x;
                    sGroup.sprites[dividerInd].y = drawPos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * ((Mathf.FloorToInt(i / 4) * 4));
                }
            }
        }

        sGroup.sprites[Spr_SmallSelector].x = drawPos.x;
        sGroup.sprites[Spr_SmallSelector].y = drawPos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * (((transSelected % transUIElems.Length) *4) + itemSelected);
        sGroup.sprites[Spr_BigSelector].x = drawPos.x;
        sGroup.sprites[Spr_BigSelector].y = drawPos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * (((transSelected % transUIElems.Length) * 4));
        if (transSelected == -1) {
            sGroup.sprites[Spr_SmallSelector].isVisible = false;
            sGroup.sprites[Spr_BigSelector].isVisible = false;
        } else {
            sGroup.sprites[Spr_SmallSelector].isVisible = true;
            sGroup.sprites[Spr_BigSelector].isVisible = true;
        }

        base.DrawSprites(sGroup, frameProgress);
    }

    public override void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
    {
        foreach (FSprite fsprite in sGroup.sprites) {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
        newContainer.AddChild(pageLabel);
        newContainer.AddChild(titleLabel);
    }

    public void ResortDataList()
    {
        transList.Sort((a, b) => (a.start.CompareTo(b.start)));
    }

    public void RefreshPages()
    {
        // Remove any old TransUIElems from the previous page
        for (int i = 0; i < transUIElems.Length; i += 1) {
            if (transUIElems[i] != null) {
                transUIElems[i].Destroy();
                transUIElems[i] = null;
            }
        }

        // Update page number
        if (transSelected >= 0)
            page = Mathf.FloorToInt(transSelected / transUIElems.Length);
        pageLabel.text = "Page " + (page + 1).ToString() + "/" + TotalPages.ToString();

        // Add TransUIElems for this page
        int added = 0;
        int pageItemStart = page * transUIElems.Length;
        for (int i= pageItemStart; i<Mathf.Min(pageItemStart+transUIElems.Length, transList.Count); i+=1) {
            transUIElems[added] = new TransformationItem(this, transList[i], new Vector2(pos.x, pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * (added * 4)));
            VoezEditor.Editor.AddObject(transUIElems[added]);
            added += 1;
        }
    }

    public override void Destroy()
    {
        ResortDataList();
        base.Destroy();
        addButton.Destroy();
        deleteButton.Destroy();
        nextPageButton.Destroy();
        prevPageButton.Destroy();
        copyButton.Destroy();
        pasteButton.Destroy();
        if (mirrorButton != null)
            mirrorButton.Destroy();
        border.Destroy();
        pageLabel.RemoveFromContainer();
        titleLabel.RemoveFromContainer();
        for (int i = 0; i < transUIElems.Length; i += 1) {
            if (transUIElems[i] != null)
                transUIElems[i].Destroy();
        }
    }

    public class TransformationItem : UIElement {
        public ProjectData.TrackTransformation data;
        TransformationList parent;
        public FLabel[] labels;

        public TransformationItem(TransformationList parent, ProjectData.TrackTransformation data, Vector2 pos)
        {
            this.data = data;
            this.parent = parent;
            this.pos = pos;
            labels = new FLabel[4];
            labels[0] = new FLabel("Raleway16", "Value:");
            labels[1] = new FLabel("Raleway16", "Start:");
            labels[2] = new FLabel("Raleway16", "End:");
            labels[3] = new FLabel("Raleway16", "Easing:");
            RefreshLabelValues();
        }

        public override void Update()
        {
            base.Update();
        }

        public void RefreshLabelValues()
        {
            if (parent.type == ProjectData.TrackTransformation.TransformType.COLOR) {
                labels[0].text = "Color: " + ProjectData.colorNames[(int)data.to];
                labels[0].color = ProjectData.colors[(int)data.to];
            } else if (parent.type == ProjectData.TrackTransformation.TransformType.MOVE)
                labels[0].text = "Position: " + Mathf.FloorToInt(data.to * 100f).ToString() + "%";
            else if (parent.type == ProjectData.TrackTransformation.TransformType.SCALE)
                labels[0].text = "Scale: " + data.to.ToString("0.00") + "x";

            labels[1].text = "Start: " + data.start.ToString("0.000");
            labels[2].text = "End: " + data.end.ToString("0.000");
            labels[3].text = "Ease: " + Enum.GetName(typeof(ProjectData.Easing), data.ease);
            for (int i = 0; i < labels.Length; i += 1) {
                labels[i].x = pos.x - WIDTH * 0.5f + 10f + labels[i].textRect.width * 0.5f;
                labels[i].y = pos.y - LINE_HEIGHT * (0.5f + i);
            }
        }

        public void UpdateValue(int valueID, int delta)
        {
            if (valueID == 0) {
                if (parent.type == ProjectData.TrackTransformation.TransformType.COLOR) {
                    data.to += Mathf.Sign(delta);
                    if (data.to < 0)
                        data.to = ProjectData.colors.Length - 1;
                    if (data.to > ProjectData.colors.Length - 1)
                        data.to = 0;
                }
                else if (parent.type == ProjectData.TrackTransformation.TransformType.SCALE)
                    data.to = Mathf.Clamp(data.to + 0.1f * delta, 0f, 10f);
                else if (parent.type == ProjectData.TrackTransformation.TransformType.MOVE)
                    data.to = Mathf.Clamp(data.to + 0.01f * delta, 0f, 1f);
            }
            else if (valueID == 1)
                data.start = Mathf.Clamp(data.start + delta * VoezEditor.Editor.GetBPMTimeIncrement(), parent.StartTimeBound(data), data.end);
            else if (valueID == 2)
                data.end = Mathf.Clamp(data.end + delta * VoezEditor.Editor.GetBPMTimeIncrement(), data.start, parent.EndTimeBound(data));
            else if (valueID == 3) 
                data.ease = CycleEasing(delta > 0);
            VoezEditor.Editor.RefreshTrack(parent.parent.data.id);
            RefreshLabelValues();
        }

        public ProjectData.Easing CycleEasing(bool forward)
        {
            Array cycleValues = Enum.GetValues(typeof(ProjectData.Easing));
            for(int i=0; i<cycleValues.Length; i+=1) {
                if ((ProjectData.Easing)cycleValues.GetValue(i) == data.ease) {
                    if (forward) {
                        if (i == cycleValues.Length - 1)
                            return (ProjectData.Easing)cycleValues.GetValue(0);
                        else
                            return (ProjectData.Easing)cycleValues.GetValue(i + 1);
                    }
                    else {
                        if (i == 0)
                            return (ProjectData.Easing)cycleValues.GetValue(cycleValues.Length - 1);
                        else
                            return (ProjectData.Easing)cycleValues.GetValue(i - 1);
                    }
                }
            }
            return 0;
        }

        public override void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
        {
            for (int i = 0; i < labels.Length; i += 1)
                newContainer.AddChild(labels[i]);
        }

        public override void Destroy()
        {
            base.Destroy();
            for(int i=0; i<labels.Length; i+=1)
                labels[i].RemoveFromContainer();
        }
    }
}
