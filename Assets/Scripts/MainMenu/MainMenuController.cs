using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingMenuPanel;
    public Image flashImage;   // flash nháy đen ngay lập tức
    public Image fadeImage;    // fade sang đen từ từ

    [Header("Timings")]
    public float flashHold = 0.1f;        // thời gian giữ flash đen
    public float delayBeforeFade = 1.0f;  // GIỮ 1 GIÂY sau flash trước khi fade
    public float fadeDuration = 1.0f;     // thời gian fade sang đen
    public float blackHold = 0.5f;        // giữ màn hình đen trước khi load scene

    public string sceneToLoad = "SampleScene";

    public void OnNewGame()
    {
        StartCoroutine(NewGameSequence());
    }

    IEnumerator NewGameSequence()
{
     // --- ẨN TOÀN BỘ MENU NGAY LẬP TỨC ---
    mainMenuPanel.SetActive(false);

    // FLASH effect (chớp đen nhanh)
flashImage.gameObject.SetActive(true);
flashImage.color = new Color(0, 0, 0, 1);  // đen bật lên ngay lập tức

yield return new WaitForSeconds(flashHold); // giữ đen trong tích tắc (ví dụ 0.1s)

flashImage.color = new Color(0, 0, 0, 0); // tắt ngay lập tức

// GIỮ MÀN HÌNH BÌNH THƯỜNG 1s TRƯỚC KHI FADE
yield return new WaitForSeconds(1f);

// bắt đầu fade sang đen từ từ
fadeImage.gameObject.SetActive(true);
float t = 0;
while (t < fadeDuration)
{
    t += Time.deltaTime;
    float alpha = Mathf.Clamp01(t / fadeDuration);
    fadeImage.color = new Color(0, 0, 0, alpha);
    yield return null;
}

// giữ đen thêm 0.5s
yield return new WaitForSeconds(blackHold);

// Load scene
SceneManager.LoadScene(sceneToLoad);

}

    // Các hàm khác
    public void OnSetting()
    {
        mainMenuPanel.SetActive(false);
        settingMenuPanel.SetActive(true);
    }

    public void OnBack()
    {
        settingMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
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
