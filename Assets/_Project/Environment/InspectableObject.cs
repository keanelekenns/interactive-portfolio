using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InspectableObject : Interactable
{
    private GameObject inspectButton;

    void Start()
    {
        // Create the button above the text object
        CreateInspectButton();
        inspectButton.SetActive(false); // hidden until player touches
    }

    private void CreateInspectButton()
    {
        // You can replace this with your own prefab if you want styling
        inspectButton = new GameObject("InspectButton");
        Button btn = inspectButton.AddComponent<Button>();
        TextMeshProUGUI btnText = inspectButton.AddComponent<TextMeshProUGUI>();
        btnText.text = "Inspect";

        // Position the button just above the text
        inspectButton.transform.SetParent(transform, false);
        inspectButton.transform.localPosition = Vector3.up * 2f;

        btn.onClick.AddListener(Interact);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{name} triggered by {other.name}");

        // An Interactable can only be discovered by the player by default
        if (other.CompareTag("Player"))
        {
            // Display the "Inspect" button above the object
            inspectButton.SetActive(true);
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"EXIT: {other.name}");
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