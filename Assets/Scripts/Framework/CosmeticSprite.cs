using System;
using UnityEngine;

public class CosmeticSprite : UpdatableAndDeletable, IDrawable {
    public override void Update(bool eu)
    {
        lastPos = pos;
        base.Update(eu);
    }

    public virtual void InitiateSprites(SpriteLeaser sLeaser)
    {
    }

    public virtual void DrawSprites(SpriteLeaser sLeaser, float timeStacker)
    {
        if (slatedForDeletetion)
            sLeaser.CleanSpritesAndRemove();
    }

    public virtual void AddToContainer(SpriteLeaser sLeaser, FContainer newContatiner)
    {
        foreach (FSprite fsprite in sLeaser.sprites) {
            fsprite.RemoveFromContainer();
            newContatiner.AddChild(fsprite);
        }
    }

    public Vector2 pos;
    public Vector2 lastPos;
}
