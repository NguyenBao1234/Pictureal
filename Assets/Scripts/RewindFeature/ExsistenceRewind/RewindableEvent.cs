using UnityEngine;

public abstract class RewindableEvent
{
    public float? OccurredTime;
    protected GameObject GameObjectTarget;
    public abstract void Rewind();

    public virtual bool IsExpired(float currentTime)
    {
        return currentTime - OccurredTime > 8;
    }

    protected void ClearData()
    {
        OccurredTime = null;
        GameObjectTarget = null;
    }
}
