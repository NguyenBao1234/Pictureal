using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;   // Thêm thư viện UI

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Footstep Settings")]
    public AudioSource audioSource;
    public AudioClip[] footstepClips; // mảng tiếng bước chân
    public float walkStepInterval = 0.5f; // khoảng cách thời gian giữa các bước khi đi
    public float runStepInterval = 0.3f;  // khi chạy
    private float stepTimer = 0f;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2.5f;
    public float airControlPercent = 0.5f;

    [Header("Look Settings")]
    public Transform cameraHolder;
    public float lookSpeed = 2f;
    public float lookXLimit = 85f;

    [Header("Crouch Settings")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    [Header("Rewind UI")]
    public GameObject rewindUI;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool canMove = true;
    private float targetSpeed;

    // input state
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool bCrouching;
    private PlayerInput playerInput;
    private bool bRewinding = false;
    private RewindableObject rwObj;

    [Header("Interaction Settings")]
    public float interactDistance = 2f;    
    public Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f); 
    public LayerMask interactLayer;            

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        targetSpeed = walkSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rwObj = GetComponent<RewindableObject>();

        if (rewindUI != null) rewindUI.SetActive(false);
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

        playerInput.actions["Interact"].performed += OnInteract;
        playerInput.actions["Jump"].performed += OnJump;
        
        playerInput.actions["Rewind"].performed += StartRewind;
        playerInput.actions["Rewind"].canceled += StopRewind;

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

        playerInput.actions["Interact"].performed -= OnInteract;
        playerInput.actions["Jump"].performed -= OnJump;
        
        playerInput.actions["Rewind"].performed -= StartRewind;
        playerInput.actions["Rewind"].canceled -= StopRewind;
    }

    private void Update()
    {
        HandleMovement();
        ApplyMovement();
        HandleLook();
        HandleFootsteps();

    }

    // === Input Callbacks ===
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        AttemptInteract();
    }
    private void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    private void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (bCrouching) return;
        targetSpeed = ctx.performed ? runSpeed : walkSpeed;
        Debug.Log("Sprint!");
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
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

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && characterController.isGrounded && canMove)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // --- Logic di chuyển ---
    private void HandleMovement()
    {
        if (!canMove || bRewinding) return;
        Vector3 inputDir = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;

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
    
    private void StartRewind(InputAction.CallbackContext obj)
    {
        if (rwObj == null) return;
        rwObj.SetRewind(true);
        bRewinding = true;

        if (rewindUI != null) rewindUI.SetActive(true);
    }

    private void StopRewind(InputAction.CallbackContext obj)
    {
        if (rwObj == null) return;
        rwObj.SetRewind(false);
        bRewinding = false;

        if (rewindUI != null) rewindUI.SetActive(false);
    }


    private void ApplyMovement()
    {
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (bRewinding) return;
        float rotationX = cameraHolder.localRotation.eulerAngles.x;
        if(rotationX > 180) rotationX -= 360;
        rotationX += -lookInput.y * lookSpeed * 0.1f;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        cameraHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed * 0.1f, 0);
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

    Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
    float speed = horizontalVelocity.magnitude;

    // nếu đứng yên → reset timer về interval, không cho kêu tiếp
    if (speed < 0.1f)
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
        audioSource.PlayOneShot(footstepClips[index]);
    }
}

