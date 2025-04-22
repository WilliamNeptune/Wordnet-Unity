using UnityEngine;
using UnityEngine.UI;

public class LineDrawer : MonoBehaviour
{
    public RectTransform centerText;
    public RectTransform[] connectedTexts;
    public Color lineColor = Color.white;
    public float lineWidth = 2f;

    private Canvas canvas;
    private RectTransform canvasRect;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void OnPostRender()
    {
        if (centerText == null || connectedTexts == null || connectedTexts.Length == 0)
            return;

        // Create a material for line drawing
        Material lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);

        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        Vector3 centerPos = GetWorldPositionFromRectTransform(centerText);
        foreach (RectTransform connectedText in connectedTexts)
        {
            if (connectedText != null)
            {
                Vector3 connectedPos = GetWorldPositionFromRectTransform(connectedText);
                GL.Vertex(centerPos);
                GL.Vertex(connectedPos);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    private Vector3 GetWorldPositionFromRectTransform(RectTransform rectTransform)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, rectTransform.position, canvas.worldCamera, out localPoint);
        return canvas.transform.TransformPoint(localPoint);
    }
}