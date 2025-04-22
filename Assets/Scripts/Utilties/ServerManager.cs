using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Data = SynonymData;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Numerics;
public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;

    private const string BASE_URL = "http://127.0.0.1:8787";

    public event Action OnBookmarkReceived;
    public event Action<bool> OnBookmarkPagesReceived;
    public event Action<string, string, string> OnCommentsReceived;
    public event Action OnTrendingWordsReceived;
    public event Action OnSynsetReceived;
    public event Action OnFeedbackSent;
    public event Action<string, bool, float> OnMessageReceived;
    public event Action<string> OnPhoneticTranscriptionReceived;
    public event Action OnBookmarkAdded;
    void Awake()
    {
        instance = this;

        if (FindObjectOfType<UnityMainThreadDispatcher>() == null)
        {
            GameObject go = new GameObject("UnityMainThreadDispatcher");
            go.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }

        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
    }

    #region Get Synsets

    struct SynsetRequest
    {
        public string lemma;
        public string userId;
    }
    public async void TryToGetSynsets(string word)
    {
        string url = $"{BASE_URL}/routes/updateTrendingSearches";

        // var requestData = new SynsetRequest
        // {
        //     lemma = word,
        //     userId = LoginManager.instance.GetUserId()
        // };
        // string jsonRequest = JsonUtility.ToJson(requestData);

        // UnityWebRequest www = new UnityWebRequest(url, "POST");
        // byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
        // www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        // www.downloadHandler = new DownloadHandlerBuffer();
        // www.SetRequestHeader("Content-Type", "application/json");
        // www.certificateHandler = new BypassCertificate();

        // var operation = www.SendWebRequest();
        // while (!operation.isDone)
        // {
        //     await Task.Yield();
        // }

        // if (www.result != UnityWebRequest.Result.Success)
        // {
        //     Debug.LogError($"Error sending lemma: {www.error}");
        //     Debug.LogError($"Response Code: {www.responseCode}");
        //     Debug.LogError($"Response Body: {www.downloadHandler.text}");
        //     Debug.LogError($"Request URL: {www.url}");
        //     Debug.LogError($"Request Method: {www.method}");

        //     if (www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
        //     {
        //         Debug.LogError($"Response Body: {www.downloadHandler.text}");
        //     }
        //     SynonymStruct.SynsetResponse response = null;
        //     Debug.LogError($"Error getting synonym: {www.error}");
        // }
        // else
        // {
        //     WordNetData.UpdateSynsetsByLemma(word);
        //     WordnetPanel.instance.ResponseSynsetContent(word);
        //     TryToGetPhoneticTranscription(word);
        // }

        // www.Dispose();

        WordNetData.UpdateSynsetsByLemma(word);
        WordnetPanel.instance.ResponseSynsetContent(word);
        TryToGetPhoneticTranscription(word);
    }
    #endregion

    #region Get Comments
    public async void TryToGetAIComparsion(string word1, string word2)
    {
        AIComparsion aiComparsion = AIComparsion.LoadAIComparsionFromPlayerPrefs();
        string savedComparsion = aiComparsion.GetAIComparsion(word1, word2);
        bool isSavedInLocal = savedComparsion != "";

        string url = $"{BASE_URL}/routes/getAIComparsion";

        WWWForm form = new WWWForm();
        form.AddField("lemma1", word1);
        form.AddField("lemma2", word2);
        form.AddField("guid", SystemManager.Instance.GetGuid());
        form.AddField("userId", LoginManager.instance.GetUserId());
        form.AddField("isSubscribed", IAPManager.Instance.IsSubscribed() ? 1 : 0);
        form.AddField("isSavedInLocal", isSavedInLocal ? 1 : 0);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            if (www == null)
            {
                Debug.LogError("Failed to create UnityWebRequest");
                return;
            }

            www.certificateHandler = new BypassCertificate();


            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = $"Error: {www.error}\nResponse Code: {www.responseCode}\nResponse Body: {www.downloadHandler.text}";
                Debug.LogError(errorMessage);
                if (www.responseCode == 403)
                {
                    OnMessageReceived?.Invoke("NO_MORE_AI_SEARCHING_REMAIN_COUNT", true, 1.5f);
                }
            }
            else
            {
                if(isSavedInLocal)
                {
                    OnCommentsReceived?.Invoke(word1, word2, savedComparsion);
                    return;
                }

                string response = www.downloadHandler.text;
                JObject jsonObject = JObject.Parse(response);
                string responseContent = jsonObject["resp"].ToString();
                aiComparsion.AddAIComparsion(word1, word2, responseContent);
                OnCommentsReceived?.Invoke(word1, word2, responseContent);
            }

        }
    }
    #endregion

    #region Send Feedback
    public async void TryToSendFeedback(string feedback)
    {
        string url = $"{BASE_URL}/routes/sendFeedback";
        WWWForm form = new WWWForm();
        form.AddField("userId", LoginManager.instance.GetUserId());
        form.AddField("feedback", feedback);
        form.AddField("guid", SystemManager.Instance.GetGuid());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {www.error}");
                Debug.LogError($"Response Code: {www.responseCode}");
                if (www.responseCode == 400)
                {
                    OnMessageReceived?.Invoke("FEEDBACK_LIMIT_REACHED", true, 1.5f);
                }
            }
            else
            {
                OnFeedbackSent?.Invoke();
                Debug.Log($"Feedback sent successfully. Response: {www.downloadHandler.text}");
            }
        }
    }
    #endregion

    #region Get Bookmarks
    public async void TryToGetAllBookmarks(bool settingNewBookmark = false)
    {

        if (LoginManager.instance.IsUserLoggedIn() && IAPManager.Instance.IsSubscribed())
        {
            string url = $"{BASE_URL}/routes/getAllBookmarks";
            WWWForm form = new WWWForm();

            form.AddField("userId", LoginManager.instance.GetUserId());
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.certificateHandler = new BypassCertificate();
                var operation = www.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error: {www.error}");
                    Debug.LogError($"Response Code: {www.responseCode}");
                }
                else
                {
                    string response = www.downloadHandler.text;
                    JObject jsonObject = JObject.Parse(response);
                    //List<SynonymStruct.BookmarkPages> bookmarkPages = jsonObject["bookmark_types"].ToObject<List<SynonymStruct.BookmarkPages>>();
                    Dictionary<int, string> bookmarks = jsonObject["bookmark_types"]
                    .ToObject<List<JObject>>()
                    .ToDictionary(
                        item => (int)item["bookid"],
                        item => (string)item["bookmarkName"]
                    );
                    Bookmark bookmark = new Bookmark(bookmarks);
                    bookmark.SaveBookmarkToPlayerPrefs();
                    OnBookmarkPagesReceived?.Invoke(settingNewBookmark);
                }
            }
        }
        else
        {
            // Bookmark bookmark = new Bookmark();
            // bookmark.SaveBookmarkToPlayerPrefs();           
            OnBookmarkPagesReceived?.Invoke(settingNewBookmark);
        }
    }

    public async void TryToDeleteSpecificBookmark(int bookmarkPageId)
    {
        string url = $"{BASE_URL}/routes/deleteSpecificBookmark";
        WWWForm form = new WWWForm();
        form.AddField("userId", LoginManager.instance.GetUserId());
        form.AddField("bookmark_page_id", bookmarkPageId);
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {

            }
            else
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Bookmark pageeee deleted successfully. Response: {response}");
                Bookmark bookmark = Bookmark.LoadBookmarkFromPlayerPrefs();
                bookmark.DeleteBookmark(bookmarkPageId);
                bookmark.SaveBookmarkToPlayerPrefs();
                TryToGetAllBookmarks(false);
            }
        }
    }
    //all the default bookmark name are "New Bookmark", actually the parameter is spare
    public async void TryToAddNewBookmark(string bookmarkPageName)
    {
        string url = $"{BASE_URL}/routes/addNewBookmark";
        WWWForm form = new WWWForm();
        form.AddField("bookmark_name", bookmarkPageName);
        form.AddField("userId", LoginManager.instance.GetUserId());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {www.error}");
                Debug.LogError($"Response Code: {www.responseCode}");
            }
            else
            {
                Debug.Log($"Bookmark pages modified successfully. Response: {www.downloadHandler.text}");
                TryToGetAllBookmarks(true);
            }
        }
    }

    public async void TryToChangeBookmarkName(int id, string name)
    {
        if (!LoginManager.instance.IsUserLoggedIn() || !IAPManager.Instance.IsSubscribed())
        {
            Bookmark bookmark = Bookmark.LoadBookmarkFromPlayerPrefs();
            bookmark.ChangeBookmarkName(id, name);
            bookmark.SaveBookmarkToPlayerPrefs();
            TryToGetAllBookmarks(false);
            return;
        }
        string url = $"{BASE_URL}/routes/changeBookmarkName";
        WWWForm form = new WWWForm();
        form.AddField("userId", LoginManager.instance.GetUserId());
        form.AddField("id", id);
        form.AddField("name", name);
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {www.error}");
                Debug.LogError($"Response Code: {www.responseCode}");
            }
            else
            {
                Debug.Log($"Bookmark name changed successfully. Response: {www.downloadHandler.text}");
                TryToGetAllBookmarks(false);
            }
        }
    }
    public async void TryToDeleteALemmaInBookmark(int bookmarkPageId, string lemma)
    {
        if (!LoginManager.instance.IsUserLoggedIn() || !IAPManager.Instance.IsSubscribed())
        {
            Bookmark bookmark = Bookmark.LoadBookmarkFromPlayerPrefs();
            bookmark.DeleteLemmaFromBookmark(bookmarkPageId, lemma);
            bookmark.SaveBookmarkToPlayerPrefs();
            TryToGetLemmasByBookmarkId(bookmarkPageId);
            return;
        }
        string url = $"{BASE_URL}/routes/deleteALemmaInBookmark";
        WWWForm form = new WWWForm();
        form.AddField("userId", LoginManager.instance.GetUserId());
        form.AddField("bookmark_page_id", bookmarkPageId);
        form.AddField("lemma", lemma);
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Bookmark deleted successfully. Response: {response}");
                TryToGetLemmasByBookmarkId(bookmarkPageId);
            }
        }
    }
    public async void TryToGetLemmasByBookmarkId(int bookmarkPageId = 0)
    {
        if (!LoginManager.instance.IsUserLoggedIn() || !IAPManager.Instance.IsSubscribed())
        {
            OnBookmarkReceived?.Invoke();
            return;
        }
        string url = $"{BASE_URL}/routes/getLemmasByBookmarkId";
        WWWForm form = new WWWForm();
        form.AddField("userId", LoginManager.instance.GetUserId());
        form.AddField("bookmark_page_id", bookmarkPageId);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.certificateHandler = new BypassCertificate(); // Add this line to bypass SSL certificate validation
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                Debug.LogError($"Full URL: {webRequest.url}");
                Debug.Log($"Downloaded bytes: {webRequest.downloadedBytes}");
                Debug.Log($"Request completed. Result: {webRequest.result}");

                if (webRequest.downloadHandler != null && !string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    Debug.LogError($"Response Body: {webRequest.downloadHandler.text}");
                }
                Debug.LogError($"Error getting bookmarks: {webRequest.error}");
            }
            else
            {
                string response = webRequest.downloadHandler.text;
                JObject jsonObject = JObject.Parse(response);
                List<string> bookmarks = jsonObject["lemmas"].ToObject<List<string>>();
                Bookmark bookmark = Bookmark.LoadBookmarkFromPlayerPrefs();
                bookmark.SetLemmaToBookmark(bookmarkPageId, bookmarks);
                bookmark.SaveBookmarkToPlayerPrefs();
                OnBookmarkReceived?.Invoke();
            }
        }
    }
    public async void TryToAddLemmaToBookmark(string lemma, int bookId = 0)
    {
        if (bookId == 0) //this is the default bookmark
        {
            Bookmark bookmark = Bookmark.LoadBookmarkFromPlayerPrefs();
            bookmark.AddLemmaToBookmark(bookId, lemma);
            bookmark.SaveBookmarkToPlayerPrefs();
            OnBookmarkAdded?.Invoke();
        }

        if (!LoginManager.instance.IsUserLoggedIn() || !IAPManager.Instance.IsSubscribed())
        {
            return;
        }

        //for the subscription user
        string userId = LoginManager.instance.GetUserId();
        Debug.Log($"Adding lemma: {lemma} for userId: {userId}");

        string url = $"{BASE_URL}/routes/addLemmaToBookmark";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("lemma", lemma);
        form.AddField("book_id", bookId);  // Changed from bookmark_id to book_id

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error sending lemma: {www.error}");
                Debug.LogError($"Response Code: {www.responseCode}");
                Debug.LogError($"Response Body: {www.downloadHandler.text}");
                Debug.LogError($"Request URL: {www.url}");
                Debug.LogError($"Request Method: {www.method}");
                Debug.LogError($"Request Body: userId={userId}&lemma={lemma}&book_id=0");
            }
            else
            {
                if(bookId != 0)
                {
                    OnBookmarkAdded?.Invoke();
                }
                Debug.Log($"Lemma sent successfully. Response: {www.downloadHandler.text}");
            }
        }
    }
    #endregion

    #region Get autoComplete
    public async void TryToGetAutoCompleteList(string prefix, Action<List<string>> onComplete, Action<string> onError)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            onError?.Invoke("Prefix is required");
            return;
        }

        string url = $"{BASE_URL}/routes/getAutoCompleteList?prefix={UnityWebRequest.EscapeURL(prefix)}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {www.error}");
                onError?.Invoke($"Error: {www.error}");
            }
            else
            {

                string jsonResponse = www.downloadHandler.text;
                Debug.Log($"Raw JSON response: {jsonResponse}"); // Log the raw response

                JObject jsonObject = JObject.Parse(jsonResponse);
                JArray suggestionsArray = (JArray)jsonObject["suggestions"];

                if (suggestionsArray != null)
                {
                    List<string> suggestions = suggestionsArray.ToObject<List<string>>();
                    onComplete?.Invoke(suggestions);
                }
                else
                {
                    onError?.Invoke("Invalid response format: 'suggestions' array not found");
                }

            }
        }
    }
    #endregion

    #region Custom Lemma
    public async void TryToAddCustomLemmaToSynset(string lemma, uint synsetId)
    {
        if (!Functions.wordValidalityCheck(lemma))
        {
            ContainerPanel.instance.ShowMessage("INVALID_WORD", true);
            return;
        }
        CustomLemmaStorage customLemmaStorage = CustomLemmaStorage.LoadCustomLemmaFromPlayerPrefs();
        bool isAdded = customLemmaStorage.AddCustomLemma(synsetId, lemma);
        if (isAdded)
        {
            OnSynsetReceived?.Invoke();
        }
        else
        {
            OnMessageReceived?.Invoke("LEMMA_ALREADY_EXISTS", true, 1.5f);
        }

        if (!LoginManager.instance.IsUserLoggedIn() || !IAPManager.Instance.IsSubscribed())
        {
            return;
        }

        //to server
        string userId = LoginManager.instance.GetUserId();
        string url = $"{BASE_URL}/routes/addCustomLemmaToSynset";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("lemma", lemma);
        form.AddField("synsetId", synsetId.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                Debug.Log("Lemma added successfully");
            }
        }
    }

    [System.Serializable]
    private struct CustomLemmaItem
    {
        public uint synsetId { get; set; }
        public string lemma { get; set; }
    }
    public async void TryToGetAllSavedCustomSynonyms()
    {
        if (!LoginManager.instance.IsUserLoggedIn()||!IAPManager.Instance.IsSubscribed())
        {
            return;
        }

        string userId = LoginManager.instance.GetUserId();
        string url = $"{BASE_URL}/routes/getAllSavedCustomSynonyms";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                Debug.Log("Custom lemmas retrieved successfully");
                try
                {
                    string responseText = www.downloadHandler.text;
                    Debug.Log($"Raw response: {responseText}"); // For debugging

                    JObject jsonObject = JObject.Parse(responseText);
                    List<CustomLemmaItem> customLemmas = jsonObject["lemmas"].ToObject<List<CustomLemmaItem>>();

                    CustomLemmaStorage storage = CustomLemmaStorage.LoadCustomLemmaFromPlayerPrefs();
                    foreach (var item in customLemmas)
                    {
                        storage.AddCustomLemma(item.synsetId, item.lemma);
                        Debug.Log($"Custom lemma added to storage: {item.synsetId} - {item.lemma}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing custom lemmas: {e.Message}");
                    Debug.LogError($"Raw response: {www.downloadHandler.text}");
                }
            }
        }
    }
    #endregion
    public async void TryToGetPhoneticTranscription(string lemma)
    {
        if (!IAPManager.Instance.IsSubscribed() || !LoginManager.instance.IsUserLoggedIn())
        {
            Debug.Log("User is not subscribed or logged in");
            OnPhoneticTranscriptionReceived?.Invoke(null);
            return;
        }
        AIPhoneticTranscription aiPhoneticTranscription = AIPhoneticTranscription.LoadAIPhoneticTranscriptionFromPlayerPrefs();
        string savedTranscription = aiPhoneticTranscription.GetAIPhoneticTranscription(lemma);
        if(savedTranscription != "")
        {
            OnPhoneticTranscriptionReceived?.Invoke(savedTranscription);
            return;
        }


        string url = $"{BASE_URL}/routes/getPhoneticTranscription";
        WWWForm form = new WWWForm();
        form.AddField("lemma", lemma);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                JObject jsonObject = JObject.Parse(www.downloadHandler.text);
                string transcription = jsonObject["transcription"].ToString();
                aiPhoneticTranscription.AddAIPhoneticTranscription(lemma, transcription);
                OnPhoneticTranscriptionReceived?.Invoke(transcription);
            }
        }
    }
    #region Get Trending Words
    public async void TryToGetTrendingSearches()
    {
        // string url = $"{BASE_URL}/routes/getTrendingSearches";
        // WWWForm form = new WWWForm();

        // using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        // {
        //     www.certificateHandler = new BypassCertificate();
        //     var operation = www.SendWebRequest();

        //     while (!operation.isDone)
        //     {
        //         await Task.Yield();
        //     }

        //     if (www.result != UnityWebRequest.Result.Success)
        //     {
        //         Debug.LogError($"Error: {www.error}");                
        //     }
        //     else
        //     {
        //         string response = www.downloadHandler.text;
        //         JObject jsonObject = JObject.Parse(response);
        //         JArray trendingWordsArray = jsonObject["trendingWords"] as JArray;

        //         // Create a list of TrendingWord objects instead of just strings
        //         List<SynonymStruct.TrendingWord> trendingWords = trendingWordsArray.Select(item => new SynonymStruct.TrendingWord
        //         {
        //             word = item["word"].ToString(),
        //             percentageChange = item["percentage_change"].Value<float>()
        //         }).ToList();
                
        //         Data.storedTrendingWords = trendingWords;
        //         Debug.Log("Trending words loaded from server");
        //     }

        //     OnTrendingWordsReceived?.Invoke();
        // }

        
        List<SynonymStruct.TrendingWord> trendingWords = new List<SynonymStruct.TrendingWord>();
        string[] words = new string[10]{"Hello", "World", "Test", "Example", "Demo", "Speak", "Wonderful", "shark", "necessary", "charming"};
    
        for (int i = 0; i < 10; i++)
        {
            trendingWords.Add(new SynonymStruct.TrendingWord() { word = words[i], percentageChange =  (10 - i) * 1.2f * 10 });
        }
        Data.storedTrendingWords = trendingWords;

        OnTrendingWordsReceived?.Invoke();
    }
    #endregion

    #region upadteUserInfo
    public async void TryToUploadClientInfo()
    {
        string url = $"{BASE_URL}/routes/UploadClientInfo";
        WWWForm form = new WWWForm();

        form.AddField("guid", SystemManager.Instance.GetGuid());
        form.AddField("userId", LoginManager.instance.GetUserId());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                string response = www.downloadHandler.text;
                JObject jsonObject = JObject.Parse(response);
                string version = jsonObject["version"].ToString();
                SystemManager.Instance.checkCurrentVersion(version);
            }
        }
    }
    #endregion

    public async void TryToDeleteAccountFromServer()
    {
        string url = $"{BASE_URL}/routes/deleteAccount";
        WWWForm form = new WWWForm();
        form.AddField("userId", LoginManager.instance.GetUserId());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.certificateHandler = new BypassCertificate();
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }   
            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                Debug.Log("Account deleted successfully");
            }
        }
    }
}
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        Debug.Log("Bypassing SSL certificate validation");
        return true;
    }
}
