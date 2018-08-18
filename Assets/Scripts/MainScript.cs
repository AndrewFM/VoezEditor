using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour {

    public MainLoopProcess activeProcess;
    public static Vector2 windowRes = new Vector2(1366f, 768f);

	// Use this for initialization
	void Start() {
		FutileParams futileParams = new FutileParams(true, true, true, true);
		futileParams.AddResolutionLevel(windowRes.x, 1f, 1f, string.Empty);
		futileParams.origin = new Vector2(0f, 0f);
		Futile.instance.Init(futileParams);
		Futile.displayScale = 1f;

		// Load Resources
		Futile.atlasManager.LoadAtlas("Atlases/mainAtlas");

        activeProcess = new EditorProcess();
	}
	
	// Update is called once per frame
	void Update() {
        activeProcess.RawUpdate(Time.deltaTime);
	}
}
