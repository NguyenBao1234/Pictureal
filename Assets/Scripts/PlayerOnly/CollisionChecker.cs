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
    public CustomFrustumLocalSpace frustumLocalSpace;
    [HideInInspector]
    public int side;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Cuttable"))
            frustumLocalSpace.AddObjectToCut(other.gameObject, side);
    }
}
