using System;

public abstract class MainLoopProcess {
    public int framesPerSecond;
    private float cumulativeDelta;
    public int framesSinceStart;
    public InputManager inputManager;

    public MainLoopProcess()
    {
        framesPerSecond = 60;
        inputManager = new InputManager();
    }

    public virtual void RawUpdate(float dt)
    {
        cumulativeDelta += dt * framesPerSecond;
        if (cumulativeDelta > 1f) {
            inputManager.Update();
            Update();
            cumulativeDelta -= 1f;
            if (cumulativeDelta >= 1f)
                cumulativeDelta = 0f;
        }
        framesSinceStart += 1;
        DrawUpdate(cumulativeDelta);
    }

    public virtual void Update()
    {
    }

    public virtual void DrawUpdate(float frameProgress)
    {
    }

    public virtual void ShutDownProcess()
    {
    }

    public virtual void CommunicateWithUpcomingProcess(MainLoopProcess nextProcess)
    {
    }
}
