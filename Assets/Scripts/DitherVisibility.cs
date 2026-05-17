using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DitherVisibility : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeSpeed = 3f;
    public float visibleAlpha = 1f;
    public float hiddenAlpha  = 0f;

    [Header("Persistence")]
    [Tooltip("Stealable objects: once the player sees it, it never hides again.")]
    public bool staysVisibleOnceSeen = false;
    [Tooltip("Enemies: stay on screen this many seconds after the player looks away.")]
    public float lingerTime = 0f;

    private Renderer[] _ditherRenderers;  // have _Base_Color → fade alpha
    private Renderer[] _toggleRenderers;  // no _Base_Color (particles, cone mesh) → enable/disable
    private MaterialPropertyBlock _propBlock;
    private float _targetAlpha;
    private float _currentAlpha;
    private Coroutine _fadeCoroutine;
    private bool _hasBeenSeen;

    private static readonly int ColorID = Shader.PropertyToID("_Base_Color");

    /// <summary>
    /// True while the player's vision cone currently contains this object.
    /// Single source of truth for "is the player looking at this right now" —
    /// read by enemy speed logic, proximity cues, etc.
    /// </summary>
    public bool IsVisible { get; private set; }

    private void Awake()
    {
        var all = GetComponentsInChildren<Renderer>();
        var dither = new List<Renderer>();
        var toggle = new List<Renderer>();

        foreach (Renderer r in all)
        {
            if (r.sharedMaterial != null && r.sharedMaterial.HasProperty(ColorID))
                dither.Add(r);
            else
                toggle.Add(r);
        }

        _ditherRenderers = dither.ToArray();
        _toggleRenderers = toggle.ToArray();
        _propBlock    = new MaterialPropertyBlock();
        _targetAlpha  = hiddenAlpha;
        _currentAlpha = hiddenAlpha;
        ApplyAlpha(_currentAlpha);
    }

    /// <summary>Called by PlayerVisionCone when visibility state changes.</summary>
    public void SetVisible(bool visible)
    {
        IsVisible = visible;

        if (visible)
            _hasBeenSeen = true;
        else if (staysVisibleOnceSeen && _hasBeenSeen)
            return;   // latched object — never hide again

        // Coroutines can't run on an inactive object — apply the change instantly instead.
        if (!isActiveAndEnabled)
        {
            _targetAlpha = _currentAlpha = visible ? visibleAlpha : hiddenAlpha;
            ApplyAlpha(_currentAlpha);
            return;
        }

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeRoutine(visible));
    }

    private IEnumerator FadeRoutine(bool visible)
    {
        // Enemies linger on screen briefly after the player looks away.
        if (!visible && lingerTime > 0f)
            yield return new WaitForSeconds(lingerTime);

        _targetAlpha = visible ? visibleAlpha : hiddenAlpha;

        while (!Mathf.Approximately(_currentAlpha, _targetAlpha))
        {
            _currentAlpha = Mathf.MoveTowards(_currentAlpha, _targetAlpha, fadeSpeed * Time.deltaTime);
            ApplyAlpha(_currentAlpha);
            yield return null;
        }
        _currentAlpha = _targetAlpha;
        ApplyAlpha(_currentAlpha);
    }

    private void ApplyAlpha(float alpha)
    {
        foreach (Renderer r in _ditherRenderers)
        {
            r.GetPropertyBlock(_propBlock);
            Color col = _propBlock.GetVector(ColorID);
            if (col == Color.clear)
                col = r.sharedMaterial.GetColor(ColorID);
            col.a = alpha;
            _propBlock.SetColor(ColorID, col);
            r.SetPropertyBlock(_propBlock);
        }

        bool show = alpha > 0f;
        foreach (Renderer r in _toggleRenderers)
            r.enabled = show;
    }
}
