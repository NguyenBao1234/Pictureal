using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsMenuController : MonoBehaviour
{
    [Header("FPS Settings")]
    public TextMeshProUGUI fpsValueLabel;    // Ở giữa (giá trị cap chọn)
    private int[] fpsOptions = { 30, 60, 90, 120 };
    private int currentFpsIndex = 1; // mặc định 60
    private float deltaTime = 0.0f;

    [Header("Shadow Settings")]
    public TextMeshProUGUI shadowValueLabel;
    private string[] shadowOptions = { "Low", "Medium", "High" };
    private int currentShadowIndex = 1;

    [Header("Overall Quality")]
    public TextMeshProUGUI overallValueLabel;
    private string[] overallOptions = { "Low", "Medium", "High", "Ultra" };
    private int currentOverallIndex = 1; // Medium

    // Thêm URP Assets vào đây (kéo thả trong Inspector)
    public UniversalRenderPipelineAsset lowURP;
    public UniversalRenderPipelineAsset mediumURP;
    public UniversalRenderPipelineAsset highURP;
    public UniversalRenderPipelineAsset ultraURP;

    [Header("Resolution Settings")]
    public TextMeshProUGUI resolutionValueLabel;
    private string[] resolutionOptions = { "1280x720", "1920x1080", "2560x1440" };
    private int currentResolutionIndex = 1; // mặc định 1080p

    public GameObject PrevUI;

    void Start()
    {
        Application.targetFrameRate = fpsOptions[currentFpsIndex];
        ApplyQuality(); // áp dụng quality mặc định
        ApplyResolution(); // áp dụng res mặc định
        UpdateLabels();
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

    switch (currentShadowIndex)
    {
        case 0: QualitySettings.shadows = UnityEngine.ShadowQuality.Disable; break;
        case 1: QualitySettings.shadows = UnityEngine.ShadowQuality.HardOnly; break;
        case 2: QualitySettings.shadows = UnityEngine.ShadowQuality.All; break;
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

        UniversalRenderPipelineAsset selected = null;
        switch (currentOverallIndex)
        {
            case 0: selected = lowURP; break;
            case 1: selected = mediumURP; break;
            case 2: selected = highURP; break;
            case 3: selected = ultraURP; break;
        }

        if (selected != null)
        {
            GraphicsSettings.defaultRenderPipeline = selected;
            QualitySettings.renderPipeline = selected;
        }
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
        Screen.SetResolution(width, height, FullScreenMode.Windowed);
        resolutionValueLabel.text = resolutionOptions[currentResolutionIndex];
    }

    private void UpdateLabels()
    {
        fpsValueLabel.text = fpsOptions[currentFpsIndex].ToString();
        shadowValueLabel.text = shadowOptions[currentShadowIndex];
        overallValueLabel.text = overallOptions[currentOverallIndex];
        resolutionValueLabel.text = resolutionOptions[currentResolutionIndex];
    }

    public void OnBack()
    {
        PrevUI.SetActive(true);
        gameObject.SetActive(false);
    }
}
