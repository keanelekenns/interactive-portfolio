using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D linkCursor;
    public Vector2 defaultHotspot;
    public Vector2 linkHotspot;

    public static CursorManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        ResetCursor();
    }

    public void SetLinkCursor()
    {
        Cursor.SetCursor(linkCursor, linkHotspot, CursorMode.Auto);
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(defaultCursor, defaultHotspot, CursorMode.Auto);
    }
}