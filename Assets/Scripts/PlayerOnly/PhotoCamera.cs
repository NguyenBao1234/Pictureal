using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PhotoCamera : MonoBehaviour
{
    public FrustumCutHandler frustumCutHandler;

    private Camera PolaroidRenderCam;       // Camera phụ
    public GameObject photoPrefab; // Prefab cái ảnh (Quad cầm tay)
    
    private Texture2D capturedTex;
    private PlayerInput playerInput;
    private bool bTakenPhoto;
    private bool bLookingViaCamera = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PolaroidRenderCam = frustumCutHandler.CameraBinocular;
        PolaroidRenderCam.gameObject.SetActive(false);
        photoPrefab.SetActive(false);//refactoring later
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
        if (bLookingViaCamera) CapturePhoto();
        else
        {
            //Hold camera to look
            if (bTakenPhoto)
            {
                frustumCutHandler.Cut(false);
                bTakenPhoto = false;
                photoPrefab.SetActive(false);
                return;
            }
            PolaroidRenderCam.gameObject.SetActive(true);
            bLookingViaCamera = true;
            photoPrefab.SetActive(true);
        }
    }
    
    void OnSubAction(InputAction.CallbackContext ctx)
    {
        Debug.Log("Implement SubAction Later");
        if (bLookingViaCamera)
        {
            photoPrefab.SetActive(false);
            bLookingViaCamera = false;
            PolaroidRenderCam.gameObject.SetActive(false);
        }
    }
    void CapturePhoto()
    {
        bTakenPhoto = true;

        PolaroidRenderCam.gameObject.SetActive(false);
        bLookingViaCamera = false;
        if (frustumCutHandler)
        {
            frustumCutHandler.Cut(true);
        }
    }
    
    private void OnDisable()
    {
        playerInput.actions["Action"].performed -= OnAction;
        playerInput.actions["SubAction"].performed -= OnSubAction;
    }
}
