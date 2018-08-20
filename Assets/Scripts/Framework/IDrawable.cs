using System;
using UnityEngine;

public interface IDrawable {
    void InitiateSprites(SpriteGroup sGroup);
    void DrawSprites(SpriteGroup sGroup, float frameProgress);
    void AddToContainer(SpriteGroup sGroup, FContainer newContatiner);
}
