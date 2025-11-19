using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ShadowResolution = UnityEngine.ShadowResolution;

public class SettingsMenuController : BackableUI
{
    [Header("FPS Settings")]
    public TextMeshProUGUI fpsValueLabel;    // Ở giữa (giá trị cap chọn)
    private int[] fpsOptions = { 30, 60, 90, 120 };
    private int currentFpsIndex = 1; // mặc định 60

    [Header("Shadow Settings")]
    public TextMeshProUGUI shadowValueLabel;
    private string[] shadowOptions = { "Low", "Medium", "High" };
    private int currentShadowIndex = 1;

    [Header("Overall Quality")]
    public TextMeshProUGUI overallValueLabel;
    private string[] overallOptions = { "Low", "Medium", "High", "Ultra" };
    private int currentOverallIndex = 1; // Medium

    [Header("Resolution Settings")]
    public TextMeshProUGUI resolutionValueLabel;
    private string[] resolutionOptions = { "1280x720", "1920x1080", "2560x1440" };
    private int currentResolutionIndex = 1; // mặc định 1080p
    [Header("Screen Mode Settings")]
    public TextMeshProUGUI  screenModeValueLabel;
    

    void OnEnable()
    {
        Application.targetFrameRate = fpsOptions[currentFpsIndex];
        currentOverallIndex = QualitySettings.GetQualityLevel();
        currentShadowIndex = (int)QualitySettings.shadowResolution;
        Debug.Log("Shadow Resolution Index: " + currentShadowIndex);
        GetDeviceResolution();//Setup 
        ApplyQuality(); // áp dụng quality mặc định
        ApplyResolution(); // áp dụng res mặc định
        UpdateLabels();
        UpdateScreenButtonText();
    }

    void Update()
    {

    }
    
    // ================= FPS =================
    public void DecreaseFPS()
    {
        currentFpsIndex = (currentFpsIndex - 1 + fpsOptions.Length) % fpsOptions.Length;
        Application.targetFrameRate = fpsOptions[currentFpsIndex];
        fpsValueLabel.text = fpsOptions[currentFpsIndex].ToString();
    }

    public void IncreaseFPS()
    {
        currentFpsIndex = (currentFpsIndex + 1) % fpsOptions.Length;
        Application.targetFrameRate = fpsOptions[currentFpsIndex];
        fpsValueLabel.text = fpsOptions[currentFpsIndex].ToString();
    }

    // ================= Shadow =================
    public void DecreaseShadow()
    {
        currentShadowIndex = (currentShadowIndex - 1 + shadowOptions.Length) % shadowOptions.Length;
        ApplyShadow();
    }

    public void IncreaseShadow()
    {
        currentShadowIndex = (currentShadowIndex + 1) % shadowOptions.Length;
        ApplyShadow();
    }

    void ApplyShadow()
    {
        shadowValueLabel.text = shadowOptions[currentShadowIndex];
        switch (shadowOptions[currentShadowIndex])
        {
            case "Low": QualitySettings.shadowResolution = ShadowResolution.Low;
                break;
            case "Medium": QualitySettings.shadowResolution = ShadowResolution.Medium;
                break;
            case "High": QualitySettings.shadowResolution = ShadowResolution.High;
                break;
        }
    }

    // ================= Overall Quality =================
    public void DecreaseQuality()
    {
        currentOverallIndex = (currentOverallIndex - 1 + overallOptions.Length) % overallOptions.Length;
        ApplyQuality();
    }

    public void IncreaseQuality()
    {
        currentOverallIndex = (currentOverallIndex + 1) % overallOptions.Length;
        ApplyQuality();
    }

    void ApplyQuality()
    {
        overallValueLabel.text = overallOptions[currentOverallIndex];
        QualitySettings.SetQualityLevel(currentOverallIndex, true);
        Debug.Log(QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

    // ================= Resolution =================
    public void DecreaseResolution()
    {
        currentResolutionIndex = (currentResolutionIndex - 1 + resolutionOptions.Length) % resolutionOptions.Length;
        ApplyResolution();
    }

    public void IncreaseResolution()
    {
        currentResolutionIndex = (currentResolutionIndex + 1) % resolutionOptions.Length;
        ApplyResolution();
    }

    void ApplyResolution()
    {
        string[] parts = resolutionOptions[currentResolutionIndex].Split('x');
        int width = int.Parse(parts[0]);
        int height = int.Parse(parts[1]);
        Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
        resolutionValueLabel.text = resolutionOptions[currentResolutionIndex];
    }

    private void GetDeviceResolution()
    {
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;
        string currentRes = currentWidth + "x" + currentHeight;
        
        bool exists = false;
        for (int i = 0; i < resolutionOptions.Length; i++)
        {
            if (resolutionOptions[i] == currentRes)
            {
                exists = true;
                currentResolutionIndex = i;
                break;
            }
        }

        if (!exists)
        {
            string[] newOptions = new string[resolutionOptions.Length + 1];
            newOptions[0] = currentRes;
            
            for (int i = 0; i < resolutionOptions.Length; i++) newOptions[i + 1] = resolutionOptions[i];
            resolutionOptions = newOptions;
            currentResolutionIndex = 0;
        }
    }
    private void UpdateLabels()
    {
        fpsValueLabel.text = fpsOptions[currentFpsIndex].ToString();
        shadowValueLabel.text = shadowOptions[currentShadowIndex];
        overallValueLabel.text = overallOptions[currentOverallIndex];
        resolutionValueLabel.text = resolutionOptions[currentResolutionIndex];
    }
    
    public void ToggleScreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
        UpdateScreenButtonText();
    }

    private void UpdateScreenButtonText()
    {
        if (Screen.fullScreen) screenModeValueLabel.text = "Full Screen";
        else screenModeValueLabel.text = "Windowed";
    }
}
