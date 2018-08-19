using UnityEngine;
using System.Collections;

public class Track : CosmeticSprite {
    public ProjectData.TrackData data;
    public EditorProcess controller;
    public float trackProgress;
    public static float TRACK_SCREEN_HEIGHT = 0.85f;
    public static float TRACK_SCREEN_WIDTH = 0.12f; // at size = 1.0f
    public int ID;

    public float currentWidth;
    public int lastSubColor;
    public int lastSubColorInd = -1;
    public float lastSubScale;
    public int lastSubScaleInd = -1;

    public float flashEffectTime;

    public Track(EditorProcess parent, ProjectData.TrackData data)
    {
        this.data = data;
        controller = parent;
        ID = data.id; 
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        trackProgress = (controller.songTime - data.start) / (data.end - data.start);
        pos.y = MainScript.windowRes.y * (1.0f - TRACK_SCREEN_HEIGHT);
        float desiredX = GetXAtTime(controller.songTime);
        pos.x = Util.ScreenPosX(desiredX);

        if (trackProgress > 1f || trackProgress < 0f)
            slatedForDeletetion = true;
        if (flashEffectTime > 0)
            flashEffectTime -= 1;
    }

    // Move Tweening
    public float GetXAtTime(float time)
    {
        float desiredX = data.x;
        float lastSubMove = data.x;
        for (int i = 0; i < data.move.Count; i += 1) {
            if (time >= data.move[i].start && time < data.move[i].end) {
                if (i == 0)
                    lastSubMove = data.x;
                else
                    lastSubMove = data.move[i - 1].to;
                float subMoveProgress = (time - data.move[i].start) / (data.move[i].end - data.move[i].start);
                desiredX = data.move[i].GetEaseFunction()(lastSubMove, data.move[i].to, Mathf.Clamp(subMoveProgress, 0, 1));
            } else if ((i < data.move.Count - 1 && time >= data.move[i].end && time < data.move[i + 1].start)
                   || ((i == data.move.Count - 1 && time >= data.move[i].end && time < data.end)))
                desiredX = data.move[i].to;
        }
        return desiredX;
    }

    public int Spr_BackGradient { get { return 0; } }
    public int Spr_LeftGradLine { get { return 2; } }
    public int Spr_RightGradLine { get { return 3; } }
    public int Spr_MiddleGradLine { get { return 4; } }
    public int Spr_BottomGradFade { get { return 1; } }
    public int Spr_BottomDiamond { get { return 5; } }

    public override void InitiateSprites(SpriteLeaser sLeaser)
    {
        sLeaser.sprites = new FSprite[6];
        sLeaser.sprites[Spr_BackGradient] = new FSprite("trackGradient");
        sLeaser.sprites[Spr_BackGradient].scaleY = (MainScript.windowRes.y / sLeaser.sprites[Spr_BackGradient].height) * TRACK_SCREEN_HEIGHT;
        sLeaser.sprites[Spr_BackGradient].scaleX = (MainScript.windowRes.x / sLeaser.sprites[Spr_BackGradient].width) * TRACK_SCREEN_WIDTH * data.size;
        sLeaser.sprites[Spr_BackGradient].color = ProjectData.colors[data.color];
        sLeaser.sprites[Spr_BackGradient].anchorY = 0f;
        sLeaser.sprites[Spr_BackGradient].alpha = 0.9f;

        sLeaser.sprites[Spr_BottomGradFade] = new FSprite("bottomGradient");
        sLeaser.sprites[Spr_BottomGradFade].scaleY = 1f;
        sLeaser.sprites[Spr_BottomGradFade].scaleX = (MainScript.windowRes.x / sLeaser.sprites[Spr_BottomGradFade].width) * TRACK_SCREEN_WIDTH * data.size;
        sLeaser.sprites[Spr_BottomGradFade].color = Color.white;
        sLeaser.sprites[Spr_BottomGradFade].anchorY = 0f;
        sLeaser.sprites[Spr_BottomGradFade].alpha = 1f;

        for (int i=Spr_LeftGradLine; i<=Spr_MiddleGradLine; i+=1) {
            sLeaser.sprites[i] = new FSprite("trackGradient");
            sLeaser.sprites[i].scaleY = sLeaser.sprites[Spr_BackGradient].scaleY;
            sLeaser.sprites[i].scaleX = 0.5f;
            if (i == Spr_MiddleGradLine)
                sLeaser.sprites[i].color = Color.black;
            else
                sLeaser.sprites[i].color = Color.white;
            sLeaser.sprites[i].anchorY = 0f;
        }

        sLeaser.sprites[Spr_BottomDiamond] = new FSprite("slide");
        sLeaser.sprites[Spr_BottomDiamond].color = Color.black;
        sLeaser.sprites[Spr_BottomDiamond].rotation = 45f;
        sLeaser.sprites[Spr_BottomDiamond].scale = 0.5f;
    }

