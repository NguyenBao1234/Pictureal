using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TouchLook : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector2 input = Vector2.zero;
    public Vector2 GetInput(){return input;}
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        input = eventData.delta;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
    }
}
