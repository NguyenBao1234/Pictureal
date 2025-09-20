using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public  SettingsMenuController settingMenuPanel;

    public void ContinueClicked()
    {
        gameObject.SetActive(false);
        PlayerController Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Player.bPausing =  false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void OpenSetting()
    {
        gameObject.SetActive(false);
        settingMenuPanel.PrevUI = gameObject;
        settingMenuPanel.gameObject.SetActive(true);
    }

    public void MainMenuClicked()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
