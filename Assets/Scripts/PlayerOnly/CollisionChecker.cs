using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [HideInInspector]
    public FrustumCutHandler frustumCutHandler;
    [HideInInspector]
    public int side;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default")) frustumCutHandler.AddObjectToCut(other.gameObject, side);
    }
}
