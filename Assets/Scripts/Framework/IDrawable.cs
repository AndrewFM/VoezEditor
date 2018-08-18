using System;
using UnityEngine;

public interface IDrawable {
    void InitiateSprites(SpriteLeaser sLeaser);
    void DrawSprites(SpriteLeaser sLeaser, float timeStacker);
    void AddToContainer(SpriteLeaser sLeaser, FContainer newContatiner);
}
