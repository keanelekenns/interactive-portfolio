using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.position = Input.mousePosition;
    }
}