using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Section : MonoBehaviour
{
    public DisplaySection details;
    public MonoBehaviour informationModal;

    void Start()
    {
        SetupTitle();
        SetupButton();
    }

    void SetupTitle()
    {
        TMP_Text textMeshProText = GetComponentInChildren<TMP_Text>();
        textMeshProText.SetText(details.title);
        textMeshProText.fontSize = details.size;

        // Force a mesh update so bounds are correct
        textMeshProText.ForceMeshUpdate();
    }

    private void SetupButton()
    {
        Button button = GetComponentInChildren<Button>();
        button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        bool ranFunction = false;
        foreach (Content content in details.contents)
        {
            if (content.type == "function")
            {
                SectionFunctions.Instance.Call(content.value, this);
                ranFunction = true;
            }
        }

        if (!ranFunction)
        {
            OpenModal();
        }
    }

    public void OpenModal()
    {
        informationModal.gameObject.SetActive(true);
        AudioManager.Instance.PlaySfx("clickIn");
    }
}