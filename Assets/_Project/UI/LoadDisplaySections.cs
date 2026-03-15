using UnityEngine;

public class JsonLoader : MonoBehaviour
{
    public Section sectionPrefab;
    public InformationModal modalPrefab;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("DisplaySections");
        DisplaySectionContainer container = JsonUtility.FromJson<DisplaySectionContainer>(jsonFile.text);
        foreach (var sectionDetails in container.sections)
        {
            SpawnSection(sectionDetails);
        }
    }

    /// <summary>
    /// Spawn the given text relative to the parent gameobject.
    /// </summary>
    /// <param name="position">Relative position to add to the new text.</param>
    /// <param name="details">The values for the section being spawned</param>
    void SpawnSection(DisplaySection details)
    {
        Section section = Instantiate(
            sectionPrefab,
            new Vector2(details.location.x, details.location.y),
            Quaternion.identity,
            new InstantiateParameters() { parent = transform, worldSpace = true }
        );
        section.details = details;
        InformationModal modal = Instantiate(modalPrefab, transform);
        modal.gameObject.SetActive(false);
        section.informationModal = modal;

        section.gameObject.name = details.title;
        modal.gameObject.name = details.title + " Modal";

        modal.AddContents(details.contents);
    }
}
