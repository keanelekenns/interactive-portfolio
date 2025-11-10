using UnityEngine;

public class JsonLoader : MonoBehaviour
{
    public Section sectionPrefab;
    public InformationModal modalPrefab;
    public Transform center;
    public float radius = 7;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("DisplaySections");
        DisplaySectionContainer container = JsonUtility.FromJson<DisplaySectionContainer>(jsonFile.text);
        transform.position = center.position;
        SpawnSectionInOval(container.sections, radius);
    }

    /// <summary>
    /// Spawns text prefabs in an oval.
    /// </summary>
    /// <param name="sections"></param>
    /// <param name="baseRadius">Radius of the long part of the oval.</param>
    void SpawnSectionInOval(DisplaySection[] sections, float baseRadius)
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

            SpawnSection(new Vector3(x, y, 0), ds);
        }
    }

    /// <summary>
    /// Spawn the given text relative to the parent gameobject.
    /// </summary>
    /// <param name="position">Relative position to add to the new text.</param>
    /// <param name="values">The values for the section being spawned</param>
    void SpawnSection(Vector3 position, DisplaySection values)
    {
        Section section = Instantiate(sectionPrefab, position, transform.rotation, transform);
        section.details = values;
        InformationModal modal = Instantiate(modalPrefab, transform);
        modal.gameObject.SetActive(false);
        section.informationModal = modal;

        section.gameObject.name = values.category;
        modal.gameObject.name = values.category + " Modal";
    }
}
