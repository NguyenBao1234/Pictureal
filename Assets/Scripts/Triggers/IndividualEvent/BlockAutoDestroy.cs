using TMPro;
using UnityEngine;

public class BlockAutoDestroy : MonoBehaviour
{
    [SerializeField]private string Message = "Mình sẽ đợi khoảng 40 giây";
    [SerializeField]private float WaitTime = 39f;
    [SerializeField]private TextMeshProUGUI TextHUD;

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        TextHUD.text = Message;
        Invoke(nameof(DestroySelf), WaitTime);
    }
    void DestroySelf()
    {
        TextHUD.text = "";
        Destroy(gameObject);
    }
}
