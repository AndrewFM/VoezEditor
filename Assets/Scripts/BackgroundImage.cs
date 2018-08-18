using UnityEngine;
using System.Collections;

public class BackgroundImage : CosmeticSprite {
    //public FShader blurShader;

    public BackgroundImage(Texture2D image)
    {
        if (Futile.atlasManager.GetAtlasWithName("background") != null)
            Futile.atlasManager.UnloadAtlas("background");
        Futile.atlasManager.LoadAtlasFromTexture("background", image);
        //blurShader = FShader.CreateShader("GaussianBlur", Shader.Find("Futile/GaussianBlur"));
    }

    public override void InitiateSprites(SpriteLeaser sLeaser)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("background");
        sLeaser.sprites[0].anchorX = 0f;
        sLeaser.sprites[0].anchorY = 0f;
        sLeaser.sprites[0].scaleX = MainScript.windowRes.x / sLeaser.sprites[0].width;
        sLeaser.sprites[0].scaleY = MainScript.windowRes.y / sLeaser.sprites[0].height;
        //sLeaser.sprites[0].shader = blurShader;
    }
}
