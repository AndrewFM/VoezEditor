﻿using UnityEngine;
using System.Collections;

public class SpriteLeaser {
    public SpriteLeaser(IDrawable obj)
    {
        this.drawableObject = obj;
        this.drawableObject.InitiateSprites(this);
    }

    public void Update(float timeStacker)
    {
        drawableObject.DrawSprites(this, timeStacker);
    }

    public void CleanSpritesAndRemove()
    {
        deleteMeNextFrame = true;
        RemoveAllSpritesFromContainer();
    }

    public void RemoveAllSpritesFromContainer()
    {
        for (int i = 0; i < sprites.Length; i++) {
            sprites[i].RemoveFromContainer();
        }
        if (containers != null) {
            for (int j = 0; j < containers.Length; j++) {
                containers[j].RemoveFromContainer();
            }
        }
    }

    public void AddSpritesToContainer(FContainer newContainer)
    {
        drawableObject.AddToContainer(this, newContainer);
    }

    public IDrawable drawableObject;
    public FSprite[] sprites;
    public bool deleteMeNextFrame;
    public FContainer[] containers;
}
