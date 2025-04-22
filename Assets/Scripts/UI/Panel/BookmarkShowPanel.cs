using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using Data = SynonymData;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.EventSystems;
using System.Collections;
public class BookmarkShowPanel:ParentPanel
{
    [SerializeField] private ScrollRect scrollView;
    private int bookmarkPageId;
    private int interval = 125;
    private int deleteButtonWidth = 240;
    void Start()
    {
        base.Start();
        OnClosePanel += () => {
            ContainerPanel.instance.SetBtnUpRightBtnVisible(true);
        };
    }

    void OnEnable()
    {
        ServerManager.instance.OnBookmarkReceived += ResponseBookmarksContent;
        
    }
    void OnDestroy()  //it must be called when the panel is destroyed
    {
        ServerManager.instance.OnBookmarkReceived -= ResponseBookmarksContent;
    }

    public void Initialize(int bookmarkPageId)
    {
        this.bookmarkPageId = bookmarkPageId;
        ServerManager.instance.TryToGetLemmasByBookmarkId(bookmarkPageId);
    }

    public void ResponseBookmarksContent()
    {
        if(scrollView.content.transform.childCount > 0)
        {
            foreach (Transform child in scrollView.content)
            {
                Destroy(child.gameObject);
            }
        }
        Bookmark bookmark = Bookmark.LoadBookmarkFromPlayerPrefs();
        List<string> lemmas = bookmark.GerAllLemmasInBookmark(bookmarkPageId);
        int count = lemmas.Count;


        float width = this.GetComponent<RectTransform>().rect.width;
        for (int i = 0; i < count; i++)
        {
            createBookmarkObject(i, lemmas[i], width);
        }

        scrollView.content.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, interval * count);
    }

    private void createBookmarkObject(int index, string lemma, float width)
    {
        Vector2 anchoredPosition = new Vector2(0, -(interval * index));
        var bookmarkObj = CreateGameObject.createUIObject(scrollView.content.transform, anchoredPosition, CreateGameObject.pathPrefabBookmarkEntity);
        
        if (bookmarkObj != null)
        {
            var itemScrollRect = bookmarkObj.transform.GetChild(0).GetComponent<ScrollRect>();
            var itemButton = itemScrollRect.GetComponent<Button>();
            var textComponent = itemScrollRect.content.transform.GetChild(0).GetComponent<Text>();
            var deleteButton = itemScrollRect.content.transform.GetChild(2).GetComponent<Button>();
            
            // Set up scroll tracking
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
            endDrag.callback.AddListener((data) => { 
                StartCoroutine(ResetSlideFlag());
            });
            trigger.triggers.Add(endDrag);

             IEnumerator ResetSlideFlag()
            {
                yield return new WaitForSeconds(0.1f);
                isSliding = false;
            }
            // Set text and button listeners
            textComponent.text = lemma;
            deleteButton.onClick.AddListener(() => {
                ServerManager.instance.TryToDeleteALemmaInBookmark(bookmarkPageId, lemma);
            });

            itemButton.onClick.AddListener(() => {
                if (!isSliding)  // Only proceed if not sliding
                {
                    OnClickBookmark(lemma);
                }
            });

            // Set content size
            itemScrollRect.content.transform.GetComponent<RectTransform>().sizeDelta = 
                new Vector2(width + deleteButtonWidth, 0);
        }
    }



    private void OnClickBookmark(string bookmark)
    {
        Debug.Log("OnClickBookmark: " + bookmark);
        closePanel();
        ContainerPanel.instance.switchTab(0);
        ServerManager.instance.TryToGetSynsets(bookmark);
    }

}