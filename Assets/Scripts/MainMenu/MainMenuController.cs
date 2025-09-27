using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public string sceneToLoad = "SampleScene";
    public GameObject mainMenuPanel;
    public SettingsMenuController settingMenuPanel;
    public Animator ScreenAnimator;
    
    private float deltaTime;
    private float startTime;
    
    public TMP_Text fpsText;
    public TMP_Text timerText;
    
    public int targetFPS = 60;
    

    void Start()
    {
        Application.targetFrameRate = targetFPS;
        startTime = Time.time;
        if (fpsText != null)
            fpsText.text = targetFPS + " FPS";
    }

    void Update()
    {
        if (fpsText != null)
        {
            int fpsInt = Mathf.RoundToInt(1f / Time.deltaTime);
            fpsText.text = targetFPS + " FPS";
        }

        
        float elapsed = Time.time - startTime;

        int hours = (int)(elapsed / 3600f);
        int minutes = (int)((elapsed % 3600f) / 60f);
        int seconds = (int)(elapsed % 60f);
        int milliseconds = (int)((elapsed - Mathf.Floor(elapsed)) * 100f);
        
        timerText.text = $"{hours:00}:{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
    
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
        StartCoroutine(QuitSequence());
    }

    private IEnumerator QuitSequence()
    {
        ScreenAnimator.SetTrigger("FlashIn");
        yield return new WaitForSeconds(1);
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
