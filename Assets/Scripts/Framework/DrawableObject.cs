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

    public virtual void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
    {
        defaultContainer = newContainer;
        foreach (FSprite fsprite in sGroup.sprites) {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
    }

    public virtual void ReInitiateSprites(SpriteGroup sGroup)
    {
        foreach (FSprite fsprite in sGroup.sprites) {
            fsprite.RemoveFromContainer();
        }
        sGroup.sprites = null;
        InitiateSprites(sGroup);
        AddToContainer(sGroup, defaultContainer);
    }

    public FContainer defaultContainer;
    public Vector2 pos;
    public Vector2 lastPos;
}
