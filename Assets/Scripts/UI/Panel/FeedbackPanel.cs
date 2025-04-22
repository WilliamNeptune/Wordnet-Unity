using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedbackPanel : ParentPanel
{

    [SerializeField] Button btnSubmitFed;
    [SerializeField] TMP_InputField inputFed;   
    protected override void Start()
    {
        base.Start();
        
        btnSubmitFed.onClick.AddListener(SubmitFeedback);
        ServerManager.instance.OnFeedbackSent += OnFeedbackSentSuccessfully;
    }

    void OnDestroy()
    {
        btnSubmitFed.onClick.RemoveListener(SubmitFeedback);
        ServerManager.instance.OnFeedbackSent -= OnFeedbackSentSuccessfully;
    }

    public void SubmitFeedback()
    {
        string feedback = inputFed.text;
        if (feedback.Length == 0)
        {
            PanelManager.CreateMessagePanel("WRITING_FEEDBACK", true);
            return;
        }
        ServerManager.instance.TryToSendFeedback(feedback);
    }

    private void OnFeedbackSentSuccessfully()
    {
        ContainerPanel.instance.BackToLastPanel();
        PanelManager.CreateMessagePanel("THANKS_FOR_FEEDBACK", true);
    }

}
