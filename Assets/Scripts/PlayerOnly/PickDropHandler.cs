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

    private PlayerInput _playerInput;
    private InputAction _interactAction;
    private ObjectGrabbable _objectGrabbable;
    
    private void Awake()
    {
        _playerInput = transform.parent.gameObject.GetComponent<PlayerInput>();
        _interactAction = _playerInput.actions["Interact"];
    }

    private void OnEnable()
    {
        _interactAction.performed += OnInteractGrab;
    }

    private void OnDisable()
    {
        _interactAction.performed -= OnInteractGrab;
    }

    private void OnInteractGrab(InputAction.CallbackContext context)
    {
        if (_objectGrabbable == null)
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
                _objectGrabbable = grabObj;
                _objectGrabbable.Grab(objectGrabPointTransform);

                Debug.Log($"Grabbed: {hit.collider.name}");

            }
        }
    }

    private void Drop()
    {
        if (_objectGrabbable == null) return;

        _objectGrabbable.Drop();

        Debug.Log($"Dropped: {_objectGrabbable.name}");

        _objectGrabbable = null;
    }
}