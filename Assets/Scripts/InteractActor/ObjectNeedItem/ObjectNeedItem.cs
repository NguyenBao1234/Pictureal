using UnityEngine;
using UnityEngine.Events;

public class ObjectNeedItem : MonoBehaviour
{
    [SerializeField] UnityEvent OnItemInteracted;
    [SerializeField] private string requiredItemName; // tên item cần thiết để tương tác
    public void InteractByItem(GameObject ItemFromInteract)
    {
        if (ItemFromInteract.name == requiredItemName)
        {
            InteractionByItem(ItemFromInteract);
            OnItemInteracted?.Invoke();
        }
    }
    protected virtual void InteractionByItem(GameObject ItemFromInteract) {}
}

