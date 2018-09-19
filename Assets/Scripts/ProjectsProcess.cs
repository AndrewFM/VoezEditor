using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProjectsProcess : MainLoopProcess {

    public List<UpdatableObject> updateList;
    public List<SpriteGroup> spriteGroups;
    public FContainer songsContainer;
    public FContainer foregroundContainer;
    public MusicPlayer musicPlayer;
    public ProjectIcon[] icons;
    public List<string> projectDirectories;
    public ProjectsUI ui;
    public bool init;
    public float scrollOffset;
    public float targetScroll;
    public float lastScrollOffset;
    public bool mouseDragInit;
    public static float GRID_MARGIN = 16f;
    

    public ProjectsProcess()
    {
        updateList = new List<UpdatableObject>();
        spriteGroups = new List<SpriteGroup>();
        Futile.atlasManager.LoadAtlas("Atlases/projectsAtlas");

        // Find all projects
        projectDirectories = new List<string>();
        string rootDirectory = Application.dataPath + "/../ActiveProject";
        string[] subDirectories = Directory.GetDirectories(rootDirectory);
        for(int i=0; i<subDirectories.Length; i+=1) {
            string[] projectFiles = Directory.GetFiles(subDirectories[i]);
            for(int j=0; j<projectFiles.Length; j+=1) {
                if (projectFiles[j].Contains("song_")) {
                    // Has a song file, so meets minimum requirements to be considered a project. Add to list.
                    projectDirectories.Add(subDirectories[i]);
                    break;
                }
            }
        }

        scrollOffset = ProjectIcon.size * 0.5f;
        targetScroll = scrollOffset;

        // Setup drawing layers (order matters here)
        songsContainer = new FContainer();
        foregroundContainer = new FContainer();
        Futile.stage.AddChild(songsContainer);
        Futile.stage.AddChild(foregroundContainer);
    }

    public override void Update()
    {
        base.Update();

        // Post-Initialization
        if (!init) {
            init = true;
            AddObject(new SolidBackground(Color.white));
            ProjectIcon activeIcon = null;
            icons = new ProjectIcon[26];
            for (int i = 0; i < icons.Length; i += 1) {
                int projectInd = IconIndToProjectInd(i) - 3;
                if (projectInd < 0 || projectInd >= projectDirectories.Count)
                    icons[i] = new ProjectIcon(null, Vector2.zero);
                else {
                    icons[i] = new ProjectIcon(projectDirectories[projectInd], Vector2.zero);
                    if (projectDirectories[projectInd] == VoezEditor.activeProjectFolder)
                        activeIcon = icons[i];
                }
                AddObject(icons[i]);
            }
            ui = new ProjectsUI();
            musicPlayer = new MusicPlayer();
            musicPlayer.loopMode = true;
            if (activeIcon != null)
                SetSelectedProject(activeIcon);
        }

        // Control of scrolling the list around
        float snap = (ProjectIcon.size + GRID_MARGIN) * 0.5f;
        scrollOffset = Mathf.Lerp(scrollOffset, targetScroll, 0.1f);
        if (!Input.GetMouseButton(0))
            mouseDragInit = false;
        if (InputManager.leftMousePushed && !ui.HoveringOverSubmenuItem()) {
            lastScrollOffset = targetScroll;
            mouseDragInit = true;
        }
        if (InputManager.leftMouseReleased) {
            if ((InputManager.screenPosOnLeftMousePush.x - Input.mousePosition.x) < 0)
                targetScroll = Mathf.Floor(targetScroll / snap) * snap;
            else
                targetScroll = Mathf.Ceil(targetScroll / snap) * snap;
        }
        if (Input.GetMouseButton(0) && Vector2.Distance(InputManager.screenPosOnLeftMousePush, Input.mousePosition) > 16 && mouseDragInit) {
            targetScroll = lastScrollOffset + (InputManager.screenPosOnLeftMousePush.x - Input.mousePosition.x);
        } else {
            float multi = 1;
            if (Util.ShiftDown())
                multi = 4;
            if (InputManager.UpTick() || InputManager.RightTick())
                targetScroll += snap * multi;
            if (InputManager.DownTick() || InputManager.LeftTick())
                targetScroll -= snap * multi;
        }
        if (targetScroll < 0)
            targetScroll = 0;

        musicPlayer.Update();
        LayoutProjectIcons();

        // Update all active objects
        int updateIndex = updateList.Count - 1;
        while (updateIndex >= 0) {
            UpdatableObject obj = updateList[updateIndex];
            if (obj.readyForDeletion)
                PurgeObject(obj);
            else
                obj.Update();
            updateIndex--;
        }
        ui.Update();

        if (readyToShutDown) {
            ShutDownProcess();
            VoezEditor.activeProcess = new EditorProcess();
        }
    }

    public void SetSelectedProject(ProjectIcon project)
    {
        ui.selector.linkedIcon = project;
        ui.SetSongName(project.data.songName);
        ui.SetAuthorName(project.data.author);
        musicPlayer.CrossfadeIntoClip(project.data.songPVPath);
        ui.UpdateEditInterface();
        VoezEditor.activeProjectFolder = project.data.projectFolder;
    }

    public int ProjectIndToIconInd(int ind)
    {
        int baseInd = 1;
        if (ind % 4 == 1)
            baseInd = 3;
        else if (ind % 4 == 2)
            baseInd = 0;
        else if (ind % 4 == 3)
            baseInd = 2;
        return baseInd + 4 * Mathf.FloorToInt(ind / 4);
    }

    public int IconIndToProjectInd(int ind)
    {
        int baseInd = 2;
        if (ind % 4 == 1)
            baseInd = 0;
        else if (ind % 4 == 2)
            baseInd = 3;
        else if (ind % 4 == 3)
            baseInd = 1;
        return baseInd + 4 * Mathf.FloorToInt(ind / 4);
    }

    public void LayoutProjectIcons()
    {
        float relScrollOff = -scrollOffset;
        for (int i = 0; i < icons.Length; i += 1) {
            int posInd = i;
            icons[i].pos.x = relScrollOff + (ProjectIcon.size + GRID_MARGIN) * Mathf.Floor(posInd / 4);
            icons[i].pos.y = VoezEditor.windowRes.y - ProjectIcon.margin - (ProjectIcon.size + GRID_MARGIN) * (i % 2);
            if (posInd % 4 >= 2) {
                icons[i].pos.x += ProjectIcon.size / 2f + GRID_MARGIN / 2f;
                icons[i].pos.y -= ProjectIcon.size / 2f + GRID_MARGIN / 2f;
            }
            icons[i].pos.x += ProjectIcon.size / 2f;
            icons[i].pos.y -= ProjectIcon.size / 2f;
        }
    }

    public void AddObject(UpdatableObject obj)
    {
        this.updateList.Add(obj);
        if (obj is IDrawable) {
            SpriteGroup group = new SpriteGroup(obj as IDrawable);
            spriteGroups.Add(group);
            if (obj is UIElement)
                group.AddSpritesToContainer(foregroundContainer);
            else
                group.AddSpritesToContainer(songsContainer);
        }
    }

    private void PurgeObject(UpdatableObject obj)
    {
        updateList.Remove(obj);
        if (obj is IDrawable) {
            for (int i = spriteGroups.Count - 1; i >= 0; i--) {
                if (spriteGroups[i].drawableObject == obj) {
                    spriteGroups[i].CleanSpritesAndRemove();
                    break;
                }
            }
        }
    }

    public override void DrawUpdate(float frameProgress)
    {
        base.DrawUpdate(frameProgress);
        for (int i = spriteGroups.Count - 1; i >= 0; i--) {
            spriteGroups[i].Update(frameProgress);
            if (spriteGroups[i].deleteMeNextFrame)
                spriteGroups.RemoveAt(i);
        }
    }

    public override void ShutDownProcess()
    {
        base.ShutDownProcess();
        Futile.atlasManager.UnloadAtlas("Atlases/projectsAtlas");
        if (musicPlayer != null)
            musicPlayer.Destroy();
        int updateIndex = updateList.Count - 1;
        while (updateIndex >= 0) {
            PurgeObject(updateList[updateIndex]);
            updateIndex--;
        }
        ui.Unload();
    }
}
