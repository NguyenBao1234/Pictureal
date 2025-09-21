using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public string sceneToLoad = "SampleScene";
    public GameObject mainMenuPanel;
    public SettingsMenuController settingMenuPanel;
    public Animator ScreenAnimator;
    

    public void OnNewGame()
    {
        StartCoroutine(NewGameSequence());
    }

    IEnumerator NewGameSequence()
    {
        mainMenuPanel.GetComponent<Canvas>().enabled = false;
        if(ScreenAnimator) ScreenAnimator.SetTrigger("TakeShot");
        yield return new WaitForSeconds(2);
        if(ScreenAnimator) ScreenAnimator.SetTrigger("FlashIn");
        Debug.Log("FlashIn Execute");
        yield return new WaitForSeconds(1);
        // Load scene
        SceneManager.LoadScene(sceneToLoad);

    }

    // Các hàm khác
    public void OnSetting()
    {
        gameObject.SetActive(false);
        settingMenuPanel.PrevUI = gameObject;
        settingMenuPanel.gameObject.SetActive(true);
    }
    
    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
