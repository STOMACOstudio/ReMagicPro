using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip buttonClick;
    public AudioClip cardPlay;
    public AudioClip playCreature;
    public AudioClip playArtifact;
    public AudioClip declareAttack;
    public AudioClip declareBlock;
    public AudioClip break_artifact;
    public AudioClip tap_for_mana;
    public AudioClip plague;
    public AudioClip dealDamage;
    public AudioClip impact;
    public AudioClip drink;
    public AudioClip miner;
    public AudioClip gain_life;
    public AudioClip victory;
    public AudioClip defeat;
    public AudioClip drawCard;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
