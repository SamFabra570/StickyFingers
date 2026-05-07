using UnityEngine;
using ZoneInteractables;

public class DanceFloorManager : MonoBehaviour
{
    [Header("Sequence")]
    [SerializeField] private int[] correctSequence;

    [Header("References")]
    [SerializeField] private ObjectSpawner rewardSpawner;
    [SerializeField] private GameObject hudSequencePanel;

    [Header("Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;

    private int _currentStep = 0;
    private bool _completed = false;

    private void Start()
    {
        if (hudSequencePanel != null)
            hudSequencePanel.SetActive(true);
    }

    public void OnArrowStepped(int index)
    {
        if (_completed) return;

        if (index == correctSequence[_currentStep])
        {
            _currentStep++;

            if (_currentStep >= correctSequence.Length)
                Complete();
        }
        else
        {
            ResetSequence();
            PlaySound(failSound);
        }
    }

    private void Complete()
    {
        _completed = true;
        PlaySound(successSound);

        if (hudSequencePanel != null)
            hudSequencePanel.SetActive(false);

        if (rewardSpawner != null)
            rewardSpawner.TriggerSpawn();
        else
            Debug.LogWarning("[DanceFloorManager] No reward spawner assigned.");
    }

    private void ResetSequence()
    {
        _currentStep = 0;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
