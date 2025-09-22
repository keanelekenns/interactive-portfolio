using UnityEngine;

public class JsonLoader : MonoBehaviour
{
    public Section sectionPrefab;
    public Transform center;
    public float radius = 7;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("DisplaySections");
        DisplaySectionContainer container = JsonUtility.FromJson<DisplaySectionContainer>(jsonFile.text);
        transform.position = center.position;
        SpawnTextInOval(container.sections, radius);
    }

    /// <summary>
    /// Spawns text prefabs in an oval.
    /// </summary>
    /// <param name="sections"></param>
    /// <param name="baseRadius">Radius of the long part of the oval.</param>
    void SpawnTextInOval(DisplaySection[] sections, float baseRadius)
    {
        int count = sections.Length;
        float angleStep = Mathf.PI * 2 / count; // Angle between each text

        for (int i = 0; i < count; i++)
        {
            DisplaySection ds = sections[i];
            float angle = angleStep * i;

            float x = Mathf.Cos(angle) * baseRadius;
            // Since text goes horizontally, reduce the relative vertical distance
            float y = Mathf.Sin(angle) * baseRadius * 0.5f;

            SpawnText(new Vector3(x, y, 0), ds.category, ds.size);
        }
    }

    /// <summary>
    /// Spawn the given text relative to the parent gameobject.
    /// </summary>
    /// <param name="position">Relative position to add to the new text.</param>
    /// <param name="message">What the text displays.</param>
    /// <param name="fontSize">How large the text is.</param>
    void SpawnText(Vector3 position, string message, float fontSize)
    {
        Section section = Instantiate(sectionPrefab, position, transform.rotation, transform);
        section.title = message;
        section.fontSize = fontSize;
    }
}
