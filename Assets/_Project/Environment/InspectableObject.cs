using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InspectableObject : Interactable
{
    private GameObject inspectButton;

    void Start()
    {
        SetupInspectButton();
    }

    private void SetupInspectButton()
    {
        // Canvas canvas = GetComponent<Canvas>();
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
        // TODO: Open
    }
}