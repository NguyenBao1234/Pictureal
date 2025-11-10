using System;
using UnityEngine;

public class BlockingVolume : MonoBehaviour
{
    
    BoxCollider BoxVolume;
    private void Awake()
    {
         BoxVolume = GetComponent<BoxCollider>();
    }

    public void SetBlockingVolume(bool bBlock)
    {
        BoxVolume.gameObject.SetActive(bBlock);
    }

}
