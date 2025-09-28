using Unity.VisualScripting;
using UnityEngine;

public class UnlockTrigger : MonoBehaviour
{
    [SerializeField] private Door door;//Polish architect later
    [SerializeField] private bool bUnlock;
    [SerializeField] private bool bDoOnce = true;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.SetUnlock(bUnlock);
            if (bDoOnce) GameObject.Destroy(gameObject);
        }
    }
}
