using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D linkCursor;
    public Vector2 hotspot = Vector2.zero;

    public static CursorManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        ResetCursor();
    }

    public void SetLinkCursor()
    {
        Cursor.SetCursor(linkCursor, hotspot, CursorMode.Auto);
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }
}