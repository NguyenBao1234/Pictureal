using UnityEngine;
using UnityEngine.InputSystem;

public class PhotoCamera : MonoBehaviour
{
    public Camera renderCam;       // Camera phụ
    public RenderTexture renderTex; 
    public GameObject photoPrefab; // Prefab cái ảnh (Quad cầm tay)
    
    private Texture2D capturedTex;
    private PlayerInput playerInput;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        if(!playerInput) playerInput = GetComponent<PlayerInput>();
        playerInput.actions["SubAction"].performed += OnSubAction;
    }

    private void OnDisable()
    {
        playerInput.actions["SubAction"].performed -= OnSubAction;
    }

    void Update()
    {
        
    }

    void OnSubAction(InputAction.CallbackContext ctx)
    {
        CapturePhoto();
    }
    void CapturePhoto()
    {
        RenderTexture.active = renderTex;
        renderCam.targetTexture = renderTex;

        renderCam.Render();

        capturedTex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGB24, false);
        capturedTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        capturedTex.Apply();

        // Tạo 1 tấm ảnh player có thể cầm
        GameObject photo = Instantiate(photoPrefab);
        photo.GetComponent<Renderer>().material.mainTexture = capturedTex;
    }
}
