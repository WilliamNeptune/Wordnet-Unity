using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using DG.Tweening;
using GraphSystem;

public class ContainerPanel : ParentPanel
{
    public static ContainerPanel instance;
    public Button btnDebug;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Button[] btnTabs;
    [SerializeField] private Button btnUpRight;
    [SerializeField] private GameObject btnBottomArea;
    private bool isSynonymComparePanelVisible = false;
    private float animationDuration = 0.5f;
    public Button btnBack;
    public bool isWordnetMode = false;

    private string[] classesname = { "WordnetPanel", "BookmarkPanel", "SettingPanel" };
    private string[] titleText = { "SID_TITLE1", "SID_TITLE2", "SID_TITLE3" };
    public Text title;
    private int tabIndex = -1;

    void Awake()
    {
        instance = this;

    }
    void Start()
    {
        base.Start();

        int n = panels.Length;

        switchTab(0);
        for (int i = 0; i < n; ++i)
        {
            int index = i; // Capture the current value of i
            btnTabs[index].onClick.AddListener(() =>
            {
                switchTab(index);
            });
        }

        btnBack.onClick.AddListener(BackToLastPanel);
        btnUpRight.onClick.AddListener(OnClickTopRightButton);

        SetBtnBackVisible(false);
        btnDebug.onClick.AddListener(OpenDebugPanel);


        ServerManager.instance.OnMessageReceived += ShowMessage;
        Functions.OnColorModeChanged += setBtnIconAppearenceByIndices;
    }


    public void OpenDebugPanel()
    {
        GameObject debugPanel = PanelManager.CreatePanel(transform, "DebugPanel", new Vector2(0, 0), true);
    }

    public void OnClickTopRightButton()
    {
        if (GetCurrentPage() == Define.CurrentPage.Bookmark)
        {
            BookmarkPanel.instance.AddNewBookmarkPage();
            return;
        }

        GameObject risingPanel = PanelManager.CreatePanel(transform, "BookmarkChoosingPanel", new Vector2(0, 0), true);
        PanelManager.OpenPanelVertically(risingPanel, null, 900);
    }



    public void SetBtnUpRightBtnVisible(bool visible)
    {
        btnUpRight.gameObject.SetActive(visible);
    }

    public void SetTitle(string strTitle, bool isLocalize = false)
    {
        if (isLocalize)
        {
            Functions.setTextStr(title, strTitle);
        }
        else
        {
            title.text = strTitle;
        }
    }

    public void switchTab(int index)
    {
        if (tabIndex == index)
            return;
        tabIndex = index;

        SetTitle(titleText[tabIndex], true);

        SetPanelVisByIndex();

        //top right buttion setting
        SetBtnUpRightBtnVisible(tabIndex == 0 && isWordnetMode || (tabIndex == 1 && IAPManager.Instance.IsSubscribed()));
    }

    private void SetPanelVisByIndex()
    {
        int n = panels.Length;
        for (int i = 0; i < n; ++i)
        {
            panels[i].SetActive(i == tabIndex);
            setBtnIconAppearence(i);
        }
    }

    private void setBtnIconAppearenceByIndices()
    {
        int n = btnTabs.Length;
        for (int i = 0; i < n; i++)
        {
            setBtnIconAppearence(i);
        }
    }

    private void setBtnIconAppearence(int i)
    {
        Image tabImage = btnTabs[i].transform.GetChild(0).GetComponent<Image>();

        if (i == tabIndex)
        {
            Vector3 targetScale = new Vector3(1.6f, 1.6f, 1f);
            Vector3 originalScale = new Vector3(1.4f, 1.4f, 1f);
            // Activate the tab and animate it
            tabImage.gameObject.SetActive(true);
            tabImage.transform.DOScale(targetScale, 0.15f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => tabImage.transform.DOScale(originalScale, 0.1f).SetEase(Ease.InOutSine));
        }
        else
        {
            // Deactivate the tab
            tabImage.gameObject.SetActive(false);
            tabImage.transform.localScale = Vector3.one;
        }

        btnTabs[i].transform.GetChild(1).GetComponent<Image>().color = i == tabIndex ?
        Functions.getBackgroundColor() : Functions.getProspectColor();

        btnTabs[i].transform.GetChild(0).GetComponent<Image>().color = i == tabIndex ?
        Functions.getProspectColor() : Functions.getBackgroundColor();
    }
    protected override void SetColorMode()
    {
        base.SetColorMode();
        switchTab(tabIndex); ///???
    }

