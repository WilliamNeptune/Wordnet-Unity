using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : ParentPanel
{
    public Button btnClose;
    public Button btnShowProviderId;
    public Button btnShowProviderInfo;
        void Start()
    {
        btnShowProviderId.onClick.AddListener(OnShowProviderIdButtonClick);
        btnClose.onClick.AddListener(closePanel);
        btnShowProviderInfo = this.transform.Find("btnShowProviderInfo").GetComponent<Button>();
        btnShowProviderInfo.onClick.AddListener(OnShowProviderInfoButtonClick);
    }

    public void OnShowProviderIdButtonClick()
    {
        LoginManager.instance.getProviderID();
    }
    
    public void OnShowProviderInfoButtonClick()
    {
        LoginManager.instance.showProviderInfo();
    }
}
