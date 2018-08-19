using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : UIElement {
    public float width;
    public float progress;
    public bool allowScrubbing;

    public Slider(Vector2 pos, float width)
    {
        this.pos = pos;
        this.width = width;
    }

    public int Spr_SliderBack { get { return 0; } }
    public int Spr_SliderFront { get { return 1; } }
    public int Spr_SliderScrubberBack { get { return 2; } }
    public int Spr_SliderScrubberFront { get { return 3; } }

    public override void Update(bool eu)
    {
        base.Update(eu);
    }

    public override void InitiateSprites(SpriteLeaser sLeaser)
    {
        sLeaser.sprites = new FSprite[4];

        sLeaser.sprites[Spr_SliderBack] = new FSprite("Futile_White");
        sLeaser.sprites[Spr_SliderBack].color = Color.black;
        sLeaser.sprites[Spr_SliderBack].scaleX = (width + 4f) / sLeaser.sprites[Spr_SliderBack].width;
        sLeaser.sprites[Spr_SliderBack].scaleY = 10f / sLeaser.sprites[Spr_SliderBack].height;
        sLeaser.sprites[Spr_SliderFront] = new FSprite("Futile_White");
        sLeaser.sprites[Spr_SliderFront].scaleX = width / sLeaser.sprites[Spr_SliderFront].width;
        sLeaser.sprites[Spr_SliderFront].scaleY = 10f / sLeaser.sprites[Spr_SliderFront].height;

        sLeaser.sprites[Spr_SliderScrubberBack] = new FSprite("Futile_White");
        sLeaser.sprites[Spr_SliderScrubberBack].scaleX = 48f / sLeaser.sprites[Spr_SliderScrubberBack].width;
        sLeaser.sprites[Spr_SliderScrubberBack].scaleY = 48f / sLeaser.sprites[Spr_SliderScrubberBack].height;
        sLeaser.sprites[Spr_SliderScrubberBack].rotation = 45f;
        sLeaser.sprites[Spr_SliderScrubberBack].alpha = 0.8f;
        sLeaser.sprites[Spr_SliderScrubberBack].color = Color.black;
        sLeaser.sprites[Spr_SliderScrubberFront] = new FSprite("outlineBoxMed");
        sLeaser.sprites[Spr_SliderScrubberFront].scaleX = 43f / sLeaser.sprites[Spr_SliderScrubberFront].width;
        sLeaser.sprites[Spr_SliderScrubberFront].scaleY = 43f / sLeaser.sprites[Spr_SliderScrubberFront].height;
        sLeaser.sprites[Spr_SliderScrubberFront].rotation = 45f;
    }

    public override void DrawSprites(SpriteLeaser sLeaser, float timeStacker)
    {
        Vector2 drawPos = new Vector2(Mathf.Lerp(this.lastPos.x, this.pos.x, timeStacker), Mathf.Lerp(this.lastPos.y, this.pos.y, timeStacker));
        sLeaser.sprites[Spr_SliderBack].x = drawPos.x;
        sLeaser.sprites[Spr_SliderBack].y = drawPos.y-4f;
        sLeaser.sprites[Spr_SliderFront].x = drawPos.x;
        sLeaser.sprites[Spr_SliderFront].y = drawPos.y;

        sLeaser.sprites[Spr_SliderScrubberBack].x = (int)(drawPos.x - width * 0.5f + progress * width);
        sLeaser.sprites[Spr_SliderScrubberBack].y = (int)(sLeaser.sprites[Spr_SliderFront].y);
        sLeaser.sprites[Spr_SliderScrubberFront].x = (int)(sLeaser.sprites[Spr_SliderScrubberBack].x);
        sLeaser.sprites[Spr_SliderScrubberFront].y = (int)(sLeaser.sprites[Spr_SliderScrubberBack].y);
        base.DrawSprites(sLeaser, timeStacker);
    }
}
