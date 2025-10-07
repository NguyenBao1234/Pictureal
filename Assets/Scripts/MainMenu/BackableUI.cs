using UnityEngine;

public class BackableUI : MonoBehaviour
{
    public GameObject PrevUI;

    virtual public void OnBack()
    {
        PrevUI.SetActive(true);
        gameObject.SetActive(false);
    }
}
