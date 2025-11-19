using UnityEngine;

public class DestroyCallbackEvent : MonoBehaviour
{
    [SerializeField] private bool bHide = true;
    public void ExecuteDestroyEvent()
    {
        if (bHide) gameObject.SetActive(false);
        else Destroy(gameObject);
    }
}
