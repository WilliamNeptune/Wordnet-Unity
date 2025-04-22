using UnityEngine;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Runtime.InteropServices;
using System;
using System.Security;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
public class SystemManager : MonoBehaviour
{
    private static SystemManager _instance;
    public static SystemManager Instance => _instance;
    private const string LANGUAGE_PREF_KEY = "SelectedLanguage";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void SystemInitialize()
    {
        int curLangIndex = LoadLanguagePreference();
        Functions.loadLangPackage(curLangIndex);
    }
    private static Queue<(float, Action)> actionQueue = new Queue<(float, Action)>();
    public static float systemRunningTime { get; private set; } = 0f;
    private bool isPaused = false;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            LoadConfig();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        string dataPath;
#if UNITY_IOS && !UNITY_EDITOR
        dataPath = Path.Combine(Application.streamingAssetsPath, "WordNetData", "dict");
#elif UNITY_EDITOR
        dataPath = Path.Combine(Application.dataPath, "WordNetData", "dict");
#else
        dataPath = Path.Combine(Application.streamingAssetsPath, "WordNetData", "dict");
#endif

        WordNetData.setDataPath(dataPath);

    }
    private void Start()
    {
        if (!PlayerPrefs.HasKey("SelectedLanguage"))
        {
            int systemLang = GetSystemLanguageIndex();
            SetLang(systemLang);
        }
       // ServerManager.instance.TryToUploadClientInfo();
    }

    private void Update()
    {
        if (!isPaused)
        {
            systemRunningTime += Time.deltaTime;
            if (actionQueue.Count > 0)
            {
                (float launchTime, Action action) = actionQueue.Peek();

                if (systemRunningTime >= launchTime)
                {
                    actionQueue.Dequeue();
                    action();
                }
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
        Debug.Log("OnApplicationPause: " + isPaused);
        if (isPaused)
        {
            SaveOnlineTime();
        }
    }

    void OnApplicationQuit()
    {
        SaveOnlineTime();
    }

    private void SaveOnlineTime()
    {
        PlayerPrefs.SetInt("OnlineTime", (int)systemRunningTime);
        PlayerPrefs.Save();
    }
    private void LoadOnlineTime()
    {
        systemRunningTime = PlayerPrefs.GetFloat("OnlineTime", 0f);
    }

    public void EnqueueAction(float launchTime, Action action)
    {
        actionQueue.Enqueue((launchTime, action));
    }

    public void SetLang(int index)
    {
        Functions.loadLangPackage(index);
        PlayerPrefs.SetInt(LANGUAGE_PREF_KEY, index);
        PlayerPrefs.Save();
        Functions.InvokeLangChanged();
    }

    public void saveReviewRequested(bool isRequested)
    {
        PlayerPrefs.SetInt("ReviewRequested", isRequested ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool getReviewRequested()
    {
        return PlayerPrefs.GetInt("ReviewRequested", 0) == 1;
    }

    public int getCurLangIndex()
    {
        int index = LoadLanguagePreference();
        return index;
    }

    private static int LoadLanguagePreference()
    {
        return PlayerPrefs.GetInt(LANGUAGE_PREF_KEY, 0); // 0 is the default value (English)
    }
    //0 whilte 1 dark
    public void saveColorMode(int colorIndex)
    {
        PlayerPrefs.SetInt("ColorMode", colorIndex);
        PlayerPrefs.Save();
        Functions.InvokeColorModeChanged();//? is appropriate?
    }

    struct Config
    {
        public string email;
        public string password;
        public bool useConfigUser;
    }
    private void LoadConfig()
    {

        string configPath = Path.Combine(Application.dataPath, "config.json");
        if (File.Exists(configPath))
        {
            string jsonContent = File.ReadAllText(configPath);
            Config config = JsonUtility.FromJson<Config>(jsonContent);
        }
        else
        {
            Debug.LogError("Config file not found!");
        }
    }

    public int GetSystemLanguageIndex()
    {
        SystemLanguage lang = Application.systemLanguage;

        switch (lang)
        {
            case SystemLanguage.English:
                return 0;  // Your English index
            case SystemLanguage.Japanese:
                return 1;  // Your Japanese index
            case SystemLanguage.Korean:
                return 2;  // Your Korean index
            default:
                return 0;  // Default to English
        }
    }
    public string GetGuid()
    {
        string guid = PlayerPrefs.GetString("Guid", "");
        Debug.Log("GetGuid: " + guid);
        if (guid == "")
        {
            guid = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("Guid", guid);
            PlayerPrefs.Save();
        }
        return guid;
    }

    public void checkCurrentVersion(string version)
    {
        if(version != SynonymData.GetCurrentVersion())
        {
            ContainerPanel.instance.ShowMessage("UPDATE_POPUP_TITLE", true, 3f);
        }
    }
}
