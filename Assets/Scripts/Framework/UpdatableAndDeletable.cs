using System;

public class UpdatableAndDeletable {
    public bool slatedForDeletetion { get; set; }

    public virtual void Update(bool eu)
    {
        this.evenUpdate = eu;
    }

    public virtual void Destroy()
    {
        this.slatedForDeletetion = true;
    }

    public bool evenUpdate;
}
