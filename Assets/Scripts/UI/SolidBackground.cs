using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBackground : DrawableObject {
    public Color color;

    public SolidBackground(Color color)
    {
        this.color = color;
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[1];
        sGroup.sprites[0] = new FSprite("Futile_White");
        sGroup.sprites[0].anchorX = 0f;
        sGroup.sprites[0].anchorY = 0f;
        sGroup.sprites[0].scaleX = VoezEditor.windowRes.x / sGroup.sprites[0].width;
        sGroup.sprites[0].scaleY = VoezEditor.windowRes.y / sGroup.sprites[0].height;
        sGroup.sprites[0].color = color;
    }
}
