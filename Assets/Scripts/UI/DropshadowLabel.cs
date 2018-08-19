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

    public override void DrawSprites(SpriteLeaser sLeaser, float timeStacker)
    {
        Vector2 drawPos = new Vector2(Mathf.Lerp(this.lastPos.x, this.pos.x, timeStacker), Mathf.Lerp(this.lastPos.y, this.pos.y, timeStacker));
        normalText.x = drawPos.x;
        normalText.y = drawPos.y;
        shadowText.x = drawPos.x + offset.x;
        shadowText.y = drawPos.y + offset.y;
        base.DrawSprites(sLeaser, timeStacker);
    }

    public override void AddToContainer(SpriteLeaser sLeaser, FContainer newContainer)
    {
        newContainer.AddChild(shadowText);
        newContainer.AddChild(normalText);
    }
}
