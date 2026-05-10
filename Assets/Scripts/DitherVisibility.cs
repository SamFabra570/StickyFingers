using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DitherVisibility : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeSpeed = 3f;
    public float visibleAlpha = 1f;
    public float hiddenAlpha  = 0f;

    private Renderer[] _ditherRenderers;  // have _Base_Color → fade alpha
    private Renderer[] _toggleRenderers;  // no _Base_Color (particles, cone mesh) → enable/disable
    private MaterialPropertyBlock _propBlock;
    private float _targetAlpha;
    private float _currentAlpha;
    private Coroutine _fadeCoroutine;

    private static readonly int ColorID = Shader.PropertyToID("_Base_Color");

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
        _targetAlpha = visible ? visibleAlpha : hiddenAlpha;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
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
