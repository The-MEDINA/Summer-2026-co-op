using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    #region SERIALIZED_VARIABLES
    [Header("Generics")]
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip death;
    [Header("Specifics")]
    [SerializeField] private AudioClip deathtouch;
    [SerializeField] private AudioClip heal;
    [SerializeField] private AudioClip equipment;
    #endregion

    public static SFXManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Make the SFX Manager listen to a card's events.
    /// </summary>
    /// <param name="card">Card to listen to.</param>
    public void RegisterCard(NewVirtualCardParent card)
    {
        // register a minion.
        if (card as MinionParent != null)
        {
            MinionParent minion = (MinionParent)card;
            minion.cardAction += MinionAction;
            minion.cardDeath += CardDeath;
        }
        // register a spell.
        else if (card as SpellParent != null)
        {
            SpellParent spell = (SpellParent)card;
            spell.cardAction += SpellAction;
        }
    }

    /// <summary>
    /// Make the SFX Manager no longer listen to a card's events.
    /// </summary>
    /// <param name="card">Card to stop listening to.</param>
    public void UnregisterCard(NewVirtualCardParent card)
    {
        // unregister a minion.
        if (card as MinionParent != null)
        {
            MinionParent minion = (MinionParent)card;
            minion.cardAction -= MinionAction;
            minion.cardDeath -= CardDeath;
        }
        // unregister a spell.
        else if (card as SpellParent != null)
        {
            SpellParent spell = (SpellParent)card;
            spell.cardAction -= SpellAction; 
        }
    }

    /// <summary>
    /// Play a sound on a minion's action.
    /// </summary>
    /// <param name="cardEffect">Effect of the minion.</param>
    private void MinionAction(MinionParent.effect cardEffect)
    {
        switch(cardEffect)
        {
            case MinionParent.effect.deathtouch:
            {
                SetChannel(deathtouch, 0.25f);
                break;
            }
            case MinionParent.effect.heal:
            {
                SetChannel(heal, 1);
                break;
            }
            default:
            {
                SetChannel(hit, 0.1f);
                break;
            }
        }
    }

    /// <summary>
    /// Play a sound on a spell's action.
    /// </summary>
    /// <param name="cardEffect">Effect of the spell.</param>
    private void SpellAction(SpellParent.spellEffect cardEffect)
    {
        switch (cardEffect)
        {
            case SpellParent.spellEffect.damage:
            {
                SetChannel(deathtouch, 0.25f);
                break;
            }
            case SpellParent.spellEffect.heal:
            {
                SetChannel(heal, 1);
                break;
            }
            case SpellParent.spellEffect.equipment:
            {
                SetChannel(equipment, 0.25f);
                break;
            }
            default:
            {
                break;
            }
        }
    }

    /// <summary>
    /// Play a sound on a card's death.
    /// </summary>
    /// <param name="faction">Faction of the card that died</param>
    private void CardDeath(string faction)
    {
        switch (faction)
        {
        default:
            {
                SetChannel(death, 1f);
                break;
            }
        }
    }

    /// <summary>
    /// Select an AudioSource to play a sound, or create a new one if all are currently being used.
    /// </summary>
    /// <param name="sound">Sound to play.</param>
    /// <param name="volume">Volume to play the sound at.</param>
    private void SetChannel(AudioClip sound, float volume)
    {
        AudioSource[] existingChannels = GetComponents<AudioSource>();
        for (int i = 0; i < existingChannels.Length; i++)
        {
            if (!existingChannels[i].isPlaying)
            {
                existingChannels[i].clip = sound;
                existingChannels[i].volume = volume;
                existingChannels[i].Play();
                return;
            }
        }
        AudioSource newChannel = this.gameObject.AddComponent<AudioSource>();
        newChannel.clip = sound;
        newChannel.volume = volume;
        newChannel.Play();
    }
}
