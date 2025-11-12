using UnityEngine;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(TMP_Text))]
public class Section : Interactable
{
    public DisplaySection details;
    public MonoBehaviour informationModal;
    private GameObject inspectButton;

    void Start()
    {
        SetupTitle();
        SetupInspectButton();
    }

    void SetupTitle()
    {
        TMP_Text textMeshProText = GetComponent<TMP_Text>();
        textMeshProText.SetText(details.category);
        textMeshProText.fontSize = details.size;

        // Force a mesh update so bounds are correct
        textMeshProText.ForceMeshUpdate();

        // Fit the box collider to the text bounds
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = textMeshProText.bounds.size;
        collider.offset = textMeshProText.bounds.center;
    }

    private void SetupInspectButton()
    {
        Button button = GetComponentInChildren<Button>();
        inspectButton = button.gameObject;
        button.onClick.AddListener(Interact);
        inspectButton.SetActive(false);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // An Interactable can only be discovered by the player by default
        if (other.CompareTag("Player"))
        {
            // Display the "Inspect" button above the object
            inspectButton.SetActive(true);
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inspectButton.SetActive(false);
        }
    }


    public override bool CanInteract()
    {
        return inspectButton.activeInHierarchy;
    }

    public override void Interact()
    {
        if (!CanInteract())
        {
            Debug.Log("Inspect Button is not active!");
            return;
        }
        Debug.Log($"Inspecting: {name}");
        informationModal.gameObject.SetActive(true);
    }
}