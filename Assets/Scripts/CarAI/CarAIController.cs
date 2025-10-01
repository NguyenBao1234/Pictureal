using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class CarAIController : MonoBehaviour
{
    [Header("AI Settings")]
    [Tooltip("Khoảng cách an toàn để phát hiện vật cản")]
    public float safeDistance = 10f;

    [Tooltip("Tốc độ tối đa của xe")]
    public float carSpeed = 5f;

    [Tooltip("Tốc độ giảm khi gần vật cản")]
    public float slowDownSpeed = 2f;

    [Tooltip("Khoảng cách tối thiểu trước khi dừng hẳn")]
    public float stopBuffer = 4f;

    [Header("Audio")]
    [Tooltip("Âm thanh còi xe")]
    public AudioClip hornSound;

    private BoxCollider boxCol;
    private Rigidbody rb;
    private AudioSource audioSource;

    private bool shouldStop = false;
    private float currentSpeed;

    [SerializeField] private float brakeForce = 5f; // lực phanh (càng lớn thì dừng càng nhanh)

    public enum MoveAxis { X, Y, Z }
    public enum MoveDirection { Forward, Backward }

    [Header("Movement Settings")]
    public MoveAxis moveAxis = MoveAxis.Z; // mặc định chạy theo Z
    public MoveDirection moveDirection = MoveDirection.Forward; // mặc định đi xuôi

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation; // tránh xe bị nghiêng

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        currentSpeed = carSpeed;
        boxCol = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        // Xác định hướng di chuyển dựa vào MoveAxis + MoveDirection
        Vector3 moveDir = GetMoveDirection();

        // Raycast từ đầu xe (theo hướng moveDir)
        Vector3 rayOrigin = transform.position + moveDir * (boxCol.bounds.extents.z + 0.5f);

        if (Physics.Raycast(rayOrigin, moveDir, out RaycastHit hit, safeDistance))
        {
            if (hit.transform.CompareTag("Car") ||
                hit.transform.CompareTag("Player") ||
                hit.transform.CompareTag("Obstacle"))
            {
                float distance = hit.distance;

                // Nếu còn xa -> giảm tốc, nếu gần sát -> chuẩn bị dừng
                if (distance > stopBuffer)
                {
                    shouldStop = false;
                    currentSpeed = Mathf.Lerp(currentSpeed, slowDownSpeed, Time.fixedDeltaTime * 2f);
                }
                else
                {
                    shouldStop = true;
                }

                // Bấm còi khi gặp vật cản
                if (hornSound && !audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(hornSound);
                }
            }
            else
            {
                shouldStop = false;
            }
        }
        else
        {
            shouldStop = false;
        }

        // Thực hiện hành động
        if (shouldStop)
            Stop();
        else
            Move(moveDir);
    }

    private Vector3 GetMoveDirection()
    {
        Vector3 dir = Vector3.forward;

        switch (moveAxis)
        {
            case MoveAxis.X: dir = Vector3.right; break;
            case MoveAxis.Y: dir = Vector3.up; break;
            case MoveAxis.Z: dir = Vector3.forward; break;
        }

        // Nếu chọn ngược lại thì đảo hướng
        if (moveDirection == MoveDirection.Backward)
            dir = -dir;

        return dir;
    }

    private void Move(Vector3 moveDir)
    {
        Vector3 targetPos = transform.position + moveDir * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);

        // tăng tốc dần lên lại tốc độ chuẩn
        currentSpeed = Mathf.Lerp(currentSpeed, carSpeed, Time.fixedDeltaTime * 0.5f);
    }

    private void Stop()
    {
        // thay vì về 0 ngay lập tức thì giảm dần
        currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.fixedDeltaTime * brakeForce);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (boxCol == null)
            boxCol = GetComponent<BoxCollider>();

        Vector3 moveDir = GetMoveDirection();
        Vector3 rayOrigin = transform.position + moveDir * (boxCol != null ? boxCol.bounds.extents.z + 0.5f : 0.5f);

        Gizmos.DrawRay(rayOrigin, moveDir * safeDistance);
    }
}
/*
 - safeDistance: Khoảng cách an toàn để phát hiện vật cản.
 - carSpeed: Tốc độ tối đa của xe.
 - slowDownSpeed: Tốc độ giảm khi gần vật cản.
 - stopBuffer: Khoảng cách tối thiểu trước khi dừng hẳn.
 - set hướng trong inspector của Car
*/