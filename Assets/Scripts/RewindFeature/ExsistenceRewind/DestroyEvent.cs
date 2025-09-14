using UnityEngine;

public class DestroyEvent : RewindableEvent
{
    public DestroyEvent(GameObject inDestroyedGameObject, Vector3 inPosition, Quaternion inRotation)
    {
        GameObjectTarget = Object.Instantiate(inDestroyedGameObject);
        GameObjectTarget.transform.position = inPosition;
        GameObjectTarget.transform.rotation = inRotation;
        GameObjectTarget.SetActive(false);
    }
    public override void Rewind()
    {
        GameObjectTarget.SetActive(true);
        Debug.Log( "Respawned object: " + GameObjectTarget.name);
        ClearData();
    }

    public override bool IsExpired(float currentTime)
    {
        if (currentTime - OccurredTime > 8)
        {
            if(GameObjectTarget.activeSelf == false) Object.Destroy(GameObjectTarget);
            return true;
        }
        else return false;
    }
}
