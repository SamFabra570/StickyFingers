using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to every enemy or stealable object.
/// Controls _Color.a on all renderers using the dither shader.
/// </summary>
public class DitherVisibility : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeSpeed = 3f;          // higher = faster lerp
    public float visibleAlpha   = 1f;
    public float hiddenAlpha    = 0f;

    private Renderer[] _renderers;
    private MaterialPropertyBlock _propBlock;
    private float _targetAlpha;
    private float _currentAlpha;
    private Coroutine _fadeCoroutine;

    private static readonly int ColorID = Shader.PropertyToID("_Base_Color");

    private void Awake()
    {
        _renderers  = GetComponentsInChildren<Renderer>();
        _propBlock  = new MaterialPropertyBlock();
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
        foreach (Renderer r in _renderers)
        {
            r.GetPropertyBlock(_propBlock);

            // Preserve existing color RGB, only change alpha
            Color col = _propBlock.GetVector(ColorID);    // returns (0,0,0,0) if unset
            if (col == Color.clear)
            {
                // Fall back to the material's base color if property block is fresh
                col = r.sharedMaterial.GetColor(ColorID);
            }
            col.a = alpha;
            _propBlock.SetColor(ColorID, col);
            r.SetPropertyBlock(_propBlock);
        }
    }
}
