using UnityEngine;
using UnityEngine.UI;

public class ColorModeSetting: ParentPanel
{
    [SerializeField] private Button[] btnColorModes;

    public static ColorModeSetting instance;
    
    private void Start()
    {
        base.Start();
        int curColorModeIndex = Functions.LoadColorModePreference();
        for(int i = 0; i < btnColorModes.Length; i++)
        {
            int index = i;
            btnColorModes[index].onClick.AddListener(() => OnClickBtnColorMode(index));
            btnColorModes[index].transform.Find("tick").gameObject.SetActive(index == curColorModeIndex);
        }
    }

    private void OnClickBtnColorMode(int index)
    {
        for(int i = 0; i < btnColorModes.Length; i++)
        {
            btnColorModes[i].transform.Find("tick").gameObject.SetActive(i == index);
        }

        SystemManager.Instance.saveColorMode(index);
    }
}