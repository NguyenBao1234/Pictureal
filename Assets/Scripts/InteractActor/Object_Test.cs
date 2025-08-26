using UnityEngine;

public class Object_Test : MonoBehaviour,  IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("Interactor: " + interactor.name);
    }
}
