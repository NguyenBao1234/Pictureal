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

        // --- Rotation follow velocity---
        Vector3 direction = (TargetTransform.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * MoveSpeed);
        }

        // --- moving---
        transform.position = Vector3.MoveTowards(
            transform.position,
            TargetTransform.position,
            MoveSpeed * Time.deltaTime
        );

        // --- at goal  ---
        if (Vector3.Distance(transform.position, TargetTransform.position) <= StopDistance)
        {
            bMoving = false;
            OnMoveComplete?.Invoke();
            animator.SetBool("Moving", false);

            // Correct rotation
            transform.rotation = TargetTransform.rotation;
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
        animator.SetBool("Moving", false);
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
