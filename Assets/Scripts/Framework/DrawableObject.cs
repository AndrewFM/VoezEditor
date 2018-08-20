using System;
using UnityEngine;

public class DrawableObject : UpdatableObject, IDrawable {
    public override void Update()
    {
        lastPos = pos;
        base.Update();
    }

    public virtual void InitiateSprites(SpriteGroup sGroup)
    {
    }

    public virtual void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        if (readyForDeletion)
            sGroup.CleanSpritesAndRemove();
    }

    public virtual void AddToContainer(SpriteGroup sGroup, FContainer newContatiner)
    {
        foreach (FSprite fsprite in sGroup.sprites) {
            fsprite.RemoveFromContainer();
            newContatiner.AddChild(fsprite);
        }
    }

    public Vector2 pos;
    public Vector2 lastPos;
}
