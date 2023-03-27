using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TransformationList : UIElement {
    public TrackEditor parent;
    public List<ProjectData.TrackTransformation> transList;
    public List<int> itemToTransListInds;
    public List<TransformationItem.TransformationItemType> itemTypes;
    public TransformationItem[] transUIElems;
    public Button addButton;
    public Button deleteButton;
    public Button nextPageButton;
    public Button prevPageButton;
    public Button copyButton;
    public Button pasteButton;
    public Button mirrorButton;
    public Button repeatButton;
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
        itemToTransListInds = new List<int>();
        itemTypes = new List<TransformationItem.TransformationItemType>();
        if (type == ProjectData.TrackTransformation.TransformType.MOVE)
            transList = parent.data.move;
        else if (type == ProjectData.TrackTransformation.TransformType.SCALE)
            transList = parent.data.scale;
        else
            transList = parent.data.colorChange;
        ResortDataList();
        transUIElems = new TransformationItem[TRANS_PER_PAGE];
        transSelected = -1;
        pageLabel = new FLabel("Raleway16", "Page");
        titleLabel = new FLabel("Raleway24", "Keyframes:");
    }

    public int TotalPages
    {
        get { return Mathf.FloorToInt((itemTypes.Count - 1) / transUIElems.Length) + 1;  }
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
            repeatButton = new Button("repeat", new Vector2(baseX + (buttonSize + spacing) * 4f, baseY), buttonSize, false);
            copyButton = new Button("copy", new Vector2(baseX + (buttonSize + spacing) * 5f, baseY), buttonSize, false);
            pasteButton = new Button("paste", new Vector2(baseX + (buttonSize + spacing) * 6f, baseY), buttonSize, false);
            pageLabel.x = pos.x;
            pageLabel.y = baseY - buttonSize * 0.5f - spacing - pageLabel.textRect.height * 0.5f;
            titleLabel.x = pos.x;
            titleLabel.y = pos.y + HEIGHT * 0.5f + LINE_HEIGHT;
            VoezEditor.Editor.AddObject(prevPageButton);
            VoezEditor.Editor.AddObject(nextPageButton);
            VoezEditor.Editor.AddObject(addButton);
            VoezEditor.Editor.AddObject(deleteButton);
            VoezEditor.Editor.AddObject(repeatButton);
            VoezEditor.Editor.AddObject(copyButton);
            VoezEditor.Editor.AddObject(pasteButton);
            if (type == ProjectData.TrackTransformation.TransformType.MOVE) {
                mirrorButton = new Button("mirror", new Vector2(baseX + (buttonSize + spacing) * 7f, baseY), buttonSize, false);
                VoezEditor.Editor.AddObject(mirrorButton);
            }

            border = new RectangleBorder(new Rect(new Vector2(pos.x - WIDTH * 0.5f, pos.y - HEIGHT * 0.5f), new Vector2(WIDTH, HEIGHT)), 3f);
            VoezEditor.Editor.AddObject(border);
            page = TotalPages - 1;
            RefreshPages();
            init = true;
        }

        // Edit Values
        int mappedTransSelected = -1;
        if (transSelected >= 0) {
            mappedTransSelected = itemToTransListInds[transSelected];
        }
        if (transSelected >= 0 && transUIElems[transSelected % transUIElems.Length] != null) {
            int delta = 0;
            if (InputManager.UpTick())
                delta = 1;
            if (InputManager.DownTick())
                delta = -1;
            if (InputManager.RightTick())
                delta = 5;
            if (InputManager.LeftTick())
                delta = -5;
            if (delta != 0) {
                if (Util.CtrlDown() && (itemSelected == 1 || itemSelected == 2)) {
                    // Shift all keyframes after this one (TODO FOR REPEATS)
                    float shiftAmount = delta * VoezEditor.Editor.GetBPMTimeIncrement();
                    float endShiftLimit = parent.data.end - transList[transList.Count - 1].end;
                    float startShiftLimit = 0;
                    if (itemSelected == 2)
                        startShiftLimit = transList[mappedTransSelected].start - transList[mappedTransSelected].end;
                    else {
                        if (mappedTransSelected == 0)
                            startShiftLimit = parent.data.start - transList[mappedTransSelected].start;
                        else
                            startShiftLimit = transList[mappedTransSelected - 1].end - transList[mappedTransSelected].start;
                    }
                    if (shiftAmount > endShiftLimit)
                        shiftAmount = endShiftLimit;
                    if (shiftAmount < startShiftLimit)
                        shiftAmount = startShiftLimit;

                    if (itemSelected == 1)
                        transList[mappedTransSelected].start += shiftAmount;
                    transList[mappedTransSelected].end += shiftAmount;
                    for (int i = mappedTransSelected + 1; i < transList.Count; i += 1) {
                        transList[i].start += shiftAmount;
                        transList[i].end += shiftAmount;
                    }
                    if (itemSelected == 1)
                        VoezEditor.Editor.JumpToTime(transList[mappedTransSelected].start);
                    else
                        VoezEditor.Editor.JumpToTime(transList[mappedTransSelected].end);
                    for (int i = 0; i < transUIElems.Length; i += 1) {
                        if (transUIElems[i] != null)
                            transUIElems[i].RefreshLabelValues();
                    }
                }
                else {
                    transUIElems[itemSelected % transUIElems.Length].UpdateValue(itemSelected, delta);
                    if (itemTypes[itemSelected] != TransformationItem.TransformationItemType.NONE) {
                        UpdateTimesAfterRepeat(itemSelected);
                    }
                }
            }
        }

        // Mouse Slide Value Editing
        if (Input.GetMouseButton(1) && !VoezEditor.Editor.ui.HoveringOverSubmenuItem()) {
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
            VoezEditor.Editor.ui.trackAdder.previewScale = -1f;
            VoezEditor.Editor.ui.trackAdder.previewX = -1f;
        }

        // Page Navigation
        if (prevPageButton.clicked) {
            if (page > 0) {
                if (transSelected >= 0)
                    transSelected = Mathf.Clamp(transSelected - transUIElems.Length, 0, itemTypes.Count-1);
                else
                    page -= 1;
                RefreshPages();
            } else if (itemTypes.Count > 0) {
                transSelected = itemTypes.Count-1;
                RefreshPages();
            }
            prevPageButton.clicked = false;
        }
        if (nextPageButton.clicked) {
            if (page < TotalPages - 1) {
                if (transSelected >= 0)
                    transSelected = Mathf.Clamp(transSelected + transUIElems.Length, 0, itemTypes.Count - 1);
                else
                    page += 1;
                RefreshPages();
            } else if (itemTypes.Count > 0) {
                transSelected = 0;
                RefreshPages();
            }
            nextPageButton.clicked = false;
        }

        // Add and Delete Transformations
        mappedTransSelected = -1;
        if (transSelected >= 0) {
            mappedTransSelected = itemToTransListInds[transSelected];
        }
        if (addButton.clicked || repeatButton.clicked) {
            ProjectData.TrackTransformation transformation = new ProjectData.TrackTransformation();
            // Default Values
            if (type == ProjectData.TrackTransformation.TransformType.SCALE)
                transformation.to = 1.0f;
            if (type == ProjectData.TrackTransformation.TransformType.MOVE)
                transformation.to = 0.5f;
            if (mappedTransSelected < 0)
                mappedTransSelected = transList.Count - 1; // If no item selected, default to adding at end of trans list.
            if (repeatButton.clicked) {
                transformation.repeatCount = 1;
            }
            if (itemTypes.Count != 0 && transList[mappedTransSelected].repeatCount != 0) {
                transformation.repeatCount = transList[mappedTransSelected].repeatCount;
            }
            if (itemTypes.Count == 0) {
                transformation.start = parent.data.start;
                transformation.end = parent.data.end;
            }
            else if (Util.ShiftDown()) {
                // Add Before selection
                transformation.end = transList[mappedTransSelected].start;
                if (mappedTransSelected == 0)
                    transformation.start = Mathf.Max(transformation.end - VoezEditor.Editor.GetBPMTimeIncrement() * 5, parent.data.start);
                else {
                    transformation.start = Mathf.Max(transformation.end - VoezEditor.Editor.GetBPMTimeIncrement() * 5, transList[mappedTransSelected - 1].end);
                }
            }
            else {
                // Add After Selection
                transformation.start = transList[mappedTransSelected].end;
                if (mappedTransSelected == transList.Count - 1)
                    transformation.end = Mathf.Min(transformation.start + VoezEditor.Editor.GetBPMTimeIncrement() * 5, parent.data.end);
                else
                    transformation.end = Mathf.Min(transformation.start + VoezEditor.Editor.GetBPMTimeIncrement() * 5, transList[mappedTransSelected + 1].start);
            }
            transformation.duration = transformation.end - transformation.start;
            if (transformation.end >= transformation.start) {
                transList.Add(transformation);
                mappedTransSelected = Mathf.Clamp(mappedTransSelected + 1, 0, transList.Count - 1);
                ResortDataList();
                RefreshPages();
            }
            addButton.clicked = false;
            repeatButton.clicked = false;
        }
        if (deleteButton.clicked) {
            if (transList.Count > 0 && mappedTransSelected >= 0) {
                transList.RemoveAt(mappedTransSelected);
                transSelected -= 1;
                RefreshPages();
            }
            deleteButton.clicked = false;
        }

        // Copy and Paste Transformations
        if (copyButton.clicked) {
            VoezEditor.Editor.project.transformClipboard = ProjectData.DeepCopyTransformationList(transList);
            VoezEditor.Editor.project.clipboardContentsType = type;
            VoezEditor.Editor.project.transformClipboardStartTime = parent.data.start;
            copyButton.clicked = false;
        }
        if (pasteButton.clicked) {
            if (VoezEditor.Editor.project.transformClipboard != null && VoezEditor.Editor.project.clipboardContentsType == type) {
                ProjectData.ReplaceTransformationList(VoezEditor.Editor.project.transformClipboard, transList, VoezEditor.Editor.project.transformClipboardStartTime, parent.data.start);
                for(int i=0; i<transList.Count; i+=1) {
                    if (transList[i].end > parent.data.end) {
                        // Pasting can change the total duration of the track if the new keyframes span beyond that duration.
                        parent.data.end = transList[i].end;
                        parent.endLabel.text = "Despawn Time: " + VoezEditor.Editor.BeatTimeStamp(parent.data.end);
                    }
                }
                page = 0;
                transSelected = -1;
                mappedTransSelected = -1;
                RefreshPages();
            }
            pasteButton.clicked = false;
        }

        // Mirror X
        if (mirrorButton != null && mirrorButton.clicked) {
            if (Util.ShiftDown()) {
                if (mappedTransSelected != -1)
                    transList[mappedTransSelected].to = 1f - transList[mappedTransSelected].to;
                else
                    parent.data.x = 1f - parent.data.x;
            } else {
                parent.data.x = 1f - parent.data.x;
                for (int i = 0; i < transList.Count; i += 1)
                    transList[i].to = 1f - transList[i].to;
            }
            RefreshPages();
            parent.RefreshValueLabel();
            mirrorButton.clicked = false;
        }

        if (mappedTransSelected != -1)
            parent.selectedLine = -1;

        base.Update();
    }

    public int Spr_BigSelector { get { return 2; } }
    public int Spr_SmallSelector { get { return 3; } }
    public int Spr_BigHover { get { return 0; } }
    public int Spr_SmallHover { get { return 1; } }
    public int Spr_DividerLines { get { return 4;  } }
    public int TotalSprites { get { return Spr_DividerLines + (TRANS_PER_PAGE - 1); } }

    public void UpdateTimesAfterRepeat(int startItemIndex) {
        // Find first item in this repeat group.
        float repeatStartTime = 0;
        int repeatStartIndex = 0;
        for(int i=startItemIndex; i>=0; i-=1) {
            if (itemTypes[i] == TransformationItem.TransformationItemType.REPEAT_START) {
                repeatStartTime = transList[itemToTransListInds[i]].start;
                repeatStartIndex = i;
                break;
            }
        }
        // Count up time occupied by full execution of everything in this repeat group.
        float repeatTotalTime = 0;
        float repeatEndTime = 0;
        int repeatEndIndex = 0;
        for(int i=repeatStartIndex; i<itemTypes.Count; i+=1) {
            if (itemTypes[i] == TransformationItem.TransformationItemType.REPEAT_ITEM) {
                repeatTotalTime += transList[itemToTransListInds[i]].offset;
                repeatTotalTime += transList[itemToTransListInds[i]].duration;
            }
            if (itemTypes[i] == TransformationItem.TransformationItemType.REPEAT_END) {
                repeatTotalTime *= transList[itemToTransListInds[repeatStartIndex]].repeatCount;
                transList[itemToTransListInds[i]].end = repeatStartTime + repeatTotalTime;
                repeatEndTime = transList[itemToTransListInds[i]].end;
                repeatEndIndex = i;
                break;
            }
        }
        // TODO: How to handle the new end time exceeding the start time of the element(s) after the repeat?
    }

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

                if (InputManager.leftMousePushed) {
                    itemSelected = item;
                    transSelected = trans;

                    if (itemSelected == 1)
                        VoezEditor.Editor.JumpToTime(transUIElems[transSelected % transUIElems.Length].data.start);
                    if (itemSelected == 0 || itemSelected == 2)
                        VoezEditor.Editor.JumpToTime(transUIElems[transSelected % transUIElems.Length].data.end);
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
        transList.Sort((a, b) => (TransListSortMethod(a,b)));
        itemToTransListInds = new List<int>();
        itemTypes = new List<TransformationItem.TransformationItemType>();
        bool repeatStarted = false;
        for(int i=0; i<transList.Count; i+=1) {
            if (transList[i].repeatCount == 0) {
                itemToTransListInds.Add(i);
                itemTypes.Add(TransformationItem.TransformationItemType.NONE);
            } else {
                if (!repeatStarted) {
                    itemToTransListInds.Add(i);
                    itemTypes.Add(TransformationItem.TransformationItemType.REPEAT_START);
                    repeatStarted = true;
                }
                itemToTransListInds.Add(i);
                itemTypes.Add(TransformationItem.TransformationItemType.REPEAT_ITEM);
                if (i == transList.Count - 1 || transList[i+1].repeatCount == 0) {
                    itemToTransListInds.Add(i);
                    itemTypes.Add(TransformationItem.TransformationItemType.REPEAT_END);
                    repeatStarted = false;
                }
            }
        }
    }

    private int TransListSortMethod(ProjectData.TrackTransformation a, ProjectData.TrackTransformation b)
    {
        int firstComp = a.end.CompareTo(b.end);
        if (firstComp == 0)
            return a.start.CompareTo(b.start);
        else
            return firstComp;
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
        for (int i= pageItemStart; i<Mathf.Min(pageItemStart+transUIElems.Length, itemTypes.Count); i+=1) {
            transUIElems[added] = new TransformationItem(this, transList[itemToTransListInds[i]], new Vector2(pos.x, pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * (added * 4)), itemTypes[i]);
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
        repeatButton.Destroy();
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
        public TransformationItemType type;
        public enum TransformationItemType
        {
            NONE,
            REPEAT_START,
            REPEAT_END,
            REPEAT_ITEM
        }

        public TransformationItem(TransformationList parent, ProjectData.TrackTransformation data, Vector2 pos, TransformationItemType itemType)
        {
            this.data = data;
            this.parent = parent;
            this.pos = pos;
            type = itemType;
            if (itemType == TransformationItemType.NONE) {
                labels = new FLabel[4];
                labels[0] = new FLabel("Raleway16", "Value:");
                labels[1] = new FLabel("Raleway16", "Start:");
                labels[2] = new FLabel("Raleway16", "End:");
                labels[3] = new FLabel("Raleway16", "Easing:");
            }
            else if (itemType == TransformationItemType.REPEAT_ITEM) {
                labels = new FLabel[4];
                labels[0] = new FLabel("Raleway16", "Value:");
                labels[1] = new FLabel("Raleway16", "Offset:");
                labels[2] = new FLabel("Raleway16", "Duration:");
                labels[3] = new FLabel("Raleway16", "Easing:");
            }
            else if (itemType == TransformationItemType.REPEAT_START) {
                labels = new FLabel[3];
                labels[0] = new FLabel("Raleway16", "[REPEAT BLOCK START]");
                labels[1] = new FLabel("Raleway16", "Start:");
                labels[2] = new FLabel("Raleway16", "Repeat Count:");
            }
            else if (itemType == TransformationItemType.REPEAT_END) {
                labels = new FLabel[1];
                labels[0] = new FLabel("Raleway16", "[REPEAT BLOCK END]");
            }
            RefreshLabelValues();
        }

        public override void Update()
        {
            base.Update();
        }

        public void RefreshLabelValues() {
            if (type == TransformationItemType.NONE || type == TransformationItemType.REPEAT_ITEM) {
                if (parent.type == ProjectData.TrackTransformation.TransformType.COLOR) {
                    labels[0].text = "Color: " + ProjectData.colorNames[(int)data.to];
                    labels[0].color = ProjectData.colors[(int)data.to];
                }
                else if (parent.type == ProjectData.TrackTransformation.TransformType.MOVE)
                    labels[0].text = "Position: " + (data.to * 100f).ToString("0.###") + "%";
                else if (parent.type == ProjectData.TrackTransformation.TransformType.SCALE)
                    labels[0].text = "Scale: " + data.to.ToString("0.00") + "x";
            }
            if (type == TransformationItemType.NONE) { 
                labels[1].text = "Offset: " + VoezEditor.Editor.BeatTimeStamp(data.offset);
                labels[2].text = "Duration: " + VoezEditor.Editor.BeatTimeStamp(data.duration);
                labels[3].text = "Ease: " + Enum.GetName(typeof(ProjectData.Easing), data.ease);
            }
            if (type == TransformationItemType.REPEAT_ITEM) {
                labels[1].text = "Start: " + VoezEditor.Editor.BeatTimeStamp(data.start);
                labels[2].text = "End: " + VoezEditor.Editor.BeatTimeStamp(data.end);
                labels[3].text = "Ease: " + Enum.GetName(typeof(ProjectData.Easing), data.ease);
            }
            if (type == TransformationItemType.REPEAT_START) {
                labels[1].text = "Start: " + VoezEditor.Editor.BeatTimeStamp(data.start);
                labels[2].text = "Repeat Count: " + data.repeatCount.ToString();
            }

            for (int i = 0; i < labels.Length; i += 1) {
                labels[i].x = pos.x - WIDTH * 0.5f + 10f + labels[i].textRect.width * 0.5f;
                labels[i].y = pos.y - LINE_HEIGHT * (0.5f + i);
            }
        }

        public void UpdateValue(int valueID, int delta)
        {
            if (valueID == 0 && (type == TransformationItemType.NONE || type == TransformationItemType.REPEAT_ITEM)) {
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
                    data.to = Mathf.RoundToInt(Mathf.Clamp(data.to + 0.01f * delta, -0.5f, 1.5f) * 100f) / 100f;
            }
            else if (valueID == 1 && (type == TransformationItemType.NONE || type == TransformationItemType.REPEAT_START)) {
                data.start = Mathf.Clamp(data.start + delta * VoezEditor.Editor.GetBPMTimeIncrement(), parent.StartTimeBound(data), data.end);
                VoezEditor.Editor.JumpToTime(data.start);
            }
            else if (valueID == 1 && type == TransformationItemType.REPEAT_ITEM) {
                data.offset = Mathf.Max(data.offset + delta * VoezEditor.Editor.GetBPMTimeIncrement(), 0);
            }
            else if (valueID == 2 && type == TransformationItemType.NONE) {
                data.end = Mathf.Clamp(data.end + delta * VoezEditor.Editor.GetBPMTimeIncrement(), data.start, parent.EndTimeBound(data));
                VoezEditor.Editor.JumpToTime(data.end);
            }
            else if (valueID == 2 && type == TransformationItemType.REPEAT_ITEM) {
                data.duration = Mathf.Max(data.duration + delta * VoezEditor.Editor.GetBPMTimeIncrement(), 0);
            }
            else if (valueID == 2 && type == TransformationItemType.REPEAT_START) {
                data.repeatCount = Mathf.Max(data.repeatCount + delta, 1);
            }
            else if (valueID == 3 && type == TransformationItemType.NONE) {
                data.ease = CycleEasing(delta > 0);
            }
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
