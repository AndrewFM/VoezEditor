using System;

public abstract class MainLoopProcess {
    public MainLoopProcess()
    {
        framesPerSecond = 60;
    }

    public virtual void RawUpdate(float dt)
    {
        myTimeStacker += dt * (float)framesPerSecond;
        if (myTimeStacker > 1f) {
            Update();
            myTimeStacker -= 1f;
            if (myTimeStacker >= 2f) {
                myTimeStacker = 0f;
            }
        }
        framesSinceStart += 1;
        GrafUpdate(myTimeStacker);
    }

    public virtual void Update()
    {
    }

    public virtual void GrafUpdate(float timeStacker)
    {
    }

    public virtual void ShutDownProcess()
    {
    }

    public virtual void CommunicateWithUpcomingProcess(MainLoopProcess nextProcess)
    {
    }

    public int framesPerSecond;
    private float myTimeStacker;
    public int framesSinceStart;
}
