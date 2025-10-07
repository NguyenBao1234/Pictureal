using UnityEngine;

public class CarNeedEnergy : ObjectNeedItem, IInteractable
{
    private bool bUnlock = false;
    [SerializeField] private Transform TargetTransformCar;
    [SerializeField] private Transform TargetTransformPlayer;

    protected override void InteractionByItem(GameObject ItemFromInteract)
    {
        base.InteractionByItem(ItemFromInteract);
        bUnlock = true;
        Destroy(ItemFromInteract);
    }

    public void Interact(GameObject interactor)
    {
        if (!bUnlock) return;
        gameObject.transform.position =  TargetTransformCar.position;
        bUnlock = false;
        var controller = interactor.GetComponent<PlayerController>();
        var charController = interactor.GetComponent<CharacterController>();
    
        controller.enabled = false;
        if (charController) charController.enabled = false;
    
        interactor.transform.position = TargetTransformPlayer.position;
    
        if (charController) charController.enabled = true;
        controller.enabled = true;
    }
}
