using UnityEngine;
using System.Collections;

public class BackgroundImage : DrawableObject {
    //public FShader blurShader;

    public BackgroundImage(Texture2D image)
    {
        if (image != null) {
            if (Futile.atlasManager.GetAtlasWithName("background") != null)
                Futile.atlasManager.UnloadAtlas("background");
            Futile.atlasManager.LoadAtlasFromTexture("background", image);
        }
        //blurShader = FShader.CreateShader("GaussianBlur", Shader.Find("Futile/GaussianBlur"));
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[1];
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
        //sGroup.sprites[0].shader = blurShader;
    }
}
