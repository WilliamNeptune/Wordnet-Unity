using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;
using GraphSystem;
using System.Runtime.InteropServices;
using System.Reflection;
using DG.Tweening;
using TMPro;
using i5.Toolkit.Core.Experimental.UnityAdapters;
using UnityEngine.EventSystems;
using Data = SynonymData;
using System.Threading;
using CandyCoded.HapticFeedback;
public class WordnetPanel : ParentPanel
{
    public static WordnetPanel instance;
    [SerializeField] private Button[] posButtons; // Array of 5 POS buttons
    [SerializeField] private Text phoneticText;
    [SerializeField] private float circleRadius = 350f;
    [SerializeField] private Text definitionText;
    [SerializeField] private TMP_InputField wordInputField;
    [SerializeField] private Text txtHint;
    [SerializeField] private Transform bookmarks;
    [SerializeField] private ScrollRect contentAutoComplete;
    [SerializeField] private RectTransform inputFieldRectTransform; // Reference to the input field's RectTransform
    [SerializeField] private Button btnAddWord;
    [SerializeField] private GameObject ChoosingArea;
    [SerializeField] private ScrollRect trendingWordsContainer;
    [SerializeField] private GameObject trendingArea;
    [SerializeField] private GameObject searchBackground;
    [SerializeField] private GameObject searchArea;
    [SerializeField] private Text tip;
    private int onceClickTips = 0;
    private List<GameObject> wordTrendingWidgets = new List<GameObject>();
    private int currentPosIndex = -1;
    private int currentSynsetIndex = -1;
    private uint currentSynsetId = 0;
    private List<Vector4> linesToDraw = new List<Vector4>(); // Store lines to draw
    private string lemma;
    private const float AUTO_COMPLETE_DELAY = 0.6f;
    private Coroutine autoCompleteCoroutine;
    private float originalInputFieldY;
    private List<SynonymStruct.TrendingWord> trendingWords = new List<SynonymStruct.TrendingWord>();
    private Dictionary<string, Vector2> wordList = new Dictionary<string, Vector2>();
    private TouchScreenKeyboard keyboard;
    void OnEnable()
    {
#if UNITY_IOS
        // Clear any existing selection
        EventSystem.current.SetSelectedGameObject(null);
#endif
    }
    void Start()
    {
        base.Start();
        instance = this;

        ServerManager.instance.OnSynsetReceived += GenerateWordsNetBySynset;
        ServerManager.instance.OnCommentsReceived += ResponseSynonymsAIComparsion;
        ServerManager.instance.OnTrendingWordsReceived += ResponseTrendingWords;
        ServerManager.instance.OnPhoneticTranscriptionReceived += UpdatePhoneticTranscription;

        wordInputField.onSelect.AddListener(OnInputFieldSelected);
        wordInputField.onValueChanged.AddListener(OnInputValueChanged);
        wordInputField.onSubmit.AddListener(SubmitLemma);
        wordInputField.transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(ExitSearchingMode);

        for (int i = 0; i < posButtons.Length; i++)
        {
            posButtons[i].gameObject.SetActive(false);
        }

        originalInputFieldY = inputFieldRectTransform.anchoredPosition.y;

#if UNITY_IOS
        TouchScreenKeyboard.hideInput = true;
#endif
        ChoosingArea.SetActive(false);
        definitionText.gameObject.SetActive(false);

        btnAddWord.onClick.AddListener(OnClickAddWord);

        ServerManager.instance.TryToGetTrendingSearches();

        AlterSearchingStyles(false);
        SetBtnAddWordVisible(false);
        SetTipsVisible(false);

    }
    void Update()
    {
    }
    #region response from server    

