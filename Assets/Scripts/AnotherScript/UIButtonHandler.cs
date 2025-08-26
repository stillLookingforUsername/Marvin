using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private System.Action _onPointerDown;
    private System.Action _onPointerUp;
    
    public void Setup(System.Action onPointerDown, System.Action onPointerUp)
    {
        _onPointerDown = onPointerDown;
        _onPointerUp = onPointerUp;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _onPointerDown?.Invoke();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _onPointerUp?.Invoke();
    }
}