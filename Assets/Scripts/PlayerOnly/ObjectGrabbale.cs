using UnityEngine;

public class ObjectGrabbable : MonoBehaviour 
{
    private Rigidbody _objectRigidbody;
    private Transform TargetTransform;
    private void Awake()
    {
        _objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.TargetTransform = objectGrabPointTransform;
        _objectRigidbody.useGravity = false;


    }
    public void Drop()
    {
        this.TargetTransform = null;
        _objectRigidbody.useGravity = true;
    }
    private void FixedUpdate()
    {
        if (TargetTransform != null)
        {
            float followSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, TargetTransform.position, Time.fixedDeltaTime * followSpeed);
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
        if (TargetTransform == null) return;
        var targetObject = other.gameObject.GetComponent<ObjectNeedItem>();
        if (targetObject == null) return;
        targetObject.InteractByItem(gameObject);
    }
}


