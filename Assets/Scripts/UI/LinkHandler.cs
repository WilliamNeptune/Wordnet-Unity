using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LinkHandler : MonoBehaviour
{
    private TMP_Text textMeshPro;
    
    void Awake()
    {
        textMeshPro = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        textMeshPro.text = Functions.getTextStr("SUBSCRIPTION_TERMS");
    }

    void Update()
    {
        // Check for touch/click input
        if (Input.GetMouseButtonDown(0))
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
                string linkID = linkInfo.GetLinkID();
                
                switch (linkID)
                {
                    case "terms":
                        Application.OpenURL("https://www.apple.com/legal/internet-services/itunes/dev/stdeula/");
                        break;
                    case "privacy":
                        Application.OpenURL("https://synonymnetfiles.pages.dev/privacy_policy");
                        break;
                }
            }
        }
    }
}