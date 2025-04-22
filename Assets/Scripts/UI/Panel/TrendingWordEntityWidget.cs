using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrendingWordEntityWidget : MonoBehaviour
{
    [SerializeField] private Text txtWord;
    [SerializeField] private Text txtTrending;
    [SerializeField] private Text txtMeaning;
    [SerializeField] private Text txtIndex;
    [SerializeField] private Button btnWord;
    void Start()
    {
        btnWord.onClick.AddListener(() => { 
            ServerManager.instance.TryToGetSynsets(txtWord.text);
        });
    }
    public void init(int index, string word, string trending)
    {
        txtIndex.text = "#" + (index + 1).ToString();
        txtWord.text = word;
        txtTrending.text =  "+" + trending + "%";
        txtMeaning.text = WordNetData.GetFirstMeaningOfWord(word);
    }
}
