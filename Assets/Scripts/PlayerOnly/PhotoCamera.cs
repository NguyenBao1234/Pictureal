using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PhotoCamera : MonoBehaviour
{
    public FrustumCutHandler frustumCutHandler;

    public Camera renderCam;       // Camera phụ
    public RenderTexture renderTex; 
    public GameObject photoPrefab; // Prefab cái ảnh (Quad cầm tay)
    
    private Texture2D capturedTex;
    private PlayerInput playerInput;
    private bool bTakenPhoto;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        if(!playerInput) playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Action"].performed += OnAction;
        playerInput.actions["SubAction"].performed += OnSubAction;
    }
    
    void Update()
    {
        
    }

    private void OnAction(InputAction.CallbackContext obj)
    {
        if(!bTakenPhoto) CapturePhoto();
        else
        {
            frustumCutHandler.Cut(false);
            bTakenPhoto = false;
        }
    }
    
    void OnSubAction(InputAction.CallbackContext ctx)
    {
        Debug.Log("Implement SubAction Later");
    }
    void CapturePhoto()
    {
        bTakenPhoto = true;
        // RenderTexture.active = renderTex;
        // renderCam.targetTexture = renderTex;
        //
        // renderCam.Render();
        //
        // capturedTex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGB24, false);
        // capturedTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        // capturedTex.Apply();// copy data calculated in cpu into gpu

        if (frustumCutHandler)
        {
            frustumCutHandler.Cut(true);
        }
        // Tạo 1 tấm ảnh player có thể cầm
        if(photoPrefab == null)  return;
        GameObject photo = Instantiate(photoPrefab);
        photo.GetComponent<Renderer>().material.mainTexture = capturedTex;
    }
    
    private void OnDisable()
    {
        playerInput.actions["Action"].performed -= OnAction;
        playerInput.actions["SubAction"].performed -= OnSubAction;
    }
}
