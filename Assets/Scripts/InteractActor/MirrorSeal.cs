using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MirrorSeal : MonoBehaviour, IInteractable
{
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private Animator ScreenAnimator;
    [SerializeField] private AudioClip SmileSound;
    bool bTriggered = false;
    public void Interact(GameObject interactor)
    {
        Debug.Log(interactor.name);
        if(bTriggered) return;
        if(ScreenAnimator) ScreenAnimator.SetTrigger("BlackIn");
        interactor.GetComponent<PlayerController>().SetCanMove(false);
        StartCoroutine(InteractCoroutine());
    }

    private IEnumerator InteractCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        textUI.gameObject.SetActive(true);
        textUI.text = "Bạn đã phong ấn chiếc máy ảnh \nvà bản thân mình";
        if(SmileSound) AudioSource.PlayClipAtPoint(SmileSound, transform.position);
        yield return new WaitForSeconds(8f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenuScene");
    }
}
