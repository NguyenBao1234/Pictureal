using UnityEngine;

public class PolaroidPickup : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.bPickedPolaroid = true;
        player.GetCameraPolaroid().PolaroidCameraModel.SetActive(true);
        InteractionTouchController touchInteraction = GameObject.FindGameObjectWithTag("TouchInteraction")?.GetComponent<InteractionTouchController>();
        if(touchInteraction) touchInteraction.OnPickUpPolaroid();
        GameObject.Destroy(gameObject);
    }
}
