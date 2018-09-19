using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectIcon : DrawableObject {

    public ProjectData data;
    public static float margin = 50f;
    public static float size = (VoezEditor.windowRes.y - margin * 2f) / 2.5f;

	public ProjectIcon(string projectPath, Vector2 pos)
    {
        this.pos = pos;
        if (projectPath != null) {
            data = new ProjectData(projectPath);
            data.LoadPreviewData();
            if (data.thumbnail != null) {
                if (Futile.atlasManager.GetAtlasWithName("thumbnail_" + data.songName) != null)
                    Futile.atlasManager.UnloadAtlas("thumbnail_" + data.songName);
                Futile.atlasManager.LoadAtlasFromTexture("thumbnail_" + data.songName, data.thumbnail);
            }
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        Futile.atlasManager.UnloadAtlas("thumbnail_" + data.songName);
        data.UnloadData();
    }

    public bool MouseOver
    {
        get {
            Vector3 mouse = Input.mousePosition;
            float dx = Mathf.Abs(mouse.x - pos.x);
            float dy = Mathf.Abs(mouse.y - pos.y);
            return (dx / size + dy / size) <= 0.5f;
        }
    }

    public override void Update()
    {
        base.Update();
        if (data != null && MouseOver && InputManager.leftMouseReleased && Vector2.Distance(Input.mousePosition, InputManager.screenPosOnLeftMousePush) < 20f) {
            VoezEditor.ProjectsPage.SetSelectedProject(this);
        }
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[1];
        float effectiveSize = Mathf.Sqrt(Mathf.Pow(size, 2f) / 2f);

        if (data == null) { 
            sGroup.sprites[0] = new FSprite("outlineBoxLargest");
            sGroup.sprites[0].color = Color.black;
            sGroup.sprites[0].alpha = 0.1f;
        }
        else {
            if (data.thumbnail != null)
                sGroup.sprites[0] = new FSprite("thumbnail_" + data.songName);
            else
                sGroup.sprites[0] = new FSprite("defaultThumbnail");
        }
        sGroup.sprites[0].rotation = 45;
        sGroup.sprites[0].scale = effectiveSize / Mathf.Max(sGroup.sprites[0].width, sGroup.sprites[0].height);
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        if (lastPos == Vector2.zero || pos == Vector2.zero) {
            for (int i = 0; i < sGroup.sprites.Length; i += 1)
                sGroup.sprites[i].isVisible = false;
        } else {
            for (int i = 0; i < sGroup.sprites.Length; i += 1)
                sGroup.sprites[i].isVisible = true;
        }

        Vector2 drawPos = new Vector2(Mathf.Lerp(lastPos.x, pos.x, frameProgress), Mathf.Lerp(lastPos.y, pos.y, frameProgress));
        sGroup.sprites[0].x = drawPos.x;
        sGroup.sprites[0].y = drawPos.y;
        base.DrawSprites(sGroup, frameProgress);
    }

}
