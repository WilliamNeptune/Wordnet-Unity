using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RecommendationWordWidget : MonoBehaviour
{
     public TextMeshProUGUI text;
    public Button button;

     private void Start()
     {
        button.onClick.AddListener(OnClick);
                    // 5. Add explicit click handler with position check
            // EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
            // if (trigger == null)
            //     trigger = gameObject.AddComponent<EventTrigger>();
                
            // EventTrigger.Entry entry = new EventTrigger.Entry();
            // entry.eventID = EventTriggerType.PointerClick;
            
            // entry.callback.AddListener((eventData) => {
            //     PointerEventData pointerData = (PointerEventData)eventData;
            //     // Check if click is actually on this object
            //     if (RectTransformUtility.RectangleContainsScreenPoint(
            //         gameObject.GetComponent<RectTransform>(),
            //         pointerData.position,
            //         WordnetPanel.instance.GetComponentInParent<Canvas>().worldCamera))
            //     {
            //         WordnetPanel.instance.SubmitSearch(text.text);
            //     }
            // });
            
            // trigger.triggers.Add(entry);
     }

     private void OnClick()
     {
        WordnetPanel.instance.SubmitLemma(text.text);
     }

     public void SetText(string text)
     {
        this.text.text = text;
     }
}
