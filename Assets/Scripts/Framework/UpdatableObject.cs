using System;

public class UpdatableObject {
    public bool readyForDeletion { get; set; }

    public virtual void Update()
    {
    }

    public virtual void Destroy()
    {
        readyForDeletion = true;
    }
}
