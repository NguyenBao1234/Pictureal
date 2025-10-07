using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public  SettingsMenuController settingMenuPanel;

    public void ContinueClicked()
    {
        gameObject.SetActive(false);
        PlayerController Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Player.SetPauseGame(false);
    }
    
    public void OpenSetting()
    {
        gameObject.SetActive(false);
        settingMenuPanel.PrevUI = gameObject;
        settingMenuPanel.gameObject.SetActive(true);
    }

    public void MainMenuClicked()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
