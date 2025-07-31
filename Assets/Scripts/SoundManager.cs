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
    public AudioClip turnChange;
    public AudioClip equipArtifact;
    public AudioClip buyCard;

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

        // Automatically load clips if they were not assigned in the inspector.
        if (buttonClick == null)      buttonClick = Resources.Load<AudioClip>("Audio/SFX/click");
        if (cardPlay == null)         cardPlay = Resources.Load<AudioClip>("Audio/SFX/play");
        if (playCreature == null)     playCreature = Resources.Load<AudioClip>("Audio/SFX/play_creature");
        if (playArtifact == null)     playArtifact = Resources.Load<AudioClip>("Audio/SFX/play_artifact");
        if (declareAttack == null)    declareAttack = Resources.Load<AudioClip>("Audio/SFX/attack");
        if (declareBlock == null)     declareBlock = Resources.Load<AudioClip>("Audio/SFX/block_combat");
        if (break_artifact == null)   break_artifact = Resources.Load<AudioClip>("Audio/SFX/break_artifact");
        if (tap_for_mana == null)     tap_for_mana = Resources.Load<AudioClip>("Audio/SFX/tap_for_mana");
        if (plague == null)           plague = Resources.Load<AudioClip>("Audio/SFX/plague");
        if (dealDamage == null)       dealDamage = Resources.Load<AudioClip>("Audio/SFX/snd_damage_low");
        if (impact == null)           impact = Resources.Load<AudioClip>("Audio/SFX/impact");
        if (drink == null)            drink = Resources.Load<AudioClip>("Audio/SFX/drink");
        if (miner == null)            miner = Resources.Load<AudioClip>("Audio/SFX/miner");
        if (gain_life == null)        gain_life = Resources.Load<AudioClip>("Audio/SFX/heal");
        if (victory == null)          victory = Resources.Load<AudioClip>("Audio/SFX/Victory");
        if (defeat == null)           defeat = Resources.Load<AudioClip>("Audio/SFX/Defeat");
        if (drawCard == null)         drawCard = Resources.Load<AudioClip>("Audio/SFX/draw_card");
        if (turnChange == null)       turnChange = Resources.Load<AudioClip>("Audio/SFX/nextturn_sfx");
        if (equipArtifact == null)    equipArtifact = Resources.Load<AudioClip>("Audio/SFX/equip_sfx");
        if (buyCard == null)          buyCard = Resources.Load<AudioClip>("Audio/SFX/buy_sfx");
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
