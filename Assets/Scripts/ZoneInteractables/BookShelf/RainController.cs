using UnityEngine;

public class RainController : MonoBehaviour
{
    public static RainController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private ParticleSystem rainParticles;

    public bool IsRaining { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartRain()
    {
        IsRaining = true;
        if (rainParticles != null)
            rainParticles.Play();
    }

    public void StopRain()
    {
        IsRaining = false;
        if (rainParticles != null)
            rainParticles.Stop();
    }
}