    public override void AddToContainer(SpriteLeaser sLeaser, FContainer newContainer)
    {
        foreach (FSprite fsprite in sLeaser.sprites)
            fsprite.RemoveFromContainer();
        controller.tracksBottomContainer.AddChild(sLeaser.sprites[Spr_BackGradient]);
        controller.tracksBottomContainer.AddChild(sLeaser.sprites[Spr_BottomGradFade]);
        controller.tracksTopContainer.AddChild(sLeaser.sprites[Spr_LeftGradLine]);
        controller.tracksTopContainer.AddChild(sLeaser.sprites[Spr_RightGradLine]);
        controller.tracksTopContainer.AddChild(sLeaser.sprites[Spr_MiddleGradLine]);
        controller.tracksTopContainer.AddChild(sLeaser.sprites[Spr_BottomDiamond]);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, float timeStacker)
    {
        if (lastPos == Vector2.zero || pos == Vector2.zero) {
            for(int i=0; i<sLeaser.sprites.Length; i+=1)
                sLeaser.sprites[i].isVisible = false;
        } else {
            for (int i = 0; i < sLeaser.sprites.Length; i += 1)
                sLeaser.sprites[i].isVisible = true;
        }

        // Color Tweening
        for (int i = 0; i < data.colorChange.Count; i += 1) {
            if (controller.songTime >= data.colorChange[i].start && controller.songTime < data.colorChange[i].end) {
                if (i != lastSubColorInd) {
                    if (i == 0)
                        lastSubColor = data.color;
                    else
                        lastSubColor = (int)data.colorChange[i - 1].to;
                    lastSubColorInd = i;
                }
                float subColorProgress = (controller.songTime - data.colorChange[i].start) / (data.colorChange[i].end - data.colorChange[i].start);
                sLeaser.sprites[Spr_BackGradient].color = Color.Lerp(ProjectData.colors[lastSubColor], ProjectData.colors[(int)data.colorChange[i].to], data.colorChange[i].GetEaseFunction()(0f, 1f, Mathf.Clamp(subColorProgress, 0, 1)));
            } else if ((i < data.colorChange.Count - 1 && controller.songTime >= data.colorChange[i].end && controller.songTime < data.colorChange[i + 1].start)
                   || ((i == data.colorChange.Count - 1 && controller.songTime >= data.colorChange[i].end && controller.songTime < data.end)))
                sLeaser.sprites[Spr_BackGradient].color = ProjectData.colors[(int)data.colorChange[i].to];
        }

        // Scale Tweening
        currentWidth = MainScript.windowRes.x * TRACK_SCREEN_WIDTH * data.size;
        for (int i = 0; i < data.scale.Count; i += 1) {
            if (controller.songTime >= data.scale[i].start && controller.songTime < data.scale[i].end) {
                if (i != lastSubScaleInd) {
                    if (i == 0)
                        lastSubScale = data.size;
                    else
                        lastSubScale = (int)data.scale[i - 1].to;
                    lastSubScaleInd = i;
                }
                float subScaleProgress = (controller.songTime - data.scale[i].start) / (data.scale[i].end - data.scale[i].start);
                currentWidth = MainScript.windowRes.x * TRACK_SCREEN_WIDTH;
                currentWidth *= data.scale[i].GetEaseFunction()(lastSubScale, data.scale[i].to, Mathf.Clamp(subScaleProgress, 0, 1));
            } else if ((i < data.scale.Count - 1 && controller.songTime >= data.scale[i].end && controller.songTime < data.scale[i + 1].start)
                   || ((i == data.scale.Count - 1 && controller.songTime >= data.scale[i].end && controller.songTime < data.end)))
                currentWidth = MainScript.windowRes.x * TRACK_SCREEN_WIDTH * data.scale[i].to;
        }
        sLeaser.sprites[Spr_BackGradient].scaleX = currentWidth / sLeaser.sprites[Spr_BackGradient].element.sourceRect.width;
        sLeaser.sprites[Spr_BottomGradFade].scaleX = (currentWidth-14f) / (sLeaser.sprites[Spr_BottomGradFade].element.sourceRect.width);

        Vector2 lerpPos = new Vector2(Mathf.Lerp(lastPos.x, pos.x, timeStacker), Mathf.Lerp(lastPos.y, pos.y, timeStacker));
        sLeaser.sprites[Spr_BackGradient].x = lerpPos.x;
        sLeaser.sprites[Spr_LeftGradLine].x = lerpPos.x - currentWidth * 0.5f + 7f;
        sLeaser.sprites[Spr_RightGradLine].x = lerpPos.x + currentWidth * 0.5f - 7f;
        sLeaser.sprites[Spr_MiddleGradLine].x = lerpPos.x;
        sLeaser.sprites[Spr_BottomGradFade].x = lerpPos.x;
        sLeaser.sprites[Spr_BottomDiamond].x = lerpPos.x;
        for (int i = 0; i < sLeaser.sprites.Length; i += 1)
            sLeaser.sprites[i].y = lerpPos.y;

        if (flashEffectTime > 0) {
            sLeaser.sprites[Spr_BottomDiamond].scale = 1.25f;
        }
        else {
            sLeaser.sprites[Spr_BottomDiamond].scale = 0.5f;
        }

        base.DrawSprites(sLeaser, timeStacker);
    }
}
