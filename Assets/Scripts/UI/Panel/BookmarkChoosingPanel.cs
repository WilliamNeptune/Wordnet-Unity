using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using Data = SynonymData;

public class BookmarkChoosingPanel:ParentPanel
{
    private List<GameObject> bookmarkButtons;
    public ScrollRect scrollView;
    public Button btnClose;
    void OnEnable()
    {
        ServerManager.instance.OnBookmarkPagesReceived += CreateBookmarkButtons;
        ServerManager.instance.OnBookmarkAdded += ClosePanel;
        ServerManager.instance.TryToGetAllBookmarks();
        btnClose.onClick.AddListener(ClosePanel);
    }
    void OnDestroy()
    {
        ServerManager.instance.OnBookmarkPagesReceived -= CreateBookmarkButtons;
        ServerManager.instance.OnBookmarkAdded -= ClosePanel;
    }
    public void CreateBookmarkButtons(bool settingNewBookmark = false)
    {
        bookmarkButtons = new List<GameObject>();
        Bookmark bookmark = Bookmark.LoadBookmarkFromPlayerPrefs();
        Dictionary<int, string> bookmarkNames = bookmark.bookmarkNames;
        int count = bookmarkNames.Count;    
        int interval = 185;
        int index = 0;
        foreach(var item in bookmarkNames.OrderBy(x => x.Key == 0 ? -1 : x.Key))
        {
            int id = item.Key; // Capture the current index in a local variable
            Vector2 anchoredPosition = new Vector2(0, -(interval * index));
            var bookmarkObj = CreateGameObject.createUIObject(scrollView.content.transform, anchoredPosition, CreateGameObject.pathBookmarkChoosingEntity);
            bookmarkButtons.Add(bookmarkObj);
            string name = item.Value;
            bookmarkObj.transform.Find("name").GetComponent<Text>().text = name;

            bookmarkObj.GetComponent<Button>().onClick.AddListener(() => OnClickBookmark(id));
            index++;
        }
        scrollView.content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, index * interval);
    }

    private void OnClickBookmark(int id)
    {
        ServerManager.instance.TryToAddLemmaToBookmark(WordnetPanel.instance.getLemma(), id);
        PanelManager.CreateMessagePanel("BOOKMARK_ADDED", true, 1.5f);
    }
    private void ClosePanel()
    {
        Action action = () => {
            SystemManager.Instance.EnqueueAction(SystemManager.systemRunningTime + 1f, () => {
                if (ContainerPanel.instance.GetCurrentPage() != Define.CurrentPage.WordNet) 
                    return;
            });
        };

        PanelManager.ClosePanelVertically(this.gameObject, action, 0f);
    }   
}
