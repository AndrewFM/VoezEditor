using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class VoezEditor : MonoBehaviour {

    public static MainLoopProcess activeProcess;
    public static Vector2 windowRes = new Vector2(1280f, 720f);

    // Use this for initialization
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, true, true, true);
        futileParams.AddResolutionLevel(windowRes.x, 1f, 1f, string.Empty);
        futileParams.origin = new Vector2(0f, 0f);
        Futile.instance.Init(futileParams);
        Futile.displayScale = 1f;

        // Load Resources
        Futile.atlasManager.LoadAtlas("Atlases/mainAtlas");
        Futile.atlasManager.LoadAtlas("Atlases/fontAtlas");
        Futile.atlasManager.LoadFont("Raleway32", "Raleway32", "Atlases/Raleway32", 0f, 0f);
        Futile.atlasManager.LoadFont("Raleway24", "Raleway24", "Atlases/Raleway24", 0f, 0f);
        Futile.atlasManager.LoadFont("Raleway16", "Raleway16", "Atlases/Raleway16", 0f, 0f);

        activeProcess = new EditorProcess();
    }

    // Update is called once per frame
    void Update()
    {
        activeProcess.RawUpdate(Time.deltaTime);
    }

    public static EditorProcess Editor {
        get { return (activeProcess != null && activeProcess is EditorProcess) ? (activeProcess as EditorProcess) : null;  }
    }

    private void Awake()
    {
        if (File.Exists("exceptionLog.txt"))
            File.Delete("exceptionLog.txt");
        Application.logMessageReceived += new Application.LogCallback(this.HandleLog);
    }

    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception) {
            File.AppendAllText("exceptionLog.txt", logString + Environment.NewLine);
            File.AppendAllText("exceptionLog.txt", stackTrace + Environment.NewLine);
            return;
        }
    }
}
