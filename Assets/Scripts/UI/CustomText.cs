using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CustomText : Text
{
    [Tooltip("Key for localized text")]
    public string StringKey = "";

    public bool newLineFlag = false;
    public bool toUpper = false;
    public int LangType = 1;
    public int VisibleLines { get; private set; }
    public bool followColorMode = true;
    protected override void Awake()
    {
        base.Awake();
        UpdateTextFromKey();
    }

    protected override void Start()
    {
        base.Start();
        ChangeColorMode();
        Functions.OnColorModeChanged += ChangeColorMode;
        Functions.OnLangChanged += UpdateTextFromKey;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Functions.OnColorModeChanged -= ChangeColorMode;
        Functions.OnLangChanged -= UpdateTextFromKey;
    }

    public void ChangeColorMode()
    {
        if (followColorMode)
        {
            Color color = Functions.LoadColorModePreference() == 0 ? Define.ColorBlack : Define.ColorWhite;
            this.color = color;
        }
    }

    private void UpdateTextFromKey()
    {
        if (!this || !isActiveAndEnabled) return;

        if (!string.IsNullOrEmpty(StringKey))
        {
            string str = Functions.getTextStr(StringKey);
            if (str != null)
            {
                if (newLineFlag)
                {
                    str = str.Replace("\\n", "\n");
                }

                if (toUpper)
                {
                    str = str.ToUpper();
                }
                this.text = str;
            }
        }
    }

    public override string text {
        get {
            return base.text;
        }
        set {
            if (!this || !isActiveAndEnabled) return;

            if (newLineFlag)
            {
                value = value.Replace("\\n", "\n");
            }

            if (toUpper)
            {
                value = value.ToUpper();
            }
            base.text = value;
        }
    }

    private void _UseFitSettings()
    {
        TextGenerationSettings settings = GetGenerationSettings(rectTransform.rect.size);
        settings.resizeTextForBestFit = false;

        if (!resizeTextForBestFit)
        {
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);
            return;
        }

        int minSize = resizeTextMinSize;
        int txtLen = text.Length;
        for (int i = resizeTextMaxSize; i >= minSize; --i)
        {
            settings.fontSize = i;
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);
            if (cachedTextGenerator.characterCountVisible == txtLen) break;
        }
    }

    private readonly UIVertex[] _tmpVerts = new UIVertex[4];
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (null == font) return;

        m_DisableFontTextureRebuiltCallback = true;
        _UseFitSettings();

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;

        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0)
        {
            toFill.Clear();
            return;
        }

        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        if (roundingOffset != Vector2.zero)
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                _tmpVerts[tempVertsIndex] = verts[i];
                _tmpVerts[tempVertsIndex].position *= unitsPerPixel;
                _tmpVerts[tempVertsIndex].position.x += roundingOffset.x;
                _tmpVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(_tmpVerts);
            }
        }
        else
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                _tmpVerts[tempVertsIndex] = verts[i];
                _tmpVerts[tempVertsIndex].position *= unitsPerPixel;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(_tmpVerts);
            }
        }

        m_DisableFontTextureRebuiltCallback = false;
        VisibleLines = cachedTextGenerator.lineCount;
    }
}
