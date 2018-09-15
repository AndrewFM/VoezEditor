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
    public bool rightClicked;
    public bool toggled;

    public Button(string symbolName, Vector2 pos, float size, bool diamond)
    {
        this.pos = pos;
        this.size = size;
        this.diamond = diamond;
        visible = true;
        mySymbol = new FSprite(symbolName);
    }

    public Button(string fontName, string text, Vector2 pos, float size, bool diamond)
    {
        this.pos = pos;
        this.size = size;
        this.diamond = diamond;
        visible = true;
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

    public override void Update()
    {
        if (InputManager.leftMousePushed && visible && MouseOver)
            clicked = true;
        if (InputManager.rightMousePushed && visible && MouseOver)
            rightClicked = true;
        base.Update();
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        if (mySymbol != null)
            sGroup.sprites = new FSprite[3];
        else
            sGroup.sprites = new FSprite[2];

        sGroup.sprites[0] = new FSprite("Futile_White");
        sGroup.sprites[0].color = DefaultColor();
        sGroup.sprites[0].alpha = 0.75f;
        if (diamond) {
            sGroup.sprites[0].rotation = 45f;
            float newEffectiveWidth = Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[0].width, 2));
            sGroup.sprites[0].scale = size / newEffectiveWidth;
        } else
            sGroup.sprites[0].scale = size / sGroup.sprites[0].width;
        sGroup.sprites[1] = new FSprite("outlineBoxMed");
        if (diamond) {
            sGroup.sprites[1].rotation = 45f;
            float newEffectiveWidth = Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[1].width, 2));
            sGroup.sprites[1].scale = (size - 7f) / newEffectiveWidth;
        } else
            sGroup.sprites[1].scale = (size - 7f) / sGroup.sprites[1].width;
        if (mySymbol != null) { 
            sGroup.sprites[2] = mySymbol;
            sGroup.sprites[2].scale = (size * 0.6f) / Mathf.Max(sGroup.sprites[2].width, sGroup.sprites[2].height);
        }
    }

    public override void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
    {
        foreach (FSprite fsprite in sGroup.sprites) {
            fsprite.RemoveFromContainer();
        }
        newContainer.AddChild(sGroup.sprites[0]);
        if (sGroup.sprites.Length > 2)
            newContainer.AddChild(sGroup.sprites[2]);
        if (myText != null)
            newContainer.AddChild(myText);
        newContainer.AddChild(sGroup.sprites[1]);
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        for (int i = 0; i < sGroup.sprites.Length; i += 1) {
            sGroup.sprites[i].isVisible = visible;
            if (myText != null)
                myText.isVisible = visible;
        }

        if (visible) {
            for (int i = 0; i < sGroup.sprites.Length; i += 1) {
                sGroup.sprites[i].x = Mathf.Lerp(this.lastPos.x, this.pos.x, frameProgress);
                sGroup.sprites[i].y = Mathf.Lerp(this.lastPos.y, this.pos.y, frameProgress);
            }
            if (myText != null) {
                myText.x = sGroup.sprites[0].x;
                myText.y = sGroup.sprites[1].y;
            }

            if (MouseOver)
                sGroup.sprites[0].color = Color.Lerp(sGroup.sprites[0].color, Color.gray, 0.15f);
            else
                sGroup.sprites[0].color = Color.Lerp(sGroup.sprites[0].color, DefaultColor(), 0.15f);
        }
        base.DrawSprites(sGroup, frameProgress);
    }

    public override void Destroy()
    {
        base.Destroy();
        if (myText != null)
            myText.RemoveFromContainer();
    }
}