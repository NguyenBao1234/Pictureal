using UnityEngine;

public class TouchObjectDeleter : MonoBehaviour
{
    [SerializeField] private bool bEnableTouchInteraction = false;
    [SerializeField] private string TouchControlName = "TouchControl";
    [SerializeField] private string PauseBtnName = "PauseBtn";

    void Start()
    {
        if (!bEnableTouchInteraction)
        {
            Transform t1 = transform.Find(TouchControlName);
            Transform t2 = transform.Find(PauseBtnName);

            if (t1 != null) Destroy(t1.gameObject);
            if (t2 != null) Destroy(t2.gameObject);
        }
    }
}
