using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Toggles "matrix mode": the world dissolves into streaming green glyphs,
// the title corrupts into "Keanu Enns/Reeves", the music changes, and the
// player gets a new look. Triggered by the "???" section via SectionFunctions
// (content type "function" in DisplaySections.json).
public class MatrixModeController : MonoBehaviour
{
    [Tooltip("Music for matrix mode. Leave empty to use a loop synthesized at runtime. Only assign audio you have the rights to redistribute; the actual Matrix soundtrack is copyrighted.")]
    public AudioClip matrixMusic;
    public float matrixMusicVolume = 0.2f;
    [Tooltip("Optional replacement look for the player (e.g. sunglasses and black coat animations). When empty, the current sprites are tinted with playerTint instead.")]
    public RuntimeAnimatorController matrixPlayerLook;
    public Color playerTint = new Color(0.6f, 1f, 0.7f);

    // "Enns" fades and de-rezzes: dim desaturated green with the letters
    // jittering out of alignment, as if the name is corrupting
    private const string MatrixTitle =
        "Keanu <color=#5E8F6E96>E<voffset=-0.1em>n</voffset><voffset=0.08em>n</voffset><voffset=-0.05em>s</voffset></color>";
    private const string ActiveSectionTitle = "Wake Up";
    private static readonly Color MatrixGreen = new Color32(0, 255, 65, 255);

    private bool active;
    private MatrixRain rain;
    private TextMeshPro reeves;

    // Scene objects looked up on first toggle
    private TMP_Text title;
    private AudioSource music;
    private PlayerController player;

    // Original state restored when leaving matrix mode
    private string originalTitle;
    private Color originalBackground;
    private AudioClip originalMusic;
    private float originalMusicVolume;
    private RuntimeAnimatorController originalPlayerLook;
    private Vector2 playerPositionOnEntry;
    private readonly List<Renderer> hiddenRenderers = new();
    private readonly List<Collider2D> disabledColliders = new();
    private readonly List<AudioSource> pausedAudioSources = new();
    private readonly Dictionary<SpriteRenderer, Color> originalPlayerColors = new();
    private AudioClip generatedMusic;

    public void Toggle(Section source)
    {
        if (active)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }

