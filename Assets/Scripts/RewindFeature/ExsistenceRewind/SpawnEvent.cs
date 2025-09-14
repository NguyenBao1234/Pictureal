using UnityEngine;

public class SpawnEvent : RewindableEvent
{
    private GameObject spawnedGameObject;

    public SpawnEvent(GameObject spawnedGameObject) => this.spawnedGameObject = spawnedGameObject;
    public override void Rewind()
    {
        if (spawnedGameObject == null) return;
        Object.Destroy(spawnedGameObject);
        spawnedGameObject = null;
    }
}
