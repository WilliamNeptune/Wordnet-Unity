using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public float holdTime = 0.5f;  // Time required to trigger long click
    public UnityEvent onLongClick;  // Event to trigger on long click
    
    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Reset();
    }

    private void Update()
    {
        if (isPointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= holdTime)
            {
                // Execute long click action
                onLongClick?.Invoke();
                Reset();
            }
        }
    }

    private void Reset()
    {
        isPointerDown = false;
        pointerDownTimer = 0f;
    }
}