using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class IntroductionPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI content;

    [SerializeField]
    private Text content2;
    public void insitialize(int index)
    {
      content2.gameObject.SetActive(index == 0);
      content.gameObject.SetActive(index != 0);
      if (index == 0)
      {
        content2.text = "\n\nThe application is inspired by the WordNet, a lexical database of English words. " +
          "Nouns, verbs, adjectives, and adverbs are grouped into sets of cognitive synonyms (synsets), " +
          "each expressing a distinct concept. Synsets are interlinked by means of conceptual-semantic and lexical relations.\n\n" +
          "When I first learned about WordNet, I thought it was an excellent dictionary. In the process of using it, " +
          "I found I needed to know the difference between the synonyms, which can help me to understand the word better. " +
          "Additionally, I found using AI to illustrate the meaning of the word is a more effective way to achieve this goal. " +
          "Based on the above idea, I created this application.\n\n" +
          "In the process of creating this application, I use some third-party libraries and services, including an AI service and the Wordnet database. You can find more information about them in the License section. \n\n" +
          "Wish this application can help you learn new words and improve your interest in learning language.";
      }
      else
      {
        content.text = "This application is licensed under the <link=\"https://wordnet.princeton.edu/license-and-commercial-use\"><color=blue>WordNet License</color></link>. " +
                  "Meanwhile, this application integrates Llama 3.1, a product of Meta Platforms, Inc. By using this application, you agree to the Llama 3.1 Community License Agreement and Meta's Privacy Policy. For more information, please visit <link=\"https://llama.meta.com\"><color=blue>Llama 3.1 Documentation</color></link>." +
                  "This product is 'Built with Llama. Llama 3.1 is licensed under the Llama 3.1 Community License, Copyright © Meta Platforms, Inc. All Rights Reserved.";
      }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(content, Input.mousePosition, null);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = content.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
