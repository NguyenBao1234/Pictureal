using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    
    [Header("Footstep Settings")]
    public AudioSource FootstepAudioSource;
    public AudioClip[] footstepClips; // mảng tiếng bước chân
    public float walkStepInterval = 0.5f; // khoảng cách thời gian giữa các bước khi đi
    public float runStepInterval = 0.3f;  // khi chạy
    private float stepTimer = 0f;

    [Header("Movement Settings")]
    public Joystick joystickInput;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2.5f;
    public float airControlPercent = 0.5f;
    private Coroutine StopRunRefreshRoutine;

    [Header("Look Settings")]
    public Transform cameraHolder;
    public float lookSpeed = 2f;
    public float lookXLimit = 85f;
    public TouchLook touchLookInput;

    [Header("Crouch Settings")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    [Header("UI")]
    public GameObject rewindUI;
    public GameObject PauseUI;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool bCanMove = true;
    public bool GetCanMove() => bCanMove;
    public void SetCanMove(bool bInCanMove) => bCanMove = bInCanMove;
    private float targetSpeed;

    // input state
    private Vector2 moveInput;
    private float moveFwdFactor;
    private float moveRtFactor ;
    private Vector2 lookInput;
    private bool bCrouching;
    private PlayerInput playerInput;
    private bool bRewinding = false;
    private RewindableObject rwObj;

    [Header("Interaction Settings")]
    public float interactDistance = 2f;    
    public Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f); 
    public LayerMask interactLayer;
    private PhotoCamera CameraPolaroid;
    public bool bPickedPolaroid;
    public PhotoCamera GetCameraPolaroid() => CameraPolaroid;
    private bool bPausing;
    public bool GetPausing() => bPausing;

    public void SetPauseProperty(bool bInPause) => bPausing = bInPause;
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        targetSpeed = walkSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rwObj = GetComponent<RewindableObject>();
        CameraPolaroid = GetComponent<PhotoCamera>();
        if (rewindUI != null) rewindUI.SetActive(false);
        if(!FootstepAudioSource) FootstepAudioSource = gameObject.AddComponent<AudioSource>();
        FootstepAudioSource.spatialBlend = 0;
        FootstepAudioSource.playOnAwake = false;
        if(!bPickedPolaroid) CameraPolaroid.PolaroidCameraModel.SetActive(false);
    }

    private void OnEnable()
    {
        if (!playerInput) playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
        playerInput.actions["Look"].performed += OnLook;
        playerInput.actions["Look"].canceled += OnLook;

        playerInput.actions["Sprint"].performed += OnSprint;
        playerInput.actions["Sprint"].canceled += OnSprint;

        playerInput.actions["Crouch"].performed += OnCrouch;
        playerInput.actions["Crouch"].canceled += OnCrouch;

        playerInput.actions["Jump"].performed += OnJump;
        
        playerInput.actions["Interact"].performed += OnInteract;
        
        playerInput.actions["Action"].performed += OnAction;
        playerInput.actions["SubAction"].performed += OnSubAction;
        
        playerInput.actions["Rewind"].performed += StartRewind;
        playerInput.actions["Rewind"].canceled += StopRewind;
        
        playerInput.actions["Pause"].performed += PauseGame;
        
    }

    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;

        playerInput.actions["Look"].performed -= OnLook;
        playerInput.actions["Look"].canceled -= OnLook;

        playerInput.actions["Sprint"].performed -= OnSprint;
        playerInput.actions["Sprint"].canceled -= OnSprint;

        playerInput.actions["Crouch"].performed -= OnCrouch;
        playerInput.actions["Crouch"].canceled -= OnCrouch;

        playerInput.actions["Jump"].performed -= OnJump;
        
        playerInput.actions["Interact"].performed -= OnInteract;
                
        playerInput.actions["Action"].performed -= OnAction;
        playerInput.actions["SubAction"].performed -= OnSubAction;
        
        playerInput.actions["Rewind"].performed -= StartRewind;
        playerInput.actions["Rewind"].canceled -= StopRewind;
        
        playerInput.actions["Pause"].performed -= PauseGame;
        
    }

    private void Update()
    {
        if (bPausing) return;
        HandleMovement();
        HandleLook();
        ApplyMovement();
        HandleFootsteps();
    }
    
    // === Input Callbacks ===
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (bPausing) return;
        AttemptInteract();
    }
    
    private void OnAction(InputAction.CallbackContext ctx) => DoAction();
    public void TouchAction() =>DoAction();
    private void DoAction()
    {
        if (bPausing) return;
        if(bPickedPolaroid) CameraPolaroid.OnAction();
    }

    private void OnSubAction(InputAction.CallbackContext ctx) => DoSubAction();
    public void TouchSubAction() =>DoSubAction();
    private void DoSubAction()
    {
        if (bPausing) return;
        if(bPickedPolaroid) CameraPolaroid.OnSubAction();
    }
    private void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    private void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();

    private void OnSprint(InputAction.CallbackContext ctx)=> DoSprint();
    public void TouchSprint() => DoSprint(); 
    private void DoSprint()
    {
        if (bPausing) return;
        if (bCrouching) return;
        targetSpeed = runSpeed;
        if (StopRunRefreshRoutine != null) StopCoroutine(StopRunRefreshRoutine);
        StartCoroutine(StopRunSpeedRefresh());
        Debug.Log("Sprint!");
    }
    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        if (bPausing) return;
        if (ctx.performed)
        {
            bCrouching = true;
            targetSpeed = crouchSpeed;
            characterController.height = crouchHeight;
        }
        else
        {
            bCrouching = false;
            targetSpeed = walkSpeed;
            characterController.height = defaultHeight;
        }
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)ExecuteJump();
    }
    public void TouchJump() => ExecuteJump();
    private void ExecuteJump()
    {
        if (bPausing) return;
        if (characterController.isGrounded && bCanMove)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    // --- Logic di chuyển ---
    private void HandleMovement()
    {
        if (!bCanMove || bRewinding) return;

        moveFwdFactor = joystickInput ? Math.Clamp(moveInput.y + joystickInput.Vertical, -1, 1 ) : moveInput.y;
        moveRtFactor = joystickInput ? Math.Clamp(moveInput.x + joystickInput.Horizontal, -1, 1 ) : moveInput.x;
        
        Vector3 inputDir = (transform.forward * moveFwdFactor + transform.right * moveRtFactor).normalized;

        if (characterController.isGrounded)
        {
            velocity.x = inputDir.x * targetSpeed;
            velocity.z = inputDir.z * targetSpeed;

            if (velocity.y < 0f) velocity.y = -2f;
        }
        else
        {
            // quán tính trên không
            velocity.x = Mathf.Lerp(velocity.x, inputDir.x * targetSpeed, airControlPercent * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, inputDir.z * targetSpeed, airControlPercent * Time.deltaTime);

            // gravity
            if (velocity.y > 0) velocity.y += gravity * Time.deltaTime;
            else velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
    }
    private IEnumerator StopRunSpeedRefresh()
    {
        while (targetSpeed == runSpeed)
        {
            yield return new WaitForSeconds(1f);
            if (characterController.velocity.magnitude < 0.1f)
            {
                targetSpeed = bCrouching ? crouchSpeed : walkSpeed;
                break;
            }
        }
        StopRunRefreshRoutine = null;
    }
    
    private void StartRewind(InputAction.CallbackContext obj) => SetRewind(true);
    private void StopRewind(InputAction.CallbackContext obj) => SetRewind(false);
    public void TouchStartRewind() => SetRewind(true);  
    public void TouchStopRewind() => SetRewind(false);
    private void SetRewind(bool bRewind)
    {
        if (bPausing) return;
        if (rwObj == null) return;
        rwObj.SetRewind(bRewind);
        bRewinding = bRewind;
        TimeRWManager.GetInst().SetRewind(bRewind);
        if (rewindUI != null) rewindUI.SetActive(bRewind);
    }

    
    private void PauseGame(InputAction.CallbackContext ctx) => DoPauseGame();
    public void TouchPauseGame() => DoPauseGame();

    private void DoPauseGame()
    {
        bPausing = !bPausing;
        SetPauseGame(bPausing);
    }
    public void SetPauseGame(bool bInPause)
    {
        bPausing = bInPause;
        Cursor.lockState = bInPause ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = bInPause;
        PauseUI.SetActive(bInPause);
        joystickInput.transform.parent.gameObject.SetActive(!bInPause);
    }
    private void ApplyMovement()
    {
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (bRewinding) return;
        float pitchCamera = cameraHolder.localRotation.eulerAngles.x;
        if(pitchCamera > 180) pitchCamera -= 360;
        
        float deltaLookPitch = touchLookInput? (lookInput.y + touchLookInput.GetInput().y) : lookInput.y;
        float deltaLookYaw = touchLookInput? (lookInput.x + touchLookInput.GetInput().x) : lookInput.x;
        
        pitchCamera += -deltaLookPitch * lookSpeed * 0.1f;
        pitchCamera = Mathf.Clamp(pitchCamera, -lookXLimit, lookXLimit);
        cameraHolder.localRotation = Quaternion.Euler(pitchCamera, 0, 0);
        transform.rotation *= Quaternion.Euler(0, deltaLookYaw * lookSpeed * 0.1f, 0);
    }

    public void AttemptInteract()
    {
        Vector3 center = cameraHolder.position + cameraHolder.forward * interactDistance;
        Collider[] hits = Physics.OverlapBox(center, boxHalfExtents, cameraHolder.rotation, interactLayer);
        foreach (var hit in hits)
        {
            // check if object implement IInteractable
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log(hit.name);
                interactable.Interact(gameObject);
                break;
            }
        }
    }
    
    // Debug hiển thị box trong Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = cameraHolder.position + cameraHolder.forward * interactDistance;
        Gizmos.matrix = Matrix4x4.TRS(center, cameraHolder.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }

    private bool isMoving = false;

    private void HandleFootsteps()
{
    if (!characterController.isGrounded)
    {
        stepTimer = (targetSpeed == runSpeed) ? runStepInterval : walkStepInterval;
        isMoving = false;
        return;
    }

    Vector2 horizontalVelocity = new Vector2(moveFwdFactor, moveRtFactor);
    float speed = horizontalVelocity.magnitude;

    // nếu đứng yên → reset timer về interval, không cho kêu tiếp
    if (speed < 0.1f )
    {
        stepTimer = (targetSpeed == runSpeed) ? runStepInterval : walkStepInterval;
        isMoving = false;
        return;
    }

    // nếu vừa bắt đầu di chuyển
    if (!isMoving)
    {
        PlayFootstep();
        isMoving = true;
        stepTimer = (targetSpeed == runSpeed) ? runStepInterval : walkStepInterval;
        return;
    }

    // bình thường khi đang di chuyển
    float interval = (targetSpeed == runSpeed) ? runStepInterval : walkStepInterval;
    stepTimer -= Time.deltaTime;

    if (stepTimer <= 0f)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 2f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                PlayFootstep();
            }
        }

        stepTimer = interval;
    }
}



    private int lastIndex = -1;

    private void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        int index;
        do
        {
            index = Random.Range(0, footstepClips.Length);
        } while (index == lastIndex && footstepClips.Length > 1);

        lastIndex = index;
        FootstepAudioSource.volume = Random.Range(0.7f, 1);
        FootstepAudioSource.pitch = Random.Range(0.7f, 1.3f);
        FootstepAudioSource.PlayOneShot(footstepClips[index]);
    }
}

