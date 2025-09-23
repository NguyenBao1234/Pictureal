using UnityEngine;

public class ObjectGrabbable : MonoBehaviour 
{
    private Rigidbody _objectRigidbody;
    private Transform _objectGrabPointTransform;
    private void Awake()
    {
        _objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this._objectGrabPointTransform = objectGrabPointTransform;
        _objectRigidbody.useGravity = false;


    }
    public void Drop()
    {
        this._objectGrabPointTransform = null;
        _objectRigidbody.useGravity = true;
    }
    private void FixedUpdate()
    {
        if (_objectGrabPointTransform != null)
        {
            float followSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, _objectGrabPointTransform.position, Time.fixedDeltaTime * followSpeed);
            _objectRigidbody.MovePosition(newPosition);
        }
    }
    //public GameObject targetObject  
    //private ObjectNeedItem targetItem
    /*OnTriggerEnter(Collider other) , check other object   == target objject
     if true targetObject.IntertractByItem()
    */
    private void OnTriggerEnter(Collider other)
    {
        // chỉ check khi đang được player cầm
        if (_objectGrabPointTransform == null) return;

        // chỉ tương tác với object có IInteractable
        if (other.TryGetComponent<IInteractable>(out var target))
        {
            Debug.Log($"{gameObject.name} is trying to interact with {other.name}");
            target.Interact(gameObject); // truyền chính item này (Knife, Key, ...)
        }
    }
}


