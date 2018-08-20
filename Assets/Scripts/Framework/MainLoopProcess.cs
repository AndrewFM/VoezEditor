using System;

public abstract class MainLoopProcess {
    public MainLoopProcess()
    {
        framesPerSecond = 60;
    }

    public virtual void RawUpdate(float dt)
    {
        cumulativeDelta += dt * framesPerSecond;
        if (cumulativeDelta > 1f) {
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

    public int framesPerSecond;
    private float cumulativeDelta;
    public int framesSinceStart;
}
