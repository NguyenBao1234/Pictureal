using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;                     // Volume
using UnityEngine.Rendering.Universal;           // ColorAdjustments

public class TranspotPile : ObjectNeedItem, IInteractable
{
    [Header("Puzzle")]
    [SerializeField] private Transform targetPointC;
    [SerializeField, Min(0.05f)] private float moveDuration = 2f;

    [Header("Post-processing (URP Volume)")]
    [Tooltip("Gán Global Volume của scene ở đây (hoặc để trống để tự tìm 'Global Volume' theo tên).")]
    [SerializeField] private Volume volume;
    
    [SerializeField, Tooltip("Số độ xoay Hue (độ). 360 = 1 vòng")] 
    private float hueShiftAmount = 360f;

    [Header("Player lookup")]
    [SerializeField] private string playerTag = "Player";

    private GameObject PlayerObject;
    private bool bUnlocked = false;
    private ColorAdjustments _colorAdjustments;
    private CharacterController _cachedCharacterController;

    private void Start()
    {
        // fallback tìm Volume theo tên GameObject nếu không gán trong Inspector
        if (volume == null)
        {
            var go = GameObject.Find("Global Volume");
            if (go != null) volume = go.GetComponent<Volume>();
        }

        // Tạo bản copy profile runtime để tránh sửa profile asset gốc
        if (volume != null && volume.profile != null)
        {
            volume.profile = Instantiate(volume.profile);
            volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);
        }
        else
        {
            Debug.LogWarning($"[{name}] Volume hoặc Profile chưa gán hoặc không có Color Adjustments.");
        }

        PlayerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (targetPointC == null)
            Debug.LogWarning($"[{name}] targetPointC chưa gán. Player sẽ không di chuyển đâu cả.");
    }

    // IInteractable implementation — interactor là object (ví dụ: item A) được truyền từ ObjectGrabbable
    public void Interact(GameObject interactor)
    {
        if (bUnlocked) StartCoroutine(DoPuzzleSequence());
        Debug.Log($"[{name}] Interacting with {interactor.name}.");
    }
    private IEnumerator DoPuzzleSequence()
    {
        
        Transform playerT = PlayerObject.transform;
        _cachedCharacterController = PlayerObject.GetComponent<CharacterController>();
        if (_cachedCharacterController != null) _cachedCharacterController.enabled = false;

        Vector3 startPos = playerT.position;
        Quaternion startRot = playerT.rotation;
        Vector3 targetPos = (targetPointC != null) ? targetPointC.position : startPos;
        Quaternion targetRot = (targetPointC != null) ? targetPointC.rotation : startRot;

        float startHue = 0f;
        if (_colorAdjustments != null) startHue = _colorAdjustments.hueShift.value;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / moveDuration));

            // di chuyển và xoay nhân vật
            playerT.position = Vector3.Lerp(startPos, targetPos, t);
            playerT.rotation = Quaternion.Slerp(startRot, targetRot, t);

            // update hue
            if (_colorAdjustments != null)
            {
                float hue = Mathf.Lerp(startHue, startHue + hueShiftAmount, t);
                _colorAdjustments.hueShift.value = hue;
            }

            yield return null;
        }

        // đảm bảo reset hue
        if (_colorAdjustments != null) _colorAdjustments.hueShift.value = startHue;

        // restore player control
        if (_cachedCharacterController != null) _cachedCharacterController.enabled = true;
        

        Debug.Log($"[{name}] Puzzle sequence finished.");
    }

    protected override void InteractionByItem(GameObject itemFromInteract)
    {
        base.InteractionByItem(itemFromInteract);
        GameObject.Destroy(itemFromInteract);
        Debug.Log(itemFromInteract +" Unlock Pile .");
        bUnlocked = true;
    }
}
