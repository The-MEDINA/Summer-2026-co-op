using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    #region SERIALIZED_VARIABLES
    [Header("Generics")]
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip death;
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

    public void RegisterCard(NewVirtualCardParent card)
    {
        // register a minion.
        if (card as MinionParent != null)
        {
            MinionParent minion = (MinionParent)card;
            minion.cardAction += MinionAction;
            minion.cardDeath += CardDeath;
        }
    }

    public void UnregisterCard(NewVirtualCardParent card)
    {
        // unregister a minion.
        if (card as MinionParent != null)
        {
            MinionParent minion = (MinionParent)card;
            minion.cardAction -= MinionAction;
            minion.cardDeath -= CardDeath;
        }
    }

    private void MinionAction(MinionParent.effect cardEffect)
    {
        switch(cardEffect)
        {
            default:
            {
                SetChannel(hit, 0.5f);
                break;
            }
        }
    }
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