    public void BackToLastPanel()
    {
        SetTitle(titleText[tabIndex], true);
        if (isWordnetMode)
        {
            ToggleWordnetMode(false);
            return;
        }
        GameObject topPanel = PanelManager.GetTopPanel();
        Debug.Log("BackToLastPanel: " + topPanel.name);
        PanelManager.closePanelHorizontally(topPanel, null, 1171);
    }

    public void ToggleSynonymComparePanel()
    {
        Vector2 visiblePosition = new Vector2(0, 0);
        Vector2 hiddenPosition = new Vector2(0, -1660);
        if (isSynonymComparePanelVisible)
        {
            SynonymComparePanel.instance.transform.GetComponent<RectTransform>().DOAnchorPosY(hiddenPosition.y, 0.1f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                Destroy(SynonymComparePanel.instance.gameObject);
            });
        }
        else
        {
            SynonymComparePanel.instance.transform.GetComponent<RectTransform>().DOAnchorPosY(visiblePosition.y, 0.1f).SetEase(Ease.InOutSine);
        }
        isSynonymComparePanelVisible = !isSynonymComparePanelVisible;
    }

    public void ToggleWordnetMode(bool isWordnetMode)
    {
        if (this.isWordnetMode == isWordnetMode)
            return;
        this.isWordnetMode = isWordnetMode;

        WordnetPanel.instance.AlterAppearenceForWordnetMode(this.isWordnetMode);
        btnBottomArea.transform.DOMoveY(isWordnetMode ? -230f : 0, 0.2f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            SetBtnBackVisible(this.isWordnetMode);
            SetBtnUpRightBtnVisible(this.isWordnetMode);
            WordnetPanel.instance.SetBtnAddWordVisible(this.isWordnetMode);
            if (!this.isWordnetMode)
            {
                GraphManager.instance.DestroyEntity();
                WordnetPanel.instance.ResetIndices();
                WordnetPanel.instance.SetPhoneticVis(false);
                WordnetPanel.instance.SetTipsVisible(false);

            }
            else
            {
                SystemManager.Instance.EnqueueAction(SystemManager.systemRunningTime + 1f, () =>
                {
                    if (GetCurrentPage() != Define.CurrentPage.WordNet)
                        return;
                });
                WordnetPanel.instance.setChoosingAreaPos();
            }

            WordnetPanel.instance.SetInputFieldPos(this.isWordnetMode);
            //WordnetPanel.instance.SetTipsVisible(this.isWordnetMode);
        });
    }

    public Define.CurrentPage GetCurrentPage()
    {
        if (tabIndex == 0)
        {
            if (!isWordnetMode)
                return Define.CurrentPage.WordNetTrending;
            else
                return Define.CurrentPage.WordNet;
        }
        else if (tabIndex == 1)
            return Define.CurrentPage.Bookmark;
        else
            return Define.CurrentPage.Setting;
    }

    public void ShowMessage(string message, bool isLocalize = false, float delay = 1.5f)
    {
        if (message == "NO_MORE_AI_SEARCHING_REMAIN_COUNT")
        {
            if (SynonymComparePanel.instance != null)
            {
                string str = Functions.getTextStr("NO_MORE_AI_SEARCHING_REMAIN_COUNT");
                SynonymComparePanel.instance.DisplayComments(str);
            }
            return;
        }
        PanelManager.CreateMessagePanel(message, isLocalize, delay);
    }

    public void SetBottomAreaVisible(bool visible)
    {
        btnBottomArea.SetActive(visible);
    }

    public void SetBtnBackVisible(bool visible)
    {
        btnBack.gameObject.SetActive(visible);
    }
}
