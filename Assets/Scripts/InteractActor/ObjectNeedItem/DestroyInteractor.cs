using UnityEngine;

public class DestroyInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip InteractSound;

    public void Interact(GameObject interactor)
    {
        AudioSource.PlayClipAtPoint(InteractSound, transform.position);
        Destroy(gameObject);
    }
}
