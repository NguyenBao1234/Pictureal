using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;     // panel chứa nút chính
    public GameObject settingMenuPanel;  // panel chứa menu Setting
    public Image flashImage;             // flash chụp ảnh
    public Image fadeImage;              // fade đen

    public void OnNewGame()
    {
        StartCoroutine(NewGameSequence());
    }

    IEnumerator NewGameSequence()
{
    // Vô hiệu nút bấm khi đã start
    var cg = mainMenuPanel.GetComponent<CanvasGroup>();
    if (cg) cg.interactable = false;

    // FLASH effect (camera chụp)
    flashImage.gameObject.SetActive(true);
    flashImage.color = new Color(1, 1, 1, 1);  // bật sáng trắng full ngay lập tức

    yield return new WaitForSeconds(0.05f); // nháy cực nhanh
    flashImage.color = new Color(1, 1, 1, 0); // tắt flash ngay

    // MÀN HÌNH ĐEN NGAY LẬP TỨC
    fadeImage.gameObject.SetActive(true);
    fadeImage.color = new Color(0, 0, 0, 1); // đen full luôn

    // Giữ 1s đen trước khi load
    yield return new WaitForSeconds(1f);

    // Load scene
    SceneManager.LoadScene("SampleScene"); // đổi sang tên scene của bạn
}




    public void OnPrototype()
    {
        Debug.Log("Prototype feature pending...");
    }

    public void OnSetting()
    {
        mainMenuPanel.SetActive(false);
        settingMenuPanel.SetActive(true);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
    public void OnBack()
    {
        settingMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
