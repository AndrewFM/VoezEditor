using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteEditor : UIElement {

    public EditorProcess parent;
    public Button clickNote;
    public Button slideNote;
    public Button swipeNote;
    public Button leftDir;
    public Button rightDir;
    public FLabel typeLabel;
    public FLabel timeLabel;
    public FLabel holdLabel;
    public FLabel dirLabel;
    public ProjectData.NoteData data;
    public static float WIDTH = 350f;
    public static float HEIGHT = 250f;
    public static float LINE_HEIGHT = 64f;
    public static float LINES_START = 85f;
    public int selectedLine;
    public int numLines;
    public bool init;

    public int Spr_Back { get { return 0; } }
    public int Spr_BackBorder { get { return 1; } }
    public int Spr_Hover { get { return 2; } }
    public int Spr_Selector { get { return 3; } }
    public int Spr_SelectorBorder { get { return 4; } }

    public NoteEditor(EditorProcess parent, Vector2 pos, ProjectData.NoteData data)
    {
        this.data = data;
        this.parent = parent;
        this.pos = pos;

        typeLabel = new FLabel("Raleway24", "Type:");
        timeLabel = new FLabel("Raleway24", "Spawn Time: "+data.time.ToString());
        holdLabel = new FLabel("Raleway24", "Hold Duration: "+data.hold.ToString());
        dirLabel = new FLabel("Raleway24", "Direction:");
    }

    public void SetPage(int pageID)
    {
        selectedLine = 0;
        if (pageID == 0) {
            numLines = 2;
            clickNote.toggled = true;
            slideNote.toggled = false;
            swipeNote.toggled = false;
            holdLabel.isVisible = true;
            dirLabel.isVisible = false;
        } else if (pageID == 1) {
            numLines = 1;
            clickNote.toggled = false;
            slideNote.toggled = true;
            swipeNote.toggled = false;
            holdLabel.isVisible = false;
            dirLabel.isVisible = false;
        } else if (pageID == 2) {
            numLines = 1;
            clickNote.toggled = false;
            slideNote.toggled = false;
            swipeNote.toggled = true;
            holdLabel.isVisible = false;
            dirLabel.isVisible = true;
        }

        if (pageID == 2) {
            leftDir.visible = true;
            rightDir.visible = true;
            if (data.dir == 0) {
                leftDir.toggled = true;
                rightDir.toggled = false;
            }
            else {
                leftDir.toggled = false;
                rightDir.toggled = true;
            }
        } else {
            leftDir.visible = false;
            rightDir.visible = false;
        }
    }

    public bool MouseOver
    {
        get {
            Vector3 mouse = Input.mousePosition;
            return mouse.x >= pos.x - WIDTH * 0.5f && mouse.x <= pos.x + WIDTH * 0.5f && mouse.y >= pos.y - HEIGHT * 0.5f && mouse.y <= pos.y + HEIGHT * 0.5f;
        }
    }

    public override void InitiateSprites(SpriteLeaser sLeaser)
    {
        sLeaser.sprites = new FSprite[5];

        sLeaser.sprites[Spr_Back] = new FSprite("Futile_White");
        sLeaser.sprites[Spr_Back].color = Color.black;
        sLeaser.sprites[Spr_Back].scaleX = (WIDTH+5) / sLeaser.sprites[Spr_Back].width;
        sLeaser.sprites[Spr_Back].scaleY = (HEIGHT+5) / sLeaser.sprites[Spr_Back].height;
        sLeaser.sprites[Spr_Back].alpha = 0.8f;
        sLeaser.sprites[Spr_BackBorder] = new FSprite("outlineBoxLarge");
        sLeaser.sprites[Spr_BackBorder].scaleX = WIDTH / sLeaser.sprites[Spr_BackBorder].width;
        sLeaser.sprites[Spr_BackBorder].scaleY = HEIGHT / sLeaser.sprites[Spr_BackBorder].height;

        sLeaser.sprites[Spr_Selector] = new FSprite("Futile_White");
        sLeaser.sprites[Spr_Selector].scaleX = WIDTH / sLeaser.sprites[Spr_Selector].width;
        sLeaser.sprites[Spr_Selector].scaleY = (LINE_HEIGHT+4) / sLeaser.sprites[Spr_Selector].height;
        sLeaser.sprites[Spr_Selector].alpha = 0.5f;
        sLeaser.sprites[Spr_Selector].color = Color.red;
        sLeaser.sprites[Spr_Selector].anchorY = 1f;
        sLeaser.sprites[Spr_SelectorBorder] = new FSprite("outlineBoxLarge");
        sLeaser.sprites[Spr_SelectorBorder].scaleX = WIDTH / sLeaser.sprites[Spr_SelectorBorder].width;
        sLeaser.sprites[Spr_SelectorBorder].scaleY = LINE_HEIGHT / sLeaser.sprites[Spr_SelectorBorder].height;
        sLeaser.sprites[Spr_SelectorBorder].anchorY = 1f;

        sLeaser.sprites[Spr_Hover] = new FSprite("Futile_White");
        sLeaser.sprites[Spr_Hover].scaleX = WIDTH / sLeaser.sprites[Spr_Hover].width;
        sLeaser.sprites[Spr_Hover].scaleY = LINE_HEIGHT / sLeaser.sprites[Spr_Hover].height;
        sLeaser.sprites[Spr_Hover].alpha = 0.8f;
        sLeaser.sprites[Spr_Hover].color = Color.gray;
        sLeaser.sprites[Spr_Hover].anchorY = 1f;
    }

    public override void Update(bool eu)
    {
        if (!init) {
            init = true;
            float buttonSize = 64f;
            clickNote = new Button("click", new Vector2(pos.x - WIDTH * 0.5f + 10f + buttonSize * 0.5f + 100f, pos.y + HEIGHT * 0.5f - 10f - buttonSize * 0.5f), buttonSize, false);
            slideNote = new Button("slide", new Vector2(pos.x - WIDTH * 0.5f + 20f + buttonSize * 0.5f + buttonSize + 100f, pos.y + HEIGHT * 0.5f - 10f - buttonSize * 0.5f), buttonSize, false);
            swipeNote = new Button("swipe", new Vector2(pos.x - WIDTH * 0.5f + 30f + buttonSize * 0.5f + buttonSize * 2f + 100f, pos.y + HEIGHT * 0.5f - 10f - buttonSize * 0.5f), buttonSize, false);
            clickNote.mySymbol.rotation = 45f;
            slideNote.mySymbol.rotation = 45f;
            swipeNote.mySymbol.rotation = 45f;
            parent.AddObject(clickNote);
            parent.AddObject(slideNote);
            parent.AddObject(swipeNote);

            leftDir = new Button("swipe", new Vector2(pos.x - WIDTH * 0.5f + 10f + buttonSize * 0.5f + 150f, pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * 1.5f - 16f), buttonSize, false);
            rightDir = new Button("swipe", new Vector2(pos.x - WIDTH * 0.5f + 20f + buttonSize * 0.5f + buttonSize + 150f, pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * 1.5f - 16f), buttonSize, false);
            leftDir.mySymbol.rotation = 45f + 180f;
            rightDir.mySymbol.rotation = 45f;
            parent.AddObject(leftDir);
            parent.AddObject(rightDir);

            if (data.type == ProjectData.NoteData.NoteType.SLIDE)
                SetPage(1);
            else if (data.type == ProjectData.NoteData.NoteType.SWIPE)
                SetPage(2);
            else
                SetPage(0);
        }

        if (Input.GetMouseButtonDown(0) && !MouseOver)
            Delete();
        if (!parent.musicPlayer.paused)
            Delete();

        if (clickNote.clicked) {
            SetPage(0);
            if (data.hold > 0f)
                data.type = ProjectData.NoteData.NoteType.HOLD;
            else
                data.type = ProjectData.NoteData.NoteType.CLICK;
            parent.RefreshNote(data.id);
            clickNote.clicked = false;
        }
        if (slideNote.clicked) {
            SetPage(1);
            data.type = ProjectData.NoteData.NoteType.SLIDE;
            parent.RefreshNote(data.id);
            slideNote.clicked = false;
        }
        if (swipeNote.clicked) {
            SetPage(2);
            data.type = ProjectData.NoteData.NoteType.SWIPE;
            parent.RefreshNote(data.id);
            swipeNote.clicked = false;
        }
        if (leftDir.clicked) {
            data.dir = 0;
            parent.RefreshNote(data.id);
            leftDir.clicked = false;
            leftDir.toggled = true;
            rightDir.toggled = false;
        }
        if (rightDir.clicked) {
            data.dir = 1;
            parent.RefreshNote(data.id);
            leftDir.toggled = false;
            rightDir.toggled = true;
            rightDir.clicked = false;
        }

        float delta = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
            delta = 1f / parent.framesPerSecond;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || (!Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
            delta = -1f / parent.framesPerSecond;
        if (Input.GetKeyDown(KeyCode.UpArrow) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") > 0))
            delta = 8f / parent.framesPerSecond;
        if (Input.GetKeyDown(KeyCode.DownArrow) || (Util.ShiftDown() && Input.GetAxis("Mouse ScrollWheel") < 0))
            delta = -8f / parent.framesPerSecond;

        if (delta != 0) {
            if (selectedLine == 0) {
                data.time = Mathf.Clamp(data.time + delta, 0f, parent.musicPlayer.source.clip.length - data.hold);
                timeLabel.text = "Spawn Time: " + data.time.ToString();
            }
            if (selectedLine == 1) {
                data.hold = Mathf.Clamp(data.hold + delta, 0f, parent.musicPlayer.source.clip.length - data.time);
                if (data.hold > 0)
                    data.type = ProjectData.NoteData.NoteType.HOLD;
                else
                    data.type = ProjectData.NoteData.NoteType.CLICK;
                holdLabel.text = "Hold Duration: " + data.hold.ToString(); 
            }
            parent.RefreshNote(data.id);
        }

        base.Update(eu);
    }

    public override void AddToContainer(SpriteLeaser sLeaser, FContainer newContainer)
    {
        foreach (FSprite fsprite in sLeaser.sprites) {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
        newContainer.AddChild(holdLabel);
        newContainer.AddChild(timeLabel);
        newContainer.AddChild(typeLabel);
        newContainer.AddChild(dirLabel);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, float timeStacker)
    {
        Vector2 drawPos = new Vector2(Mathf.Lerp(this.lastPos.x, this.pos.x, timeStacker), Mathf.Lerp(this.lastPos.y, this.pos.y, timeStacker));
        sLeaser.sprites[Spr_Back].x = drawPos.x;
        sLeaser.sprites[Spr_Back].y = drawPos.y;
        sLeaser.sprites[Spr_BackBorder].x = drawPos.x;
        sLeaser.sprites[Spr_BackBorder].y = drawPos.y;
        sLeaser.sprites[Spr_BackBorder].isVisible = false;

        Vector3 mouse = Input.mousePosition;
        sLeaser.sprites[Spr_Hover].isVisible = false;
        for (int i=0; i<numLines; i+=1) {
            if (mouse.x >= pos.x - WIDTH * 0.5f && mouse.x <= pos.x + WIDTH * 0.5f && mouse.y < pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * i && mouse.y >= pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * (i+1)) {
                sLeaser.sprites[Spr_Hover].isVisible = true;
                sLeaser.sprites[Spr_Hover].x = drawPos.x;
                sLeaser.sprites[Spr_Hover].y = pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * i;

                if (Input.GetMouseButtonDown(0))
                    selectedLine = i;
            }
        }

        sLeaser.sprites[Spr_Selector].x = drawPos.x;
        sLeaser.sprites[Spr_Selector].y = pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * selectedLine;
        sLeaser.sprites[Spr_SelectorBorder].x = drawPos.x;
        sLeaser.sprites[Spr_SelectorBorder].y = pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * selectedLine + 2f;
        sLeaser.sprites[Spr_SelectorBorder].isVisible = false;

        timeLabel.x = pos.x - WIDTH * 0.5f + 10f + timeLabel.textRect.width * 0.5f;
        dirLabel.x = pos.x - WIDTH * 0.5f + 10f + dirLabel.textRect.width * 0.5f;
        typeLabel.x = pos.x - WIDTH * 0.5f + 10f + typeLabel.textRect.width * 0.5f;
        holdLabel.x = pos.x - WIDTH * 0.5f + 10f + holdLabel.textRect.width * 0.5f;

        if (clickNote != null)
            typeLabel.y = clickNote.pos.y;
        timeLabel.y = pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * 0.5f;
        holdLabel.y = pos.y + HEIGHT * 0.5f - LINES_START - LINE_HEIGHT * 1.5f;
        dirLabel.y = holdLabel.y - 16f;

        base.DrawSprites(sLeaser, timeStacker);
    }

    public void Delete()
    {
        slatedForDeletetion = true;
        clickNote.slatedForDeletetion = true;
        slideNote.slatedForDeletetion = true;
        swipeNote.slatedForDeletetion = true;
        leftDir.slatedForDeletetion = true;
        rightDir.slatedForDeletetion = true;
        holdLabel.RemoveFromContainer();
        timeLabel.RemoveFromContainer();
        dirLabel.RemoveFromContainer();
        typeLabel.RemoveFromContainer();
        parent.noteEditor = null;
    }
}
