using UnityEngine;

public class ObjectGrabbable : MonoBehaviour 
{
    private Rigidbody _objectRigidbody;
    private Transform TargetTransform;
    [SerializeField] private float followSpeed = 15f;
    private void Awake()
    {
        _objectRigidbody = GetComponent<Rigidbody>();
        if (_objectRigidbody == null) _objectRigidbody = gameObject.AddComponent<Rigidbody>();
        if(gameObject.layer != LayerMask.NameToLayer("Interactable") ) gameObject.layer = LayerMask.NameToLayer("Interactable");
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
    private void Update()
    {
        if (TargetTransform == null) return;
        Vector3 newPosition = Vector3.Lerp(transform.position, TargetTransform.position, Time.deltaTime * followSpeed);
        _objectRigidbody.MovePosition(newPosition);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TargetTransform == null) return;
        var targetObject = other.gameObject.GetComponent<ObjectNeedItem>();
        if (targetObject == null) return;
        targetObject.InteractByItem(gameObject);
    }
}


