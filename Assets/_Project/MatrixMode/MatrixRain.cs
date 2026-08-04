using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

// Renders the classic "digital rain": columns of green glyphs streaming down
// a black screen. The rig is parented to the camera and scales with zoom so
// the effect always covers the view. Created at runtime by MatrixModeController.
public class MatrixRain : MonoBehaviour
{
    private const string Glyphs = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ#$%&+=<>*:;?";
    private const float GlyphSize = 1f; // World units per glyph at the design zoom
    private const float DesignOrthoSize = 12f; // Matches CameraController's desktop zoom
    private const float Margin = 1.2f; // Overdraw so the edges stay covered

    private static readonly Color32 HeadColor = new Color32(210, 255, 220, 255);
    private static readonly Color32 TrailColor = new Color32(0, 255, 65, 255);

    private class Column
    {
        public TextMeshPro text;
        public char[] glyphs;
        public float head; // Row index of the leading (brightest) glyph
        public float speed; // Rows per second
        public int trailLength;
        public float mutateTimer;
    }

    private Camera cam;
    private TMP_FontAsset font;
    private readonly List<Column> columns = new();
    private int rows;
    private float builtAspect;

    public void Initialize(Camera camera, TMP_FontAsset fontAsset)
    {
        cam = camera;
        font = fontAsset != null ? fontAsset : TMP_Settings.defaultFontAsset;
        transform.SetParent(cam.transform, false);
        // Sit on the world plane in front of the camera; sorting layers keep
        // the rain behind the player, title, and world-space UI.
        float depth = Mathf.Max(-cam.transform.position.z, cam.nearClipPlane + 1f);
        transform.localPosition = new Vector3(0f, 0f, depth);
        Build();
    }

    void Update()
    {
        if (cam == null) return;

        // Scale with zoom instead of rebuilding; rebuild only when the aspect
        // ratio changes (e.g. a phone rotates or the window is resized).
        transform.localScale = Vector3.one * (cam.orthographicSize / DesignOrthoSize);
        if (Mathf.Abs(cam.aspect - builtAspect) > 0.05f * builtAspect)
        {
            Build();
        }

        foreach (Column column in columns)
        {
            AnimateColumn(column);
        }
    }

    private void Build()
    {
        foreach (Column column in columns)
        {
            Destroy(column.text.gameObject);
        }
        columns.Clear();

        builtAspect = cam.aspect;
        float height = DesignOrthoSize * 2f * Margin;
        float width = DesignOrthoSize * 2f * builtAspect * Margin;
        rows = Mathf.CeilToInt(height / GlyphSize);
        int columnCount = Mathf.CeilToInt(width / GlyphSize);

        for (int i = 0; i < columnCount; i++)
        {
            float x = -width / 2f + (i + 0.5f) * GlyphSize;
            columns.Add(CreateColumn(x, height));
        }
    }

    private Column CreateColumn(float x, float height)
    {
        GameObject columnObject = new GameObject("Column");
        columnObject.transform.SetParent(transform, false);
        columnObject.transform.localPosition = new Vector3(x, 0f, 0f);

        TextMeshPro text = columnObject.AddComponent<TextMeshPro>();
        text.font = font;
        text.fontSize = GlyphSize * 10f; // 3D TMP: 10 points span ~1 world unit
        text.alignment = TextAlignmentOptions.Top;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.richText = false;
        text.rectTransform.sizeDelta = new Vector2(GlyphSize * 2f, height);

        // The Default layer renders below everything else in the scene
        MeshRenderer meshRenderer = columnObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerID = 0;
        meshRenderer.sortingOrder = 0;

        Column column = new Column
        {
            text = text,
            glyphs = new char[rows],
            head = -Random.Range(0f, rows * 2f), // Stagger the initial drops
            speed = Random.Range(6f, 20f),
            trailLength = Random.Range(5, rows),
            mutateTimer = Random.Range(0f, 0.3f),
        };
        for (int i = 0; i < rows; i++)
        {
            column.glyphs[i] = Glyphs[Random.Range(0, Glyphs.Length)];
        }
        SetColumnText(column);
        ApplyColors(column); // Avoid a one-frame flash of uncolored glyphs
        return column;
    }

    private void AnimateColumn(Column column)
    {
        column.head += column.speed * Time.deltaTime;
        if (column.head - column.trailLength > rows)
        {
            // The whole trail left the screen; recycle with a random delay
            column.head = -Random.Range(0f, rows);
            column.speed = Random.Range(6f, 20f);
            column.trailLength = Random.Range(5, rows);
        }

        column.mutateTimer -= Time.deltaTime;
        if (column.mutateTimer <= 0f)
        {
            column.glyphs[Random.Range(0, rows)] = Glyphs[Random.Range(0, Glyphs.Length)];
            SetColumnText(column);
            column.mutateTimer = Random.Range(0.05f, 0.3f);
        }

        ApplyColors(column);
    }

    private void SetColumnText(Column column)
    {
        StringBuilder builder = new StringBuilder(rows * 2);
        for (int i = 0; i < rows; i++)
        {
            if (i > 0) builder.Append('\n');
            builder.Append(column.glyphs[i]);
        }
        column.text.SetText(builder);
        column.text.ForceMeshUpdate();
    }

    // Paint the head glyph near-white and fade the trail above it to
    // transparent by writing vertex colors directly (cheaper than rich text).
    private void ApplyColors(Column column)
    {
        TMP_TextInfo textInfo = column.text.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
            if (!characterInfo.isVisible) continue;

            float distanceBehindHead = column.head - characterInfo.lineNumber;
            Color32 color;
            if (distanceBehindHead < 0f)
            {
                color = new Color32(0, 0, 0, 0); // The drop has not reached this row yet
            }
            else if (distanceBehindHead < 1f)
            {
                color = HeadColor;
            }
            else
            {
                float fade = 1f - (distanceBehindHead - 1f) / column.trailLength;
                color = TrailColor;
                color.a = (byte)(255f * Mathf.Clamp01(fade));
            }

            Color32[] vertexColors = textInfo.meshInfo[characterInfo.materialReferenceIndex].colors32;
            for (int vertex = 0; vertex < 4; vertex++)
            {
                vertexColors[characterInfo.vertexIndex + vertex] = color;
            }
        }
        column.text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
