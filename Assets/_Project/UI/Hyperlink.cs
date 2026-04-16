using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class Hyperlink : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    private TMP_Text tmp;
    private bool isPointer;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GetLinkInfo(eventData) is TMP_LinkInfo linkInfo)
        {
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CheckLink(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDefaultCursor();
    }

    void CheckLink(PointerEventData eventData)
    {
        if (GetLinkInfo(eventData) != null)
        {
            if (!isPointer)
            {
                CursorManager.Instance.SetLinkCursor();
                isPointer = true;
            }
        }
        else
        {
            SetDefaultCursor();
        }
    }

    void SetDefaultCursor()
    {
        if (isPointer)
        {
            CursorManager.Instance.ResetCursor();
            isPointer = false;
        }
    }

    private TMP_LinkInfo? GetLinkInfo(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            tmp,
            eventData.position,
            null
        );

        if (linkIndex != -1)
        {
            return tmp.textInfo.linkInfo[linkIndex];
        }
        return null;
    }
}