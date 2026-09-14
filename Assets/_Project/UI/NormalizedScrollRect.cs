using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A ScrollRect whose mouse-wheel scrolling feels the same on every platform.
///
/// Unity 2022 reports raw, platform-specific scroll deltas (e.g. ±120 per notch
/// on Windows, only a few units per notch in WebGL). ScrollRect multiplies that
/// raw delta by its scroll sensitivity, so a value tuned in the editor scrolls
/// painfully slowly in a WebGL build. Cross-platform normalization only exists
/// in the Input System on Unity 6+, so this class does it instead:
/// each scroll event is clamped to at most one notch per axis, which means
/// "Scroll Sensitivity" now simply means "pixels per wheel notch".
/// </summary>
[AddComponentMenu("UI/Normalized Scroll Rect")]
public class NormalizedScrollRect : ScrollRect
{
    [Tooltip("Scroll deltas at or above this magnitude count as one full wheel notch. " +
             "Smaller deltas (trackpads, smooth scrolling) are scaled proportionally.")]
    [SerializeField] private float fullNotchDelta = 6f;

    public override void OnScroll(PointerEventData data)
    {
        Vector2 delta = data.scrollDelta;
        data.scrollDelta = new Vector2(Normalize(delta.x), Normalize(delta.y));
        base.OnScroll(data);
    }

    private float Normalize(float delta)
    {
        if (fullNotchDelta <= 0f)
        {
            return Mathf.Sign(delta) * Mathf.Min(Mathf.Abs(delta), 1f);
        }
        return Mathf.Clamp(delta / fullNotchDelta, -1f, 1f);
    }
}
