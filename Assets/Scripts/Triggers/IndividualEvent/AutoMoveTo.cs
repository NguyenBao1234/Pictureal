using UnityEngine;
using UnityEngine.Events;

public class AutoMoveTo : MonoBehaviour
{
    [SerializeField] private Transform TargetTransform;
    [SerializeField] private float MoveSpeed = 3;
    public float StopDistance = 0.1f;
    [SerializeField] private UnityEvent OnMoveComplete;
    [SerializeField] private Animator animator;
    [SerializeField] private bool bMoving = false;

    private void Update()
    {
        if (!bMoving || !TargetTransform) return;
        
        transform.position = Vector3.MoveTowards(transform.position, TargetTransform.position, MoveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, TargetTransform.position) <= StopDistance)
        {
            bMoving = false;
            OnMoveComplete?.Invoke();
        }
    }

    
    public void StartMove()
    {
        if (TargetTransform == null)
        {
            Debug.LogWarning("AutoMoveTo: TargetTransform not exist");
            return;
        }

        bMoving = true;
    }
    
    public void SetTarget(Transform newTarget)
    {
        TargetTransform = newTarget;
    }

    public void StopMove()
    {
        bMoving = false;
    }
}
