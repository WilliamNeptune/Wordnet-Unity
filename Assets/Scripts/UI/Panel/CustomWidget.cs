using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomWidget : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isProspect = true;
    void Start()
    {
        Functions.OnColorModeChanged += ChangeColorMode;
        ChangeColorMode();
    }

    void OnDestroy()
    {
        Functions.OnColorModeChanged -= ChangeColorMode;
    }

    private void ChangeColorMode()
    {
        var image = GetComponent<Image>();
        if (image != null)
        {
            image.color = isProspect ? Functions.getProspectColor(): Functions.getBackgroundColor();
        }
        var text = GetComponent<Text>();

        if (text != null)
        {
            text.color = isProspect ? Functions.getProspectColor(): Functions.getBackgroundColor();
        }

        var textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.color = isProspect ? Functions.getProspectColor(): Functions.getBackgroundColor();
        }
    }
}
