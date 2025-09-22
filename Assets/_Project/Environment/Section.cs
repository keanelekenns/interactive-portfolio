using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
[RequireComponent(typeof(BoxCollider2D))]
public class Section : MonoBehaviour
{
    public string title;
    public float fontSize;

    void Start()
    {
        TMP_Text textMeshProText = GetComponent<TMP_Text>();
        textMeshProText.SetText(title);
        textMeshProText.fontSize = fontSize;

        // Force a mesh update so bounds are correct
        textMeshProText.ForceMeshUpdate();

        // Fit the box collider to the text bounds
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = textMeshProText.bounds.size;
        collider.offset = textMeshProText.bounds.center;

        gameObject.name = title;

    }
}
