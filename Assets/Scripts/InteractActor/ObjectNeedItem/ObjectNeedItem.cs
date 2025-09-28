using UnityEngine;

public class ObjectNeedItem : MonoBehaviour
{
    [SerializeField] private string requiredItemName; // tên item cần thiết để tương tác

    public void InteractByItem(GameObject ItemFromInteract)
    {
        if (ItemFromInteract.name == requiredItemName)
        {
            InteractionByItem(ItemFromInteract);
        }
    }

    protected virtual void InteractionByItem(GameObject ItemFromInteract) {}
    
    
}

