using UnityEngine;

public class ObjectNeedItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string requiredItemName; // tên item cần thiết để tương tác

    public void Interact(GameObject interactor)
    {
        Debug.Log($"Item1 got interaction from: {interactor.name}");
        
        if (interactor.name == requiredItemName)
        {
            Debug.Log("Correct item! Item1 reacts.");
        }
        else
        {
            Debug.Log("Wrong item. Need: " + requiredItemName);
        }
    }
}

