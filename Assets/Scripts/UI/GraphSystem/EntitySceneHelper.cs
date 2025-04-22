using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

namespace GraphSystem
{
    public class EntitySceneHelper : MonoBehaviour
    {
        public List<Connection> connections = new List<Connection>();
        private float defaultSize = 0.6f;

        // public void OnMouseEnter()
        // {
        //     if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf && !GraphMechanism.instance.UIMouseBlock)
        //         transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = true;
        // }

        // public void OnMouseExit()
        // {
        //     transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = false;
        // }
        public void init(string word)
        {
            setWord(word);
            setObjectSize(defaultSize, 0f);
        }

        public void setWord(string word)
        {
            string modifiedText = word.Replace("_", " ");
            transform.Find("Text").GetComponent<TextMeshPro>().text = modifiedText;
        }

        public string getWord()
        {
            string text = transform.Find("Text").GetComponent<TextMeshPro>().text;
            return text;
        }

        public void switchToCurrentLemma()
        {
            ServerManager.instance.TryToGetSynsets(getWord());
        }
        public void setObjectSize(float size, float duration = 0.3f)
        {
            transform.DOScale(size, duration);
        }
    }
}
