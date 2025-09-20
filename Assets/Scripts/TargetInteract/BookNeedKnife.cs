using UnityEngine;

public class BookNeedKnief : MonoBehaviour, IInteractable
{
    [SerializeField] private string requiredItemName; // tên item cần thiết để tương tác

    public void Interact(GameObject interactor)
    {
        Debug.Log($"Book got interaction from: {interactor.name}");

        if (interactor.name == requiredItemName)
        {
            Debug.Log("Correct item! Book reacts.");
        }
        else
        {
            Debug.Log("Wrong item. Need: " + requiredItemName);
        }
    }
}
