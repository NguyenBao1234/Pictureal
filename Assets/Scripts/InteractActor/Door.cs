using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Audio Settings")]
    public AudioClip openSound;
    public AudioClip closeSound;
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
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
        bOpen = !bOpen;
        StopAllCoroutines();
        AudioClip interactSound = bOpen ? openSound : closeSound;
        if (interactSound != null) AudioSource.PlayClipAtPoint(interactSound, transform.position);
        StartCoroutine(RotateDoor(bOpen ? openRot : closedRot));
    }

    private IEnumerator RotateDoor(Quaternion targetRot)
    {
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.rotation = targetRot;
    }
}
