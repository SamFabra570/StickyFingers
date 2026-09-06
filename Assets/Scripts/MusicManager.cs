using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    
    [Header("Normal Music")]
    [SerializeField] private AudioClip normalIntro;
    [SerializeField] private AudioClip normalTrack;

    [Header("Final Music")]
    [SerializeField] private AudioClip finalIntro;
    [SerializeField] private AudioClip finalTrack;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    private Coroutine musicRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        PlayNormalMusic();
    }

    public void PlayNormalMusic()
    {
        StartMusicSequence(normalIntro, normalTrack);
    }

    public void PlayFinalMusic()
    {
        StartMusicSequence(finalIntro, finalTrack);
    }

    private void StartMusicSequence(AudioClip intro, AudioClip track)
    {
        if (musicRoutine != null)
        {
            StopCoroutine(musicRoutine);
        }

        musicRoutine = StartCoroutine(MusicSequence(intro, track));
    }

    private IEnumerator MusicSequence(AudioClip intro, AudioClip track)
    {
        // Play intro
        audioSource.clip = intro;
        audioSource.Play();

        // Wait until intro finishes
        yield return new WaitForSeconds(intro.length);

        // Play main track
        audioSource.clip = track;
        audioSource.Play();
    }
}
