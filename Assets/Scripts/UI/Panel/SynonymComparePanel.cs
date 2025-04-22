using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SynonymComparePanel : MonoBehaviour
{
    public static SynonymComparePanel instance{get; private set;}
    [SerializeField] private ScrollRect commentsScrollRect;
    [SerializeField] private Button closeButton;    
    [SerializeField] private Text titleText;
    [SerializeField] private Image loadingImage;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void OnEnable()
    {
        closeButton.onClick.AddListener(() => {
            PanelManager.ClosePanelVertically (this.gameObject, null, -1660);
        });
        loadingImage.gameObject.SetActive(true);
    }
    public void init(string word1, string word2)
    {
        DisplayTitle(word1 + "   vs   " + word2);
    }
    public void DisplayComments(string comments)
    {
        loadingImage.gameObject.SetActive(false);
        Text textComponent = commentsScrollRect.content.GetComponent<Text>();
        textComponent.text = comments;


        Functions.DoFadeIn(textComponent, 1f);
        
        RectTransform rectTransform = commentsScrollRect.content.GetComponent<RectTransform>();
        ContentSizeFitter contentFitter = commentsScrollRect.content.GetComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        // Let ContentSizeFitter calculate the size first
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);


        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.y += 300f;  // Add 200 units to height
        contentFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        // Set the new size to the rectTransform    
        rectTransform.sizeDelta = sizeDelta;
        Debug.Log("sizeDelta.y: " + sizeDelta.y);
   
    }
    public void DisplayTitle(string title)
    {
        Text textComponent = transform.Find("Title").GetComponent<Text>();
        textComponent.text = title;
    }
}