        if (source != null)
        {
            TMP_Text sectionLabel = source.GetComponentInChildren<TMP_Text>();
            sectionLabel.SetText(active ? ActiveSectionTitle : source.details.title);
        }
        AudioManager.Instance.PlaySfx(active ? "clickIn" : "clickOut");
    }

    private void Activate()
    {
        Camera cam = Camera.main;
        FindSceneObjects();

        rain = new GameObject("MatrixRain").AddComponent<MatrixRain>();
        rain.Initialize(cam, title != null ? title.font : null);

        ApplyMatrixTitle();
        HideWorldRenderers();
        DisableWorldColliders();
        PauseWorldAudio();

        originalBackground = cam.backgroundColor;
        cam.backgroundColor = Color.black;

        if (music != null)
        {
            originalMusic = music.clip;
            originalMusicVolume = music.volume;
            if (matrixMusic == null && generatedMusic == null)
            {
                generatedMusic = MatrixMusic.CreateLoop();
            }
            music.clip = matrixMusic != null ? matrixMusic : generatedMusic;
            music.volume = matrixMusicVolume;
            music.Play();
        }

        ApplyPlayerLook();
        active = true;
    }

    private void Deactivate()
    {
        if (rain != null)
        {
            Destroy(rain.gameObject);
        }

        foreach (Renderer hiddenRenderer in hiddenRenderers)
        {
            if (hiddenRenderer != null)
            {
                hiddenRenderer.enabled = true;
            }
        }
        hiddenRenderers.Clear();

        foreach (Collider2D disabledCollider in disabledColliders)
        {
            if (disabledCollider != null)
            {
                disabledCollider.enabled = true;
            }
        }
        disabledColliders.Clear();

        foreach (AudioSource pausedSource in pausedAudioSources)
        {
            if (pausedSource != null)
            {
                pausedSource.UnPause();
            }
        }
        pausedAudioSources.Clear();

        Camera.main.backgroundColor = originalBackground;

        RestoreTitle();

        if (music != null)
        {
            music.clip = originalMusic;
            music.volume = originalMusicVolume;
            music.Play();
        }

        RestorePlayerLook();
        active = false;
    }

    private void FindSceneObjects()
    {
        if (title == null)
        {
            // The world-space TMP displaying "Keanu Enns"
            GameObject titleObject = GameObject.Find("Name");
            title = titleObject != null ? titleObject.GetComponent<TMP_Text>() : null;
        }
        if (music == null)
        {
            GameObject musicPlayer = GameObject.Find("MusicPlayer");
            music = musicPlayer != null ? musicPlayer.GetComponent<AudioSource>() : null;
        }
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }
    }

    private void ApplyMatrixTitle()
    {
        if (title == null) return;

        originalTitle = title.text;
        title.text = MatrixTitle;
        title.ForceMeshUpdate();
        CreateReevesStamp();
    }

    // "Reeves" is stamped slightly askew, above and to the right of "Enns"
    private void CreateReevesStamp()
    {
        // Bounds of the last four visible characters ("Enns") in the title's
        // local space
        TMP_TextInfo textInfo = title.textInfo;
        Vector3 topLeft = Vector3.zero;
        Vector3 topRight = Vector3.zero;
        float top = float.MinValue;
        int lettersFound = 0;
        for (int i = textInfo.characterCount - 1; i >= 0 && lettersFound < 4; i--)
        {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
            if (!characterInfo.isVisible) continue;
            if (lettersFound == 0)
            {
                topRight = characterInfo.topRight;
            }
            topLeft = characterInfo.topLeft;
            top = Mathf.Max(top, Mathf.Max(characterInfo.topLeft.y, characterInfo.topRight.y));
            lettersFound++;
        }
        if (lettersFound == 0) return;

        GameObject reevesObject = new GameObject("Reeves");
        reevesObject.transform.SetParent(title.transform, false);

        reeves = reevesObject.AddComponent<TextMeshPro>();
        reeves.text = "Reeves";
        reeves.font = title.font;
        reeves.fontSharedMaterial = title.fontSharedMaterial;
        reeves.fontSize = title.fontSize * 0.75f;
        reeves.fontStyle = title.fontStyle;
        reeves.color = MatrixGreen;
        reeves.alignment = TextAlignmentOptions.Center;
        reeves.enableWordWrapping = false;
        reeves.rectTransform.sizeDelta = new Vector2(2f, 2f);

        float ennsCenterX = (topLeft.x + topRight.x) / 2f;
        float ennsWidth = topRight.x - topLeft.x;
        float em = title.fontSize / 10f; // 3D TMP: 10 points span ~1 world unit
        reevesObject.transform.localPosition = new Vector3(ennsCenterX + ennsWidth * 0.4f, top + em * 0.45f, 0f);
        reevesObject.transform.localRotation = Quaternion.Euler(0f, 0f, 8f);

        MeshRenderer titleRenderer = title.GetComponent<MeshRenderer>();
        MeshRenderer reevesRenderer = reevesObject.GetComponent<MeshRenderer>();
        if (titleRenderer != null)
        {
            reevesRenderer.sortingLayerID = titleRenderer.sortingLayerID;
            reevesRenderer.sortingOrder = titleRenderer.sortingOrder + 1;
        }
    }

    private void RestoreTitle()
    {
        if (title == null) return;

        title.text = originalTitle;
        if (reeves != null)
        {
            Destroy(reeves.gameObject);
        }
    }

    // Hide the environment so only the rain, the player, the title, and the
    // world-space UI remain
    private void HideWorldRenderers()
    {
        hiddenRenderers.Clear();
        foreach (Renderer sceneRenderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            if (!sceneRenderer.enabled) continue;
            if (sceneRenderer.GetComponentInParent<PlayerController>() != null) continue;
            if (sceneRenderer.GetComponentInParent<MatrixRain>() != null) continue;
            if (title != null && sceneRenderer.transform.IsChildOf(title.transform)) continue;

            sceneRenderer.enabled = false;
            hiddenRenderers.Add(sceneRenderer);
        }
    }

    // Disable collision with the now-invisible world. The player's own
    // colliders stay on. Because the map borders are disabled too, the player
    // is returned to where they jacked in when matrix mode ends.
    private void DisableWorldColliders()
    {
        if (player != null)
        {
            playerPositionOnEntry = player.transform.position;
        }

        disabledColliders.Clear();
        foreach (Collider2D sceneCollider in FindObjectsByType<Collider2D>(FindObjectsSortMode.None))
        {
            if (!sceneCollider.enabled) continue;
            if (sceneCollider.GetComponentInParent<PlayerController>() != null) continue;

            sceneCollider.enabled = false;
            disabledColliders.Add(sceneCollider);
        }
    }

    // Pause ambient sounds like the campfire; music and UI sfx keep playing
    private void PauseWorldAudio()
    {
        pausedAudioSources.Clear();
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (!source.isPlaying) continue;
            if (source == music) continue;
            if (AudioManager.Instance != null && source.gameObject == AudioManager.Instance.gameObject) continue;

            source.Pause();
            pausedAudioSources.Add(source);
        }
    }

    private void ApplyPlayerLook()
    {
        if (player == null) return;

        Animator animator = player.GetComponentInChildren<Animator>();
        if (matrixPlayerLook != null && animator != null)
        {
            originalPlayerLook = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = matrixPlayerLook;
            return;
        }

        originalPlayerColors.Clear();
        foreach (SpriteRenderer spriteRenderer in player.GetComponentsInChildren<SpriteRenderer>())
        {
            originalPlayerColors[spriteRenderer] = spriteRenderer.color;
            spriteRenderer.color = playerTint;
        }
    }

    private void RestorePlayerLook()
    {
        if (player == null) return;

        // Wake up where the body was left
        Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
        if (playerBody != null)
        {
            playerBody.position = playerPositionOnEntry;
        }
        else
        {
            player.transform.position = playerPositionOnEntry;
        }

        if (originalPlayerLook != null)
        {
            Animator animator = player.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = originalPlayerLook;
            }
            originalPlayerLook = null;
        }

        foreach (KeyValuePair<SpriteRenderer, Color> entry in originalPlayerColors)
        {
            if (entry.Key != null)
            {
                entry.Key.color = entry.Value;
            }
        }
        originalPlayerColors.Clear();
    }
}