    private void SetPartOfSpeechButtons()
    {
        int[] posTypes = WordNetData.GetAllPosTypes();

        int posCount = posTypes.Length;

        for (int i = 0; i < posButtons.Length; i++)
        {
            bool hasPOS = i < posCount;
            posButtons[i].gameObject.SetActive(hasPOS);

            if (hasPOS)
            {
                int posType = posTypes[i];
                string pos = "";
                switch (posType)
                {
                    case (int)Define.PART_OF_SPEECH.NOUN:
                        pos = "NOUN";
                        break;
                    case (int)Define.PART_OF_SPEECH.VERB:
                        pos = "VERB";
                        break;
                    case (int)Define.PART_OF_SPEECH.ADJECTIVE:
                        pos = "ADJECTIVE";
                        break;
                    case (int)Define.PART_OF_SPEECH.ADVERB:
                        pos = "ADVERB";
                        break;
                    case (int)Define.PART_OF_SPEECH.SATELLITE:
                        pos = "ADJECTIVE_SATELITE";
                        break;
                }
                Functions.setTextStr(posButtons[i].GetComponentInChildren<Text>(), pos);
                int index = i;
                posButtons[i].onClick.RemoveAllListeners();
                posButtons[i].onClick.AddListener(() =>
                {
                    HapticFeedback.MediumFeedback();
                    SelectWordClass(index);
                });
            }
        }

        SelectWordClass(0);
    }

    private void SelectWordClass(int index)
    {
        if (index == currentPosIndex) return;
        currentPosIndex = index;
        currentSynsetIndex = 0;
        UpdateSynsetByIndex();

        for (int i = 0; i < posButtons.Length; i++)
        {
            Image image = posButtons[i].GetComponent<Image>();
            var color = i == index ? Define.ColorPOSSelected : Define.ColorPOSUnselected;
            ColorUtility.TryParseHtmlString(color, out Color baseColor);
            image.color = baseColor;
        }
    }

