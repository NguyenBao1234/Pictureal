using System.Collections;
using UnityEngine;

public class Door : ObjectNeedItem, IInteractable
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip unlockSound;
    [SerializeField] private AudioClip tryOpenLockedDoorSound;
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private bool bUnlock = true;
    public void SetUnlock(bool bInUnlock)
    {
        bUnlock = bInUnlock;
        if(unlockSound != null) AudioSource.PlayClipAtPoint(unlockSound, transform.position);
        if (!bInUnlock) transform.rotation = closedRot;
    }

    private bool bOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(0, openAngle, 0) * closedRot;
    }

    public void Interact(GameObject interactor)
    {
        if (!bUnlock)
        {
            if(tryOpenLockedDoorSound) AudioSource.PlayClipAtPoint(tryOpenLockedDoorSound, transform.position);
            return;
        }
        bOpen = !bOpen;
        StopAllCoroutines();
        AudioClip interactSound = bOpen ? openSound : closeSound;
        if (interactSound != null) AudioSource.PlayClipAtPoint(interactSound, transform.position);
        StartCoroutine(RotateDoor(bOpen ? openRot : closedRot));
    }

    protected override void InteractionByItem(GameObject ItemFromInteract)
    {
        base.InteractionByItem(ItemFromInteract);
        bUnlock = true;
        if(unlockSound) AudioSource.PlayClipAtPoint(unlockSound, transform.position);
        GameObject.Destroy(ItemFromInteract);
    }

    private IEnumerator RotateDoor(Quaternion targetRot)
    {
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            if(!bUnlock) yield break;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.rotation = targetRot;
    }
}
