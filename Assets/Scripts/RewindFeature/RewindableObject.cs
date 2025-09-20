using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Only For player in this game, haha, so dont need Manager for this transform type
public class RewindableObject : MonoBehaviour
{
    List<TransformData> RecordedTransforms = new List<TransformData>();

    //private Rigidbody rb;
    bool bRewinding = false;
    
    //void Start() => rb = GetComponent<Rigidbody>();
    
    void FixedUpdate()
    {
        if (bRewinding) Rewind();
        else Record();
    }
    
    void Record()
    {
        if (RecordedTransforms.Count > (8 / Time.fixedDeltaTime)) RecordedTransforms.RemoveAt(RecordedTransforms.Count - 1);
        
        var transformData = new TransformData(transform.position, transform.rotation);
        Debug.Log(transformData.rotation);
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null) transformData.rotation.x = playerController.cameraHolder.localRotation.x;
        
        RecordedTransforms.Insert(0, transformData);    
    }
    
    void Rewind()
    {
        if (RecordedTransforms.Count == 0) return;
        var transformData = RecordedTransforms[0];
        transform.position = transformData.position;
        float playerYaw = transformData.rotation.eulerAngles.y;
        if(playerYaw>180) playerYaw -= 360;
        transform.rotation = Quaternion.Euler( 0,playerYaw,0);
        
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            float cameraPitch = transformData.rotation.eulerAngles.x;
            if(cameraPitch > 180)  cameraPitch -= 360;
            playerController.cameraHolder.localRotation = Quaternion.Euler( cameraPitch,0,0);
        }
        RecordedTransforms.RemoveAt(0);
    }

    public void SetRewind(bool bRewind)
    {
        bRewinding = bRewind;
        //rb.isKinematic = bRewind;
    }
    
}

struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public TransformData(Vector3 inPosition, Quaternion inRotation) => (position, rotation) = (inPosition, inRotation);
}
