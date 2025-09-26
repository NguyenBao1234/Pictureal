using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EnhancedButtonEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnPressed;
    public UnityEvent OnReleased;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pressed");
        OnPressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Released");
        OnReleased?.Invoke();
    }
}
