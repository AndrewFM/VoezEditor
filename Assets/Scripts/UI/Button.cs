using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : UIElement {
    public FLabel myText;
    public FSprite mySymbol;
    public float size;
    public bool diamond;
    public bool visible;
    public bool clicked;
    public bool toggled;

    public Button(string symbolName, Vector2 pos, float size, bool diamond)
    {
        this.pos = pos;
        this.size = size;
        this.diamond = diamond;
        this.visible = true;
        mySymbol = new FSprite(symbolName);
    }

    public Button(string fontName, string text, Vector2 pos, float size, bool diamond)
    {
        this.pos = pos;
        this.size = size;
        this.diamond = diamond;
        myText = new FLabel(fontName, text);
    }

    public bool MouseOver
    {
        get {
            Vector3 mouse = Input.mousePosition;
            if (diamond) {
                float dx = Mathf.Abs(mouse.x - pos.x);
                float dy = Mathf.Abs(mouse.y - pos.y);
                return (dx / size + dy / size) <= 0.5f;
            }
            return mouse.x > pos.x - size*0.5f && mouse.x < pos.x + size*0.5f && mouse.y > pos.y - size*0.5f && mouse.y < pos.y + size*0.5f;
        }
    }

    public Color DefaultColor()
    {
        if (toggled)
            return Color.red;
        else
            return Color.black;
    }

    public override void Update(bool eu)
    {
        if (Input.GetMouseButtonDown(0) && visible && MouseOver)
            this.clicked = true;
        base.Update(eu);
    }

    public override void InitiateSprites(SpriteLeaser sLeaser)
    {
        if (mySymbol != null)
            sLeaser.sprites = new FSprite[3];
        else
            sLeaser.sprites = new FSprite[2];

        sLeaser.sprites[0] = new FSprite("Futile_White");
        sLeaser.sprites[0].color = DefaultColor();
        sLeaser.sprites[0].alpha = 0.8f;
        if (diamond) {
            sLeaser.sprites[0].rotation = 45f;
            float newEffectiveWidth = Mathf.Sqrt(2f * Mathf.Pow(sLeaser.sprites[0].width, 2));
            sLeaser.sprites[0].scale = size / newEffectiveWidth;
        } else
            sLeaser.sprites[0].scale = size / sLeaser.sprites[0].width;
        if (size > 350f)
            sLeaser.sprites[1] = new FSprite("outlineBoxLarge");
        else
            sLeaser.sprites[1] = new FSprite("outlineBoxMed");
        if (diamond) {
            sLeaser.sprites[1].rotation = 45f;
            float newEffectiveWidth = Mathf.Sqrt(2f * Mathf.Pow(sLeaser.sprites[1].width, 2));
            sLeaser.sprites[1].scale = (size - 7f) / newEffectiveWidth;
        } else
            sLeaser.sprites[1].scale = (size - 7f) / sLeaser.sprites[1].width;
        if (mySymbol != null) { 
            sLeaser.sprites[2] = mySymbol;
            sLeaser.sprites[2].scale = (size * 0.5f) / sLeaser.sprites[2].width;
        }
    }

    public override void AddToContainer(SpriteLeaser sLeaser, FContainer newContainer)
    {
        foreach (FSprite fsprite in sLeaser.sprites) {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
        if (myText != null)
            newContainer.AddChild(myText);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, float timeStacker)
    {
        for (int i = 0; i < sLeaser.sprites.Length; i += 1) {
            sLeaser.sprites[i].isVisible = visible;
            if (myText != null)
                myText.isVisible = visible;
        }

        if (visible) {
            for (int i = 0; i < sLeaser.sprites.Length; i += 1) {
                sLeaser.sprites[i].x = Mathf.Lerp(this.lastPos.x, this.pos.x, timeStacker);
                sLeaser.sprites[i].y = Mathf.Lerp(this.lastPos.y, this.pos.y, timeStacker);
            }
            if (myText != null) {
                myText.x = sLeaser.sprites[0].x;
                myText.y = sLeaser.sprites[1].y;
            }

            if (MouseOver)
                sLeaser.sprites[0].color = Color.Lerp(sLeaser.sprites[0].color, Color.gray, 0.15f);
            else
                sLeaser.sprites[0].color = Color.Lerp(sLeaser.sprites[0].color, DefaultColor(), 0.15f);
        }
        base.DrawSprites(sLeaser, timeStacker);
    }
}