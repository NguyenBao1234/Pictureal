using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PhotoCamera : MonoBehaviour
{
    public FrustumCutHandler frustumCutHandler;

    private Camera PolaroidRenderCam;
    public GameObject photoPrefab;
    public GameObject PolaroidCameraModel;
    [SerializeField] private AudioClip TakeShotSFX;
    private AudioSource TakeShotAudioSource;
    
    private Texture2D capturedTex;
    private PlayerInput playerInput;
    private bool bTakenPhoto;
    public bool IsTakenPhoto() => bTakenPhoto;
    private bool bLookingViaCamera = false;
    public bool  IsLookingViaCamera() => bLookingViaCamera;
    private int remainingFilm = 5;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!TakeShotAudioSource) TakeShotAudioSource = gameObject.AddComponent<AudioSource>();
        TakeShotAudioSource.spatialBlend = 0;
        TakeShotAudioSource.playOnAwake = false;
        PolaroidRenderCam = frustumCutHandler.CameraBinocular;
        PolaroidRenderCam.gameObject.SetActive(false);
        photoPrefab.SetActive(false);//refactoring later
    }

    public void OnAction()
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
                PolaroidCameraModel.gameObject.SetActive(true);
                return;
            }
            PolaroidRenderCam.gameObject.SetActive(true);
            bLookingViaCamera = true;
            photoPrefab.SetActive(true);
            PolaroidCameraModel.SetActive(false);
        }
    }
    
    public void OnSubAction()
    {
        if (bLookingViaCamera)
        {
            photoPrefab.SetActive(false);
            bLookingViaCamera = false;
            PolaroidRenderCam.gameObject.SetActive(false);
            PolaroidCameraModel.gameObject.SetActive(true);
        }
    }
    void CapturePhoto()
    {
        if(TakeShotSFX) TakeShotAudioSource.PlayOneShot(TakeShotSFX);
        if (remainingFilm == 0) return;
        remainingFilm--;
        bTakenPhoto = true;
        
        PolaroidRenderCam.gameObject.SetActive(false);
        bLookingViaCamera = false;
        if (frustumCutHandler)
        {
            frustumCutHandler.Cut(true);
        }
    }
    
    public void AddToRemainingFilm(int inAmount) {remainingFilm += inAmount;}
}
