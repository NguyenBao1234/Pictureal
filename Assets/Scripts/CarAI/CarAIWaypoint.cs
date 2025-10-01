using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarAIWaypoint : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public Transform[] waypoints;
    public float waypointThreshold = 1f;
    public bool loop = true;

    [Header("Movement")]
    public float speed = 5f;
    [Tooltip("Nếu true sẽ ignore chiều cao (y) khi tính hướng quay, giúp không lật/upside-down khi waypoint cao thấp")]
    public bool lockToGround = true;

    [Header("Visual / Model offset")]
    [Tooltip("Nếu model của bạn 'mặt trước' là -Z thay vì +Z, set =180 để bù. Thông thường để 0.")]
    public float visualYawOffset = 0f;

    [Header("Physics")]
    [Tooltip("Lerp speed khi xoay để mượt")]
    public float rotationLerpSpeed = 5f;

    private Rigidbody rb;
    private int currentWaypointIndex = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // tránh bị xoay do lực vật lý, nhưng MoveRotation vẫn điều khiển được
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 dir = (target.position - transform.position);

        // nếu lockToGround thì bỏ cao độ để tránh xoay nghiêng lên/xuống
        if (lockToGround) dir.y = 0f;

        float dist = dir.magnitude;
        if (dist < 0.001f) return;

        Vector3 forwardDir = dir.normalized;

        // di chuyển vật lý về phía waypoint
        Vector3 newPos = transform.position + forwardDir * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // xoay thân xe để hướng về forwardDir, có bù yaw nếu cần
        Quaternion targetRot = Quaternion.LookRotation(forwardDir, Vector3.up) * Quaternion.Euler(0f, visualYawOffset, 0f);
        Quaternion nextRot = Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * rotationLerpSpeed);
        rb.MoveRotation(nextRot);

        // kiểm tra tới waypoint chưa
        if (dist <= waypointThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loop) currentWaypointIndex = 0;
                else enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 1) return;
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] && waypoints[i + 1])
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
        if (loop && waypoints.Length > 1 && waypoints[0] && waypoints[waypoints.Length - 1])
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
    }
}
