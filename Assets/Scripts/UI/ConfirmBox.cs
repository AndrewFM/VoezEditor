using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmBox : UIElement {
    public Rect bounds;
    public FLabel text;
    public Button yesButton;
    public Button noButton;
    public Button cancelButton;
    public RectangleBorder border;
    public bool init;
    public bool enableCancel;

    public ConfirmBox(Rect bounds, string caption)
    {
        this.bounds = bounds;
        pos = bounds.center;
        VoezEditor.confirmBoxOpen = true;
        text = new FLabel("Raleway32", caption);
    }

    public override void Update()
    {
        float buttonSize = 128f;
        if (!init) {
            init = true;
            yesButton = new Button("Raleway32", "Yes", Vector2.zero, buttonSize, true);
            noButton = new Button("Raleway32", "No", Vector2.zero, buttonSize, true);
            yesButton.confirmComponent = true;
            noButton.confirmComponent = true;
            border = new RectangleBorder(new Rect(new Vector2(bounds.x + 5, bounds.y + 5), new Vector2(bounds.width - 10, bounds.height - 10)), 3f);
            VoezEditor.Editor.AddObject(yesButton);
            VoezEditor.Editor.AddObject(noButton);
            if (enableCancel) {
                cancelButton = new Button("Raleway32", "Cancel", Vector2.zero, buttonSize, true);
                cancelButton.confirmComponent = true;
                VoezEditor.Editor.AddObject(cancelButton);
            }
            VoezEditor.Editor.AddObject(border);
        }

        border.pos = pos;
        yesButton.pos.x = pos.x - bounds.width * 0.5f + buttonSize * 0.5f + 20f;
        noButton.pos.x = pos.x + bounds.width * 0.5f - buttonSize * 0.5f - 20f;
        yesButton.pos.y = pos.y - bounds.height * 0.5f + buttonSize * 0.5f + 20f;
        noButton.pos.y = yesButton.pos.y;
        if (cancelButton != null) {
            cancelButton.pos.x = (yesButton.pos.x + noButton.pos.x) / 2f;
            cancelButton.pos.y = yesButton.pos.y;
        }
        text.x = pos.x;
        text.y = pos.y + bounds.height * 0.5f - text.textRect.height * 0.5f - 20f;

        base.Update();
    }

    public override void InitiateSprites(SpriteGroup sGroup)
    {
        sGroup.sprites = new FSprite[1];
        sGroup.sprites[0] = new FSprite("Futile_White");
        sGroup.sprites[0].scaleX = bounds.width / sGroup.sprites[0].width;
        sGroup.sprites[0].scaleY = bounds.height / sGroup.sprites[0].height;
        sGroup.sprites[0].color = Color.black;
        sGroup.sprites[0].alpha = 0.8f;
    }

    public override void DrawSprites(SpriteGroup sGroup, float frameProgress)
    {
        Vector2 drawPos = new Vector2(Mathf.Lerp(lastPos.x, pos.x, frameProgress), Mathf.Lerp(lastPos.y, pos.y, frameProgress));
        sGroup.sprites[0].x = drawPos.x;
        sGroup.sprites[0].y = drawPos.y;
        base.DrawSprites(sGroup, frameProgress);
    }

    public override void AddToContainer(SpriteGroup sGroup, FContainer newContainer)
    {
        foreach (FSprite fsprite in sGroup.sprites) {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
        newContainer.AddChild(text);
    }

    public override void Destroy()
    {
        base.Destroy();
        yesButton.Destroy();
        noButton.Destroy();
        if (cancelButton != null)
            cancelButton.Destroy();
        border.Destroy();
        text.RemoveFromContainer();
        VoezEditor.confirmBoxOpen = false;
    }
}
