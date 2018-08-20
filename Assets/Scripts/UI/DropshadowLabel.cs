using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropshadowLabel : UIElement { 
    public FLabel normalText;
    public FLabel shadowText;
    public Vector2 offset;

	public DropshadowLabel(string fontName, string text, Vector2 pos, Vector2 dropShadowOffset) {
        normalText = new FLabel(fontName, text);
        shadowText = new FLabel(fontName, text);
        shadowText.color = Color.black;
        this.pos = pos;
        offset = dropShadowOffset;
        normalText.x = this.pos.x;
        normalText.y = this.pos.y;
        shadowText.x = this.pos.x + dropShadowOffset.x;
        shadowText.y = this.pos.y + dropShadowOffset.y;
	}

    public void SetText(string text)
    {
        normalText.text = text;
        shadowText.text = text;
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        Vector2 drawPos = new Vector2(Mathf.Lerp(this.lastPos.x, this.pos.x, frameProgress), Mathf.Lerp(this.lastPos.y, this.pos.y, frameProgress));
        normalText.x = drawPos.x;
        normalText.y = drawPos.y;
        shadowText.x = drawPos.x + offset.x;
        shadowText.y = drawPos.y + offset.y;
        base.DrawSprites(sGroup, frameProgress);
    }

    public override void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
    {
        newContainer.AddChild(shadowText);
        newContainer.AddChild(normalText);
    }
}
