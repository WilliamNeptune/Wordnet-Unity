using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LangSettingPanel: ParentPanel
{
    [SerializeField] private Button[] btnLangs;

    private string[] showNames = new string[] { "English", "日本語", "한국어", "Deutsch"};

    private void Start()
    {
        base.Start();
        
        int curLangIndex = SystemManager.Instance.getCurLangIndex();
        for(int i = 0; i < btnLangs.Length; i++)
        {
            int index = i;
            btnLangs[index].onClick.AddListener(() => OnClickBtnLang(index));
            btnLangs[index].transform.Find("Name").GetComponent<Text>().text = showNames[index];
            btnLangs[index].transform.Find("tick").gameObject.SetActive(index == curLangIndex);
        }
    }

    private void OnClickBtnLang(int index)
    {
        for(int i = 0; i < btnLangs.Length; i++)
        {
            btnLangs[i].transform.Find("tick").gameObject.SetActive(i == index);
        }

        SystemManager.Instance.SetLang(index);

    }
}