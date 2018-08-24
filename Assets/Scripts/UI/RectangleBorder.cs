using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleBorder : UIElement {
    public Rect bounds;
    public float thickness;

    public RectangleBorder(Rect bounds, float thickness)
    {
        this.bounds = bounds;
        this.thickness = thickness;
        pos = bounds.center;
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[4];

        for (int i = 0; i <= 1; i += 1) {
            sGroup.sprites[i] = new FSprite("Futile_White");
            sGroup.sprites[i].scaleX = thickness / sGroup.sprites[i].width;
            sGroup.sprites[i].scaleY = bounds.height / sGroup.sprites[i].height;
        }
        for (int i = 2; i <= 3; i += 1) {
            sGroup.sprites[i] = new FSprite("Futile_White");
            sGroup.sprites[i].scaleX = bounds.width / sGroup.sprites[i].width;
            sGroup.sprites[i].scaleY = thickness / sGroup.sprites[i].height;
        }
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
        sGroup.sprites[0].x = drawPos.x - bounds.width * 0.5f;
        sGroup.sprites[1].x = drawPos.x + bounds.width * 0.5f;
        sGroup.sprites[0].y = drawPos.y;
        sGroup.sprites[1].y = drawPos.y;
        sGroup.sprites[2].y = drawPos.y - bounds.height * 0.5f;
        sGroup.sprites[3].y = drawPos.y + bounds.height * 0.5f;
        sGroup.sprites[2].x = drawPos.x;
        sGroup.sprites[3].x = drawPos.x;
        base.DrawSprites(sGroup, frameProgress);
    }
}
