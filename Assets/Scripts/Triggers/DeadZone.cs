using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
    [Tooltip("Thời gian chờ trước khi restart (giây)")]
    public float restartDelay = 1f;
    public Animator FlashAnimator;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(FlashAnimator) FlashAnimator.SetTrigger("FlashIn");
            Debug.Log("Player fall into DeadZone! Restart level after " + restartDelay + " s.");
            Invoke(nameof(RestartLevel), restartDelay);
        }
    }
    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
