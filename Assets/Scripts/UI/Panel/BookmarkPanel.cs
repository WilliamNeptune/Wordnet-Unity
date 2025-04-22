using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Diagnostics.Tracing;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;
using Data = SynonymData;
using TMPro;
using UnityEngine.EventSystems;

public class BookmarkPanel : ParentPanel
{
    public static BookmarkPanel instance;
    [SerializeField] private ScrollRect scrollView;
    //[SerializeField] private GameObject widgetPoolGameObject;
    private Dictionary<int, GameObject> bookmarkObjectsDict = new Dictionary<int, GameObject>();
    // [SerializeField] private Button btnDefaultList;

    private float interval = 185f;
    private int count = 0;
    void Start()
    {
        base.Start();
        instance = this;
    }
    void OnEnable()
    {
        ClearBookmarkObjects();
        ServerManager.instance.OnBookmarkPagesReceived += ResponseBookmarksContent;
        ServerManager.instance.TryToGetAllBookmarks();
    }
    void OnDisable()
    {
        ServerManager.instance.OnBookmarkPagesReceived -= ResponseBookmarksContent;
    }

    public void ClearBookmarkObjects()
    {
        foreach (Transform child in scrollView.content)
        {
            Destroy(child.gameObject);
        }
        bookmarkObjectsDict.Clear();
    }
    public void ResponseBookmarksContent(bool settingNewBookmark = false)
    {
        ClearBookmarkObjects();
        Bookmark bookmark = Bookmark.LoadBookmarkFromPlayerPrefs();
        Dictionary<int, string> bookmarkNames = bookmark.GetAllBookmarks();
        count = bookmarkNames.Count;
        Debug.Log("count == " + count);

        int interval = 185;
        int deleteButtonWidth = 300;
        float width = this.GetComponent<RectTransform>().rect.width;
        int index = 0;

        foreach (var item in bookmarkNames.OrderBy(x => x.Key == 0 ? -1 : x.Key))
        {
            int id = item.Key;
            Vector2 anchoredPosition = new Vector2(0, -(interval * index));
            CreateBookmarkItem(id, item.Value, index);
            var bookmarkObj = bookmarkObjectsDict[id];
            bookmarkObj.transform.GetChild(0).GetComponent<ScrollRect>().content.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(width + deleteButtonWidth, 0);
            index++;
        }

        scrollView.content.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, interval * index);

        if (settingNewBookmark)
        {
            int maxId = bookmarkObjectsDict.Keys.Max(); //which is the new added bookmark id
            ChangeBookmarkName(maxId);
        }
    }

    private void CreateBookmarkItem(int id, string name, int index)
    {
        Vector2 anchoredPosition = new Vector2(0, -(interval * index));
        var bookmarkObj = CreateGameObject.createUIObject(scrollView.content.transform, anchoredPosition, CreateGameObject.pathBookmarkShowEntity);
        bookmarkObjectsDict.Add(id, bookmarkObj);

        var itemScrollRect = bookmarkObj.transform.GetChild(0).GetComponent<ScrollRect>();
        var itemButton = itemScrollRect.GetComponent<Button>();

        // Set up scroll tracking for this item
        bool isSliding = false;

        // Add event handlers
        EventTrigger trigger = itemScrollRect.gameObject.AddComponent<EventTrigger>();

        // Begin Drag
        EventTrigger.Entry beginDrag = new EventTrigger.Entry();
        beginDrag.eventID = EventTriggerType.BeginDrag;
        beginDrag.callback.AddListener((data) => { isSliding = true; });
        trigger.triggers.Add(beginDrag);

        // End Drag
        EventTrigger.Entry endDrag = new EventTrigger.Entry();
        endDrag.eventID = EventTriggerType.EndDrag;
        endDrag.callback.AddListener((data) =>
        {
            StartCoroutine(ResetSlideFlag());
        });
        trigger.triggers.Add(endDrag);

        IEnumerator ResetSlideFlag()
        {
            yield return new WaitForSeconds(0.1f);
            isSliding = false;
        }
        itemScrollRect.content.transform.Find("name").gameObject.SetActive(true);
        // Set up the text and button listeners
        itemScrollRect.content.transform.Find("name").GetComponent<Text>().text = name;

        if (id != 0)
        {
            itemScrollRect.content.transform.Find("name").GetComponent<LongClickButton>().onLongClick.AddListener(() =>
            {
                ChangeBookmarkName(id);
            });

            itemScrollRect.content.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                ServerManager.instance.TryToDeleteSpecificBookmark(id);
            });
        }

        // Modified click listener to check sliding state
        itemButton.onClick.AddListener(() =>
        {
            if (!isSliding)  // Only proceed if not sliding
            {
                var panel = PanelManager.CreatePanel(ContainerPanel.instance.transform, "BookmarkShowPanel", new Vector2(1171, 0), true);
                panel.GetComponent<BookmarkShowPanel>().Initialize(id);
                PanelManager.openPanelHorizontally(panel, null, 0);
                ContainerPanel.instance.SetBtnUpRightBtnVisible(false);

                ContainerPanel.instance.SetTitle(name, false);

            }
        });
    }
    private void ChangeBookmarkName(int id)
    {
        var bookmarkObj = bookmarkObjectsDict[id];
        bookmarkObj.transform.GetChild(0).GetComponent<ScrollRect>().content.transform.Find("name").gameObject.SetActive(false);
        var inputfield = bookmarkObj.transform.GetChild(0).GetComponent<ScrollRect>().content.transform.Find("inputfield");
        inputfield.gameObject.SetActive(true);
        inputfield.GetComponent<TMP_InputField>().onEndEdit.AddListener((string name) =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Application.isMobilePlatform)
            {
                ServerManager.instance.TryToChangeBookmarkName(id, name);
            }
        });
    }

    public void AddNewBookmarkPage()
    {
        ServerManager.instance.TryToAddNewBookmark("New Bookmark");
    }
}
