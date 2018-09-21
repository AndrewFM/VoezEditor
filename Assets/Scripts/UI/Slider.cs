using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : UIElement {
    public float width;
    public float progress;
    public float pendingProgress;
    public bool progressUpdate;
    public bool allowScrubbing;
    public static float SCRUBBER_SIZE = 64f;
    public bool clicked;
    public bool rightClicked;
    public float pulseFlashEffectTime;
    public float loopPoint = -1;

    public Slider(Vector2 pos, float width)
    {
        this.pos = pos;
        this.width = width;
        allowScrubbing = true;
    }

    public int Spr_SliderBack { get { return 0; } }
    public int Spr_SliderFront { get { return 1; } }
    public int Spr_SliderScrubberBack { get { return 2; } }
    public int Spr_SliderScrubberFront { get { return 3; } }
    public int Spr_LoopPointBack { get { return 4; } }
    public int Spr_LoopPointFront { get { return 5; } }

    public bool MouseOver
    {
        get {
            Vector3 mouse = Input.mousePosition;
            float dx = Mathf.Abs(mouse.x - ProgressX());
            float dy = Mathf.Abs(mouse.y - pos.y);
            return (dx / SCRUBBER_SIZE + dy / SCRUBBER_SIZE) <= 0.5f;
        }
    }

    public float ProgressX()
    {
        if (clicked)
            return pos.x - width * 0.5f + pendingProgress * width;
        else
            return pos.x - width * 0.5f + progress * width;
    }

    public override void Update()
    {
        if (InputManager.leftMousePushed && allowScrubbing && MouseOver && !VoezEditor.confirmBoxOpen)
            clicked = true;
        if (InputManager.rightMousePushed && allowScrubbing && MouseOver && !VoezEditor.confirmBoxOpen)
            rightClicked = true;
        if (clicked)
            pendingProgress = Mathf.Clamp((Input.mousePosition.x - (pos.x - width * 0.5f)) / width, 0f, 1f);
        if (clicked && !Input.GetMouseButton(0)) {
            clicked = false;
            progress = pendingProgress;
            progressUpdate = true;
        }
        if (pulseFlashEffectTime > 0)
            pulseFlashEffectTime -= 1;
        base.Update();
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[6];

        sGroup.sprites[Spr_SliderBack] = new FSprite("Futile_White");
        sGroup.sprites[Spr_SliderBack].color = Color.black;
        sGroup.sprites[Spr_SliderBack].scaleX = (width + 4f) / sGroup.sprites[Spr_SliderBack].width;
        sGroup.sprites[Spr_SliderBack].scaleY = 10f / sGroup.sprites[Spr_SliderBack].height;
        sGroup.sprites[Spr_SliderFront] = new FSprite("Futile_White");
        sGroup.sprites[Spr_SliderFront].scaleX = width / sGroup.sprites[Spr_SliderFront].width;
        sGroup.sprites[Spr_SliderFront].scaleY = 10f / sGroup.sprites[Spr_SliderFront].height;

        sGroup.sprites[Spr_SliderScrubberBack] = new FSprite("Futile_White");
        sGroup.sprites[Spr_SliderScrubberBack].scaleX = SCRUBBER_SIZE / Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[Spr_SliderScrubberBack].width, 2f));
        sGroup.sprites[Spr_SliderScrubberBack].scaleY = SCRUBBER_SIZE / Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[Spr_SliderScrubberBack].height, 2f));
        sGroup.sprites[Spr_SliderScrubberBack].rotation = 45f;
        sGroup.sprites[Spr_SliderScrubberBack].alpha = 0.8f;
        sGroup.sprites[Spr_SliderScrubberBack].color = Color.black;
        sGroup.sprites[Spr_SliderScrubberFront] = new FSprite("outlineBoxMed");
        sGroup.sprites[Spr_SliderScrubberFront].scaleX = (SCRUBBER_SIZE - 5f) / Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[Spr_SliderScrubberFront].width, 2f));
        sGroup.sprites[Spr_SliderScrubberFront].scaleY = (SCRUBBER_SIZE - 5f) / Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[Spr_SliderScrubberFront].height, 2f));
        sGroup.sprites[Spr_SliderScrubberFront].rotation = 45f;

        sGroup.sprites[Spr_LoopPointBack] = new FSprite("Futile_White");
        sGroup.sprites[Spr_LoopPointBack].scaleX = (SCRUBBER_SIZE * 0.5f) / Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[Spr_LoopPointBack].width, 2f));
        sGroup.sprites[Spr_LoopPointBack].scaleY = (SCRUBBER_SIZE * 0.5f) / Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[Spr_LoopPointBack].height, 2f));
        sGroup.sprites[Spr_LoopPointBack].rotation = 45f;
        sGroup.sprites[Spr_LoopPointBack].color = Color.red;
        sGroup.sprites[Spr_LoopPointFront] = new FSprite("outlineBoxMed");
        sGroup.sprites[Spr_LoopPointFront].scaleX = ((SCRUBBER_SIZE * 0.5f) - 5f) / Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[Spr_LoopPointFront].width, 2f));
        sGroup.sprites[Spr_LoopPointFront].scaleY = ((SCRUBBER_SIZE * 0.5f) - 5f) / Mathf.Sqrt(2f * Mathf.Pow(sGroup.sprites[Spr_LoopPointFront].height, 2f));
        sGroup.sprites[Spr_LoopPointFront].rotation = 45f;
        sGroup.sprites[Spr_LoopPointFront].color = Color.white;
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        Vector2 drawPos = new Vector2(Mathf.Lerp(this.lastPos.x, this.pos.x, frameProgress), Mathf.Lerp(this.lastPos.y, this.pos.y, frameProgress));
        sGroup.sprites[Spr_SliderBack].x = drawPos.x;
        sGroup.sprites[Spr_SliderBack].y = drawPos.y-4f;
        sGroup.sprites[Spr_SliderFront].x = drawPos.x;
        sGroup.sprites[Spr_SliderFront].y = drawPos.y;

        sGroup.sprites[Spr_SliderScrubberBack].x = (int)ProgressX();
        sGroup.sprites[Spr_SliderScrubberBack].y = (int)(sGroup.sprites[Spr_SliderFront].y);
        sGroup.sprites[Spr_SliderScrubberFront].x = (int)(sGroup.sprites[Spr_SliderScrubberBack].x);
        sGroup.sprites[Spr_SliderScrubberFront].y = (int)(sGroup.sprites[Spr_SliderScrubberBack].y);

        if ((MouseOver || clicked) && allowScrubbing && !VoezEditor.confirmBoxOpen) {
            sGroup.sprites[Spr_SliderScrubberBack].color = Color.Lerp(sGroup.sprites[Spr_SliderScrubberBack].color, Color.red, 0.15f);
            sGroup.sprites[Spr_SliderScrubberBack].alpha = 1f;
        } else {
            if (pulseFlashEffectTime > 0) {
                sGroup.sprites[Spr_SliderScrubberBack].color = Color.white;
                sGroup.sprites[Spr_SliderScrubberBack].alpha = 1f;
            } else {
                sGroup.sprites[Spr_SliderScrubberBack].color = Color.Lerp(sGroup.sprites[Spr_SliderScrubberBack].color, Color.black, 0.15f);
                sGroup.sprites[Spr_SliderScrubberBack].alpha = 0.8f;
            }
        }

        if (loopPoint >= 0f) {
            sGroup.sprites[Spr_LoopPointBack].isVisible = true;
            sGroup.sprites[Spr_LoopPointFront].isVisible = true;
            sGroup.sprites[Spr_LoopPointBack].x = (int)(pos.x - width * 0.5f + loopPoint * width);
            sGroup.sprites[Spr_LoopPointBack].y = (int)(sGroup.sprites[Spr_SliderFront].y);
            sGroup.sprites[Spr_LoopPointFront].x = (int)(sGroup.sprites[Spr_LoopPointBack].x);
            sGroup.sprites[Spr_LoopPointFront].y = (int)(sGroup.sprites[Spr_LoopPointBack].y);
        }
        else {
            sGroup.sprites[Spr_LoopPointBack].isVisible = false;
            sGroup.sprites[Spr_LoopPointFront].isVisible = false;
        }
        base.DrawSprites(sGroup, frameProgress);
    }
}
