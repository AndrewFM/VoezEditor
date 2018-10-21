using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectsUI {

    public ProjectSelector selector;
    public MidgroundInfo infoView;
    private Text songNameLabel;
    private Text authorNameLabel;
    public Button editButton;
    public Button easyButton;
    public Button hardButton;
    public Button specialButton;
    public GameObject songInfo;
    public static Color EASY_COLOR = Util.Color255(25, 170, 200);
    public static Color HARD_COLOR = Util.Color255(230, 65, 75);
    public static Color SPECIAL_COLOR = Util.Color255(245, 160, 15);

    public ProjectsUI()
    {
        selector = new ProjectSelector();
        VoezEditor.ProjectsPage.AddObject(selector);
        infoView = new MidgroundInfo();
        VoezEditor.ProjectsPage.AddObject(infoView);

        // Song Name and Author Text
        GameObject canvas = GameObject.Find("Canvas");
        GameObject songInfoPf = Resources.Load<GameObject>("Prefabs/SongInfoPrefab");
        songInfo = UnityEngine.Object.Instantiate(songInfoPf);
        songInfo.transform.SetParent(canvas.transform);
        songInfo.transform.position = new Vector2(VoezEditor.windowRes.x - 32f, VoezEditor.windowRes.y - 32f);
        GameObject songName = songInfo.transform.Find("SongName").gameObject;
        GameObject authorName = songInfo.transform.Find("AuthorName").gameObject;
        songNameLabel = songName.GetComponent<Text>();
        authorNameLabel = authorName.gameObject.GetComponent<Text>();

        // Edit and Difficulty Buttons
        float editButtonSize = VoezEditor.windowRes.y / 2.4f;
        Vector2 editButtonPos = new Vector2(VoezEditor.windowRes.x - 16f - editButtonSize / 2f, 16f + editButtonSize / 2f);
        editButton = new Button("Raleway32", "Edit", editButtonPos, editButtonSize, true);
        editButton.myColor = EASY_COLOR;
        editButton.myAlpha = 0.95f;
        editButton.visible = false;
        VoezEditor.ProjectsPage.AddObject(editButton);
        easyButton = new Button("Raleway24", "?"+Environment.NewLine+"Easy", new Vector2(editButtonPos.x - editButtonSize/2f, editButtonPos.y + editButtonSize/4f), editButtonSize/2f, true);
        easyButton.myColor = EASY_COLOR;
        easyButton.myAlpha = 0.95f;
        easyButton.visible = false;
        hardButton = new Button("Raleway24", "?" + Environment.NewLine + "Hard", new Vector2(editButtonPos.x - editButtonSize / 2f, editButtonPos.y - editButtonSize / 4f), editButtonSize / 2f, true);
        hardButton.myColor = HARD_COLOR;
        hardButton.myAlpha = 0.95f;
        hardButton.visible = false;
        specialButton = new Button("Raleway24", "?" + Environment.NewLine + "Special", new Vector2(editButtonPos.x - editButtonSize * 0.75f, editButtonPos.y), editButtonSize / 2f, true);
        specialButton.myColor = SPECIAL_COLOR;
        specialButton.myAlpha = 0.95f;
        specialButton.visible = false;
        VoezEditor.ProjectsPage.AddObject(easyButton);
        VoezEditor.ProjectsPage.AddObject(hardButton);
        VoezEditor.ProjectsPage.AddObject(specialButton);

        if (VoezEditor.editType == "easy")
            easyButton.clicked = true;
        else if (VoezEditor.editType == "hard")
            hardButton.clicked = true;
        else
            specialButton.clicked = true;
    }

    public bool HoveringOverSubmenuItem()
    {
        if (editButton.MouseOver || easyButton.MouseOver || hardButton.MouseOver || specialButton.MouseOver)
            return true;
        return false;
    }

    public void Update()
    {
        if (easyButton.clicked) {
            editButton.myColor = EASY_COLOR;
            selector.myColor = EASY_COLOR;
            VoezEditor.editType = "easy";
            easyButton.clicked = false;
        }
        if (hardButton.clicked) {
            editButton.myColor = HARD_COLOR;
            selector.myColor = HARD_COLOR;
            VoezEditor.editType = "hard";
            hardButton.clicked = false;
        }
        if (specialButton.clicked) {
            editButton.myColor = SPECIAL_COLOR;
            selector.myColor = SPECIAL_COLOR;
            VoezEditor.editType = "extra";
            specialButton.clicked = false;
        }
        if (editButton.clicked) {
            VoezEditor.ProjectsPage.readyToShutDown = true;
            editButton.clicked = false;
        }
    }

    public void Unload()
    {
        UnityEngine.Object.Destroy(songInfo);
    }

    public void SetSongName(string name)
    {
        songNameLabel.text = name;
    }

    public void SetAuthorName(string name)
    {
        authorNameLabel.text = name;
    }

    public void UpdateEditInterface()
    {
        editButton.visible = true;
        specialButton.visible = true;
        hardButton.visible = true;
        easyButton.visible = true;

        ProjectData data = selector.linkedIcon.data;
        if (data.easyLevel <= 0)
            easyButton.myText.text = "?";
        else
            easyButton.myText.text = Mathf.FloorToInt(data.easyLevel).ToString();
        if (data.hardLevel <= 0)
            hardButton.myText.text = "?";
        else
            hardButton.myText.text = Mathf.FloorToInt(data.hardLevel).ToString();
        if (data.extraLevel <= 0)
            specialButton.myText.text = "?";
        else
            specialButton.myText.text = Mathf.FloorToInt(data.extraLevel).ToString();
        easyButton.myText.text += Environment.NewLine + "Easy";
        hardButton.myText.text += Environment.NewLine + "Hard";
        specialButton.myText.text += Environment.NewLine + "Special";
    }

    public class MidgroundInfo : UIElement {
        public override void InitiateSprites(SpriteGroup sGroup)
        {
            sGroup.sprites = new FSprite[2];
            for (int i = 0; i < sGroup.sprites.Length; i += 1) {
                sGroup.sprites[i] = new FSprite("bigGrad");
                sGroup.sprites[i].scaleX = VoezEditor.windowRes.x / sGroup.sprites[i].width;
                sGroup.sprites[i].scaleY = (VoezEditor.windowRes.y * 0.2f) / sGroup.sprites[i].height;
                if (i == 1)
                    sGroup.sprites[i].scaleY *= -1f;
                sGroup.sprites[i].alpha = 0.75f;
                sGroup.sprites[i].color = Util.Color255(80, 80, 120);
                sGroup.sprites[i].anchorY = 0f;
                sGroup.sprites[i].anchorX = 0f;
            }
        }

        public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
        {
            sGroup.sprites[0].x = 0;
            sGroup.sprites[0].x = 0;
            sGroup.sprites[1].y = 0;
            sGroup.sprites[1].y = VoezEditor.windowRes.y;
            base.DrawSprites(sGroup, frameProgress);
        }
    }

    public class ProjectSelector : UIElement {
        public float sinTimer;
        public ProjectIcon linkedIcon;
        public int linkedIndex = -1;
        public Color myColor;

        public ProjectSelector()
        {
            pos = new Vector2(-1000, -1000);
            myColor = EASY_COLOR;
        }

        public override void Update()
        {
            base.Update();
            sinTimer += 1f;

            if (linkedIcon != null) {
                if (linkedIcon.readyForDeletion) {
                    linkedIcon = null;
                    pos = new Vector2(-1000, -1000);
                }
                else
                    pos = linkedIcon.pos;
            }
        }

        public override void InitiateSprites(SpriteGroup sGroup)
        {
            sGroup.sprites = new FSprite[1];
            float effectiveSize = Mathf.Sqrt(Mathf.Pow(ProjectIcon.size, 2f) / 2f);
            sGroup.sprites[0] = new FSprite("outlineBoxLargestBlur");
            sGroup.sprites[0].scale = (effectiveSize+12f) / sGroup.sprites[0].width;
            sGroup.sprites[0].color = myColor;
            sGroup.sprites[0].rotation = 45f;
        }

        public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
        {
            sGroup.sprites[0].x = pos.x;
            sGroup.sprites[0].y = pos.y;
            sGroup.sprites[0].color = myColor;
            sGroup.sprites[0].alpha = Mathf.Abs(Mathf.Sin(sinTimer * ((2f * Mathf.PI) / 240f))) * 0.6f + 0.4f;
            base.DrawSprites(sGroup, frameProgress);
        }
    }
}