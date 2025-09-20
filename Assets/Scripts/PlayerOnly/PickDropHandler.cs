using UnityEngine;
using UnityEngine.InputSystem;

public class PickDropHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;

    [Header("Settings")]
    [SerializeField] private float interactRange = 3f;

    private PlayerInput playerInput;
    private InputAction interactAction;
    private ObjectGrabbable objectGrabbable;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"];
    }

    private void OnEnable()
    {
        interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (objectGrabbable == null)
            TryGrab();
        else
            Drop();
    }

    private void TryGrab()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, pickupLayerMask))
        {
            if (hit.transform.TryGetComponent(out ObjectGrabbable grabObj))
            {
                objectGrabbable = grabObj;
                objectGrabbable.Grab(objectGrabPointTransform);

                Debug.Log($"Grabbed: {hit.collider.name}");

            }
        }
    }

    private void Drop()
    {
        if (objectGrabbable == null) return;

        objectGrabbable.Drop();

        Debug.Log($"Dropped: {objectGrabbable.name}");

        objectGrabbable = null;
    }
}
