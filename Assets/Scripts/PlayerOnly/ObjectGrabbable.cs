using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;


    }
    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
    }
    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float followSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.fixedDeltaTime * followSpeed);
            objectRigidbody.MovePosition(newPosition);
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
        if (objectGrabPointTransform == null) return;

        // chỉ tương tác với object có IInteractable
        if (other.TryGetComponent<IInteractable>(out var target))
        {
            Debug.Log($"{gameObject.name} is trying to interact with {other.name}");
            target.Interact(gameObject); // truyền chính item này (Knife, Key, ...)
        }
    }
}

