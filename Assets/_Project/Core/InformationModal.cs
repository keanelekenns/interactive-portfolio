using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationModal : MonoBehaviour
{
    [Header("Prefabs")]
    public TextMeshProUGUI textPrefab;
    public Image imagePrefab;

    private Transform contentParent;

    private void Awake()
    {
        // Find the Content container
        contentParent = transform.Find("Scroll View/Viewport/Content");

        if (contentParent == null)
        {
            Debug.LogError("Could not find expected 'Content' child component.");
        }

        Button exitButton = GetComponentInChildren<Button>();
        exitButton.onClick.AddListener(ExitModal);
    }

    private void OnEnable()
    {
        InputManager.InputActions.Camera.Disable();
        InputManager.InputActions.Player.Disable();
        InputManager.InputActions.UI.Enable();
    }

    private void OnDisable()
    {
        InputManager.InputActions.UI.Disable();
        InputManager.InputActions.Player.Enable();
        InputManager.InputActions.Camera.Enable();
    }

    private void ExitModal()
    {
        gameObject.SetActive(false);
    }

    public void AddContents(List<Content> contents)
    {
        foreach (Content content in contents)
        {
            AddContent(content);
        }
    }

    public void AddContent(Content content)
    {
        if (contentParent == null)
        {
            Debug.LogError("Content parent is missing.");
            return;
        }

        if (content.type == "text")
        {
            AddText(content.value);
        }
        else if (content.type == "image")
        {
            AddImage(content.value);
        }
        else
        {
            Debug.LogWarning($"Unknown content type: {content.type}");
        }
    }

    private void AddText(string textValue)
    {
        TextMeshProUGUI textObj = Instantiate(textPrefab, contentParent);
        textObj.text = textValue;
        textObj.gameObject.SetActive(true);
    }

    private void AddImage(string imagePath)
    {
        // Load the sprite — assumes image is inside a Resources folder
        Sprite sprite = Resources.Load<Sprite>(imagePath);

        if (sprite == null)
        {
            Debug.LogError($"Could not load sprite at path: {imagePath}");
            return;
        }

        Image imgObj = Instantiate(imagePrefab, contentParent);
        imgObj.sprite = sprite;
        imgObj.SetNativeSize();
        imgObj.gameObject.SetActive(true);
    }

}
