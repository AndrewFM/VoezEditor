using UnityEngine;
using System.Collections;

public class BackgroundImage : DrawableObject {
    //public FShader blurShader;
    public float pulseFlashEffectTime;

    public BackgroundImage(Texture2D image)
    {
        if (image != null) {
            if (Futile.atlasManager.GetAtlasWithName("background") != null)
                Futile.atlasManager.UnloadAtlas("background");
            Futile.atlasManager.LoadAtlasFromTexture("background", image);
        }
        //blurShader = FShader.CreateShader("GaussianBlur", Shader.Find("Futile/GaussianBlur"));
    }

    public override void Update()
    {
        if (pulseFlashEffectTime > 0)
            pulseFlashEffectTime -= 1;
        base.Update();
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[4];
        if (Futile.atlasManager.GetAtlasWithName("background") != null) {
            sGroup.sprites[0] = new FSprite("background");
            sGroup.sprites[0].anchorX = 0f;
            sGroup.sprites[0].anchorY = 0f;
            sGroup.sprites[0].scaleX = VoezEditor.windowRes.x / sGroup.sprites[0].width;
            sGroup.sprites[0].scaleY = VoezEditor.windowRes.y / sGroup.sprites[0].height;
        }
        else {
            // Sanity check: user did not provide a background image, use a solid grey color instead.
            sGroup.sprites[0] = new FSprite("Futile_White");
            sGroup.sprites[0].anchorX = 0f;
            sGroup.sprites[0].anchorY = 0f;
            sGroup.sprites[0].scaleX = VoezEditor.windowRes.x / sGroup.sprites[0].width;
            sGroup.sprites[0].scaleY = VoezEditor.windowRes.y / sGroup.sprites[0].height;
            sGroup.sprites[0].color = Color.gray;
        }

        // BPM Screen Edge Pulsers
        sGroup.sprites[sGroup.sprites.Length - 1] = new FSprite("bottomGradient");
        sGroup.sprites[sGroup.sprites.Length - 2] = new FSprite("bottomGradient");
        sGroup.sprites[sGroup.sprites.Length - 3] = new FSprite("bottomGradient");
        sGroup.sprites[sGroup.sprites.Length - 1].rotation = 90;
        sGroup.sprites[sGroup.sprites.Length - 2].rotation = 180;
        sGroup.sprites[sGroup.sprites.Length - 3].rotation = 270;
        sGroup.sprites[sGroup.sprites.Length - 1].scaleX = (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / sGroup.sprites[sGroup.sprites.Length - 1].width;
        sGroup.sprites[sGroup.sprites.Length - 2].scaleX = VoezEditor.windowRes.x / sGroup.sprites[sGroup.sprites.Length - 2].width;
        sGroup.sprites[sGroup.sprites.Length - 3].scaleX = (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT) / sGroup.sprites[sGroup.sprites.Length - 3].width;
        sGroup.sprites[sGroup.sprites.Length - 1].alpha = 1f;
        sGroup.sprites[sGroup.sprites.Length - 2].alpha = 1f;
        sGroup.sprites[sGroup.sprites.Length - 3].alpha = 1f;
        sGroup.sprites[sGroup.sprites.Length - 1].anchorY = 0f;
        sGroup.sprites[sGroup.sprites.Length - 2].anchorY = 0f;
        sGroup.sprites[sGroup.sprites.Length - 3].anchorY = 0f;
        sGroup.sprites[sGroup.sprites.Length - 1].x = 0f;
        sGroup.sprites[sGroup.sprites.Length - 2].x = VoezEditor.windowRes.x * 0.5f;
        sGroup.sprites[sGroup.sprites.Length - 3].x = VoezEditor.windowRes.x;
        sGroup.sprites[sGroup.sprites.Length - 1].y = VoezEditor.windowRes.y - (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT * 0.5f);
        sGroup.sprites[sGroup.sprites.Length - 2].y = VoezEditor.windowRes.y;
        sGroup.sprites[sGroup.sprites.Length - 3].y = VoezEditor.windowRes.y - (VoezEditor.windowRes.y * Track.TRACK_SCREEN_HEIGHT * 0.5f);

        //sGroup.sprites[0].shader = blurShader;
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        if (pulseFlashEffectTime > 0) {
            sGroup.sprites[sGroup.sprites.Length - 1].alpha = 0.3f;
            sGroup.sprites[sGroup.sprites.Length - 2].alpha = 0.3f;
            sGroup.sprites[sGroup.sprites.Length - 3].alpha = 0.3f;
        } else {
            sGroup.sprites[sGroup.sprites.Length - 1].alpha = Mathf.Lerp(sGroup.sprites[sGroup.sprites.Length - 1].alpha, 0f, 0.15f);
            sGroup.sprites[sGroup.sprites.Length - 2].alpha = Mathf.Lerp(sGroup.sprites[sGroup.sprites.Length - 2].alpha, 0f, 0.15f);
            sGroup.sprites[sGroup.sprites.Length - 3].alpha = Mathf.Lerp(sGroup.sprites[sGroup.sprites.Length - 3].alpha, 0f, 0.15f);
        }

        base.DrawSprites(sGroup, frameProgress);
    }
}
