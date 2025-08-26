using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2.5f;
    public float airControlPercent = 0.5f;

    [Header("Look Settings")]
    public float lookSpeed = 2f;
    public float lookXLimit = 85f;

    [Header("Crouch Settings")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private CharacterController characterController;
    private Vector3 velocity;
    private float rotationX = 0;

    private bool canMove = true;
    private float targetSpeed;

    // input state
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool bCrouching;
    private PlayerInput playerInput;
    
    [Header("Interaction Settings")]
    public float interactDistance = 2f;        // khoảng cách quét
    public Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f); // kích thước box
    public LayerMask interactLayer;            // layer cho object có thể tương tác

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        targetSpeed = walkSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void OnEnable()
    {
        if(!playerInput) playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
        playerInput.actions["Look"].performed += OnLook;
        playerInput.actions["Look"].canceled += OnLook;

        playerInput.actions["Sprint"].performed += OnSprint;
        playerInput.actions["Sprint"].canceled  += OnSprint;

        playerInput.actions["Crouch"].performed += OnCrouch;
        playerInput.actions["Crouch"].canceled  += OnCrouch;
        
        playerInput.actions["Interact"].performed += OnInteract;
        playerInput.actions["Jump"].performed += OnJump;
    }

    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled  -= OnMove;

        playerInput.actions["Look"].performed -= OnLook;
        playerInput.actions["Look"].canceled  -= OnLook;

        playerInput.actions["Sprint"].performed -= OnSprint;
        playerInput.actions["Sprint"].canceled  -= OnSprint;

        playerInput.actions["Crouch"].performed -= OnCrouch;
        playerInput.actions["Crouch"].canceled  -= OnCrouch;

        playerInput.actions["Interact"].performed -= OnInteract;
        playerInput.actions["Jump"].performed -= OnJump;
    }
    
    private void Update()
    {
        HandleMovement();
        ApplyMovement();
        HandleLook();
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
        Vector3 inputDir = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;

        if (characterController.isGrounded)
        {
            velocity.x = inputDir.x * targetSpeed;
            velocity.z = inputDir.z * targetSpeed;

            if (velocity.y < 0f)
                velocity.y = -2f; // giữ player dính đất
        }
        else
        {
            // quán tính trên không
            velocity.x = Mathf.Lerp(velocity.x, inputDir.x * targetSpeed, airControlPercent * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, inputDir.z * targetSpeed, airControlPercent * Time.deltaTime);

            // gravity
            if (velocity.y > 0)
                velocity.y += gravity * Time.deltaTime;
            else
                velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (!canMove) return;

        rotationX += -lookInput.y * lookSpeed * 0.1f;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed * 0.1f, 0);
    }
    
    public void AttemptInteract()
    {
        Debug.Log("Attempting interaction");
        // vị trí trung tâm của box (trước mặt playerCamera)
        Vector3 center = playerCamera.transform.position + playerCamera.transform.forward * interactDistance;
        // quét các collider trong box
        Collider[] hits = Physics.OverlapBox(center, boxHalfExtents, playerCamera.transform.rotation, interactLayer);
        foreach (var hit in hits)
        {
            // kiểm tra xem object có implement IInteractable không
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log(hit.name);
                interactable.Interact(gameObject); // gọi hàm Interact
                break; // chỉ tương tác 1 object đầu tiên tìm thấy
            }
        }
    }

    // Debug hiển thị box trong Scene View
    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null) return;
        Gizmos.color = Color.cyan;
        Vector3 center = playerCamera.transform.position + playerCamera.transform.forward * interactDistance;
        Gizmos.matrix = Matrix4x4.TRS(center, playerCamera.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }
}
