using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;                     // Volume
using UnityEngine.Rendering.Universal;           // ColorAdjustments

public class TranspotPile : ObjectNeedItem, IInteractable
{
    [Header("Puzzle")]
    [SerializeField] private Transform TransfromTarget;
    [SerializeField, Min(0.05f)] private float moveDuration = 2f;

    [Header("Post-processing (URP Volume)")]
    [SerializeField] private Volume volume;

    [Header("Player lookup")]
    [SerializeField] private string playerTag = "Player";

    private GameObject PlayerObject;
    private bool bUnlocked = false;
    private ColorAdjustments PPColorAdjustments;

    private void Start()
    {
        if (volume != null && volume.profile != null)
        {
            volume.profile = Instantiate(volume.profile);
            volume.profile.TryGet<ColorAdjustments>(out PPColorAdjustments);
        }
        else
        {
            Debug.LogWarning($"[{name}] need PP Volume");
        }

        PlayerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (TransfromTarget == null)
            Debug.LogWarning($"[{name}] need transform target");
    }
    
    public void Interact(GameObject interactor)
    {
        if (bUnlocked) StartCoroutine(DoPuzzleSequence());
    }
    private IEnumerator DoPuzzleSequence()
    {
        
        Transform playerT = PlayerObject.transform;
        PlayerObject.GetComponent<PlayerController>().enabled = false;

        Vector3 startPos = playerT.position;
        Quaternion startRot = playerT.rotation;
        Vector3 targetPos = (TransfromTarget != null) ? TransfromTarget.position : startPos;
        Quaternion targetRot = (TransfromTarget != null) ? TransfromTarget.rotation : startRot;

        float startHue = 0f;
        if (PPColorAdjustments != null) startHue = PPColorAdjustments.hueShift.value;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / moveDuration));
            
            playerT.position = Vector3.Lerp(startPos, targetPos, t);
            playerT.rotation = Quaternion.Slerp(startRot, targetRot, t);
            
            if (PPColorAdjustments != null)
            {
                float hue = startHue + 180f * Mathf.Sin(2f * Mathf.PI * t);
                PPColorAdjustments.hueShift.value = hue;
            }
            yield return null;
        }
        
        if (PPColorAdjustments != null) PPColorAdjustments.hueShift.value = startHue;

        // restore player control
        PlayerObject.GetComponent<PlayerController>().enabled = true;
    }

    protected override void InteractionByItem(GameObject itemFromInteract)
    {
        base.InteractionByItem(itemFromInteract);
        GameObject.Destroy(itemFromInteract);
        Debug.Log(itemFromInteract +" Unlock Pile");
        bUnlocked = true;
    }
}
