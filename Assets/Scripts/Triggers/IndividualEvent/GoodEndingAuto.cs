using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoodEndingAuto : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI textUI;
    [SerializeField]private Animator Screen;
    [SerializeField]private string Message = "Welcome back";
    void Start()
    {
        textUI.gameObject.SetActive(true);
        textUI.text = Message;
        StartCoroutine(DelaybackToMainMenu());
    }

    private IEnumerator DelaybackToMainMenu()
    {
        yield return new WaitForSeconds(30);
        Screen.SetTrigger("FlashIn");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("MainMenuScene");
    }
}