    private void ClearBookmarks()
    {
        int childCount = bookmarks.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(bookmarks.GetChild(i).gameObject);
        }
    }

    public void UpdateSynsetByIndex()
    {
        WordNetData.SenseComponents senseComponents = WordNetData.GetSynsetByPosIndexAndSenseIndex(currentPosIndex, currentSynsetIndex);
        currentSynsetId = senseComponents.synsetId;
        Debug.Log("senseComponents.definition == " + senseComponents.definition);
        definitionText.gameObject.SetActive(true);
        definitionText.text = senseComponents.definition;
        ClearBookmarks();
        int synsetsNum = WordNetData.CountSynsetsByPos(currentPosIndex);
        Debug.Log($"synsetsNum: {synsetsNum}");
        if (synsetsNum > 1)
        {
            
            for (int i = 0; i < synsetsNum; i++)
            {
                int posX = i * 60;

                GameObject objBookmark = CreateGameObject.createUIObject(bookmarks, new Vector2(posX, 0), CreateGameObject.pathPrefabSynsetMarker);
                int index = i;
                objBookmark.transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SelectSynsetIndex(index);
                });
                var color = i == currentSynsetIndex ? Define.ColorSynsetMarkerSelected : Define.ColorSynsetMarkerUnselected;
                ColorUtility.TryParseHtmlString(color, out Color baseColor);
                objBookmark.transform.GetComponent<Image>().color = baseColor;
                objBookmark.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
            }
        }

        GraphManager.instance.DestroyEntity();
        GenerateWordsNetBySynset();
        GraphManager.instance.CreateEntitiesFromWordNet(currentSynsetId);
        CreateConnectionBetweenLemmaAndSynonyms();
        GraphMechanism.instance.setAllAnimations(true);
    }
    private void UpdatePhoneticTranscription(string transcription)
    {
        if (transcription == null)
        {
            SetPhoneticVis(false);
            return;
        }
        SetPhoneticVis(true);
        phoneticText.text = transcription;
    }
    private void SelectSynsetIndex(int index)
    {
        if (index < 0 || index >= WordNetData.CountSynsetsByPos(currentPosIndex)) return;
        if (index == currentSynsetIndex) return;
        currentSynsetIndex = index;
        HapticFeedback.LightFeedback();
        UpdateSynsetByIndex();
    }

    public void ResetIndices()
    {
        currentPosIndex = -1;
        currentSynsetIndex = -1;
    }
    private void GenerateWordsNetBySynset()
    {
        linesToDraw.Clear();
        wordList.Clear();

        WordNetData.SenseComponents senseComponents = WordNetData.GetSynsetByPosIndexAndSenseIndex(currentPosIndex, currentSynsetIndex);
        List<string> words = WordNetData.GetSynonymWords(senseComponents);
        foreach (var word in words)
        {
            Debug.Log("word == " + word);
        }

        CustomLemmaStorage customLemmaStorage = CustomLemmaStorage.LoadCustomLemmaFromPlayerPrefs();
        List<string> customWords = customLemmaStorage.GetCustomLemmas(currentSynsetId);

        List<string> combinedWords = new List<string>();
        combinedWords.AddRange(words);
        combinedWords.AddRange(customWords);

        //Debug.Log("synsetId == " + currentSynsetId);
        Vector2 centerPosition = new Vector2(0.5f, 0.5f);
        wordList[getLemma()] = centerPosition;
        SetTipsVisible(combinedWords.Count > 1);
        if (combinedWords.Count > 1)
        {
            float angleStep = 360f / (combinedWords.Count - 1);
            Debug.Log("angleStep == " + angleStep);
            int count = 0;

            for (int i = 0; i < combinedWords.Count; i++)
            {
                if (isSameLemma(combinedWords[i])) continue;

                count++;
                float angle = count * angleStep * Mathf.Deg2Rad;
                // Calculate position relative to the center
                float x = Mathf.Cos(angle) * circleRadius;
                float y = Mathf.Sin(angle) * circleRadius;

                // Convert to viewport space (0 to 1 range)
                Vector2 position = new Vector2(

                    centerPosition.x + x / Screen.width,
                    centerPosition.y + y / Screen.height

                );
                wordList[combinedWords[i]] = position;
            }
            // foreach (var word in wordList)
            // {
            //     Debug.Log("wordPosition: " + word.Key + " == " + word.Value);
            // }
            // Debug.Log("wordList.Count == " + count);
        }
    }

    public bool isSameLemma(string word)
    {
        if (lemma + "(p)" == word) return true;
        return word.ToLower() == lemma.ToLower();
    }

    public void CreateConnectionBetweenLemmaAndSynonyms()
    {
        List<string> words = new List<string>(GetWords());
        foreach (var word in words)
        {
            string le = getLemma();
            if (word == le) continue;
            GraphManager.instance.CreateConnectionBetweenWords(le, word);
        }
    }

    public IEnumerable<string> GetWords()
    {
        return wordList.Keys;
    }

    public Vector2 GetWordPosition(string word)
    {
        return wordList[word];
    }

    public void updateLemma(string word)
    {
        lemma = word;
    }

    public string getLemma()
    {
        return lemma;
    }

    public void AlterAppearenceForWordnetMode(bool isWordnetMode)
    {
        ChoosingArea.SetActive(isWordnetMode);
        trendingArea.SetActive(!isWordnetMode);
        definitionText.gameObject.SetActive(isWordnetMode);
    }

    public void setChoosingAreaPos()
    {
        bool isSubscribed = IAPManager.Instance.IsSubscribed();
        ChoosingArea.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, isSubscribed ? 0 : 234);
    }

    public void ResponseSynsetContent(string lemma)
    {
        if (WordNetData.synsetGroupsByPos == null || WordNetData.synsetGroupsByPos.Count == 0)
            return;
        ContainerPanel.instance.SetTitle(lemma, false);
        ContainerPanel.instance.ToggleWordnetMode(true);

        ResetIndices();
        setContentAutoCompleteVisible(false);
        updateLemma(lemma);
        SetPartOfSpeechButtons();

    }

    public void ResponseSynonymsAIComparsion(string word1, string word2, string comments)
    {
        SynonymComparePanel.instance.init(word1, word2);
        SynonymComparePanel.instance.DisplayComments(comments);
    }
    public void ResponseTrendingWords()
    {
        //RecycleTrendingWords();
        trendingWords = Data.storedTrendingWords;
        for (int i = 0; i < trendingWords.Count; i++)
        {
            string word = trendingWords[i].word;
            float percentageChange = trendingWords[i].percentageChange;
            GameObject obj = CreateGameObject.createUIObject(trendingWordsContainer.content.transform, new Vector2(0, 0), CreateGameObject.pathTrendingWordEntity, false);
            obj.transform.GetComponent<TrendingWordEntityWidget>().init(i, word, percentageChange.ToString());
            wordTrendingWidgets.Add(obj);
        }

        float height = 255f;
        trendingWordsContainer.content.GetComponent<RectTransform>().sizeDelta =
        new Vector2(trendingWordsContainer.content.GetComponent<RectTransform>().sizeDelta.x, wordTrendingWidgets.Count * height + 150);
        for (int i = 0; i < wordTrendingWidgets.Count; i++)
        {
            wordTrendingWidgets[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * height);
        }
    }
    #endregion
    private void OnInputFieldSubmit(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

#if UNITY_EDITOR
        SubmitLemma(text);
#endif
    }

    // This will be called when the input field becomes selected
    private void OnInputFieldSelected(string text)
    {
        keyboard = TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.Default);
        AlterSearchingStyles(true);
    }
    private void OnInputValueChanged(string value)
    {
        if (autoCompleteCoroutine != null)
        {
            StopCoroutine(autoCompleteCoroutine);
        }
        if (value == "")
        {
            ClearContentAutoComplete();
            setContentAutoCompleteVisible(false);
            return;
        }
        autoCompleteCoroutine = StartCoroutine(AutoCompleteAfterDelay(value));
    }

    private void AlterSearchingStyles(bool isClickingOnInputField)
    {
        wordInputField.transform.Find("miginifer").gameObject.SetActive(!isClickingOnInputField);
        wordInputField.transform.Find("cancelButton").gameObject.SetActive(isClickingOnInputField);
        SetBtnAddWordVisible(!isClickingOnInputField);
        RectTransform rt = wordInputField.transform.GetComponent<RectTransform>();

        if (isClickingOnInputField)
        {
            rt.offsetMin = new Vector2(50, 30);   // Left, Bottom
            rt.offsetMax = new Vector2(-200, -40);  // -Right, -Top
        }
        else
        {
            if (ContainerPanel.instance.isWordnetMode)
            {
                rt.offsetMin = new Vector2(50, 30);   // Left, Bottom
                rt.offsetMax = new Vector2(-200, -40);  // -Right, -Top
            }
            else
            {
                rt.offsetMin = new Vector2(120, 30);   // Left, Bottom
                rt.offsetMax = new Vector2(-130, -40);  // -Right, -Top
            }
        }
        ContainerPanel.instance.SetBottomAreaVisible(!isClickingOnInputField);
        searchBackground.SetActive(isClickingOnInputField);
        AlterSearchAreaPos();
    }

    public void SetInputFieldPos(bool isClickingOnInputField)
    {
        RectTransform rt = wordInputField.transform.GetComponent<RectTransform>();
        
        // Define target values based on state
        Vector2 targetOffsetMin = new Vector2(isClickingOnInputField ? 50 : 120, 30);   // Left, Bottom
        Vector2 targetOffsetMax = new Vector2(isClickingOnInputField ? -200 : -130, -40);  // -Right, -Top
        
        // Store initial values
        Vector2 initialOffsetMin = rt.offsetMin;
        Vector2 initialOffsetMax = rt.offsetMax;
        
        // Kill any previous tweens on this transform
        DOTween.Kill(rt);
        
        // Create the animation using DOTween
        float duration = 0.3f;
        DOTween.To(() => 0f, val => {
            // Calculate interpolated values
            float t = val;
            rt.offsetMin = Vector2.Lerp(initialOffsetMin, targetOffsetMin, t);
            rt.offsetMax = Vector2.Lerp(initialOffsetMax, targetOffsetMax, t);
        }, 1f, duration)
        .SetEase(Ease.OutQuad)
        .SetTarget(rt);  
    }

    private void ExitSearchingMode()
    {
        ClearContentAutoComplete();
        wordInputField.text = "";
        setContentAutoCompleteVisible(false);
        wordInputField.DeactivateInputField();
        AlterSearchingStyles(false);
    }

    private void AlterSearchAreaPos()
    {
        bool isSearchBackgroundActive = searchBackground.activeSelf;
        searchArea.GetComponent<RectTransform>().anchoredPosition = isSearchBackgroundActive ? new Vector2(0, 110) : new Vector2(0, 0);
    }

    public void SubmitLemma(string text)
    {
        Debug.Log("SubmitLemma: " + text);
        ExitSearchingMode();
        if (!string.IsNullOrEmpty(text))
        {
            text = text.Trim();

            if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[!@#$%^&*(),.?""':;{}|<>]"))
            {
                PanelManager.CreateMessagePanel("INVALID_INPUT", true);
                return;
            }

            text = WordNetData.GetLemmaOfWord(text);
            if (text == "No results found")
            {
                PanelManager.CreateMessagePanel("NO_RESULTS_FOUND", true);
                return;
            }

            ServerManager.instance.TryToGetSynsets(text);

            wordInputField.text = "";
        }
    }

    private IEnumerator AutoCompleteAfterDelay(string value)
    {
        yield return new WaitForSeconds(AUTO_COMPLETE_DELAY);

        if (wordInputField.text == value) // Check if the text hasn't changed during the delay
        {
            ServerManager.instance.TryToGetAutoCompleteList(
                value,
                OnAutoCompleteSuccess,
                OnAutoCompleteError
            );
        }
    }

    private void OnAutoCompleteSuccess(List<string> suggestions)
    {
        if (!wordInputField.isFocused) return;
        int count = suggestions.Count > 10 ? 10 : suggestions.Count;
        if (count == 0)
        {
            setContentAutoCompleteVisible(false);
            return;
        }

        ClearContentAutoComplete();
        setContentAutoCompleteVisible(true);
        float height = 160;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = CreateGameObject.createUIObject(contentAutoComplete.content, new Vector2(0, 0 - i * height), CreateGameObject.pathWordSuggestion);

            string text = suggestions[i];
            obj.transform.GetComponent<RecommendationWordWidget>().SetText(text);
        }

        // LayoutRebuilder.ForceRebuildLayoutImmediate(contentAutoComplete.content.GetComponent<RectTransform>());
        // Canvas.ForceUpdateCanvases();
        float contentHeight = height * count;

        contentAutoComplete.content.GetComponent<RectTransform>().sizeDelta = new Vector2(contentAutoComplete.content.GetComponent<RectTransform>().sizeDelta.x, contentHeight);

        RectTransform rectTransform = contentAutoComplete.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, contentHeight);

        //contentAutoComplete.verticalNormalizedPosition = 1;
    }

    private void ClearContentAutoComplete()
    {
        int childCount = contentAutoComplete.content.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(contentAutoComplete.content.GetChild(i).gameObject);
        }
    }

    private void OnAutoCompleteError(string error)
    {
        Debug.LogError($"Autocomplete error: {error}");
    }

    private void setContentAutoCompleteVisible(bool visible)
    {
        contentAutoComplete.gameObject.SetActive(visible);
    }

    private void OnClickAddWord()
    {
        GameObject panel = PanelManager.CreatePanel(ContainerPanel.instance.transform, "AddCustomWordPanel", new Vector2(0, 1000), true);
        panel.transform.GetComponent<AddCustomWordPanel>().init(currentSynsetId);
        PanelManager.OpenPanelVertically(panel, null, 0);
    }
    public void SetPhoneticVis(bool visible)
    {
        phoneticText.gameObject.SetActive(visible);
    }
    public void SetBtnAddWordVisible(bool visible)
    {
        btnAddWord.gameObject.SetActive(visible);
    }

    public void SetTipsVisible(bool show)
    {

        if (onceClickTips < 5)
        {
            DOTween.Kill(tip);
        }
        
        if (show)
        {
            if (onceClickTips >= 5) return;
            onceClickTips++;
            tip.gameObject.SetActive(true);
            tip.DOFade(1, 0.3f);
        }
        else
        {
            tip.DOFade(0, 0f);
            tip.gameObject.SetActive(false);
        }
    }
}