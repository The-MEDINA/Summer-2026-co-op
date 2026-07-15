using UnityEngine;

public class SFXManager : MonoBehaviour
{
    #region SERIALIZED_VARIABLES
    [Header("Channel(s)")]
    [SerializeField] private AudioSource channelOne;
    [Header("Generics")]
    [SerializeField] private AudioClip hit;
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
        if (card as MinionParent != null)
        {
            MinionParent minion = (MinionParent)card;
            minion.cardAction += MinionAction;
        }
    }

    private void MinionAction(MinionParent.effect cardEffect)
    {
        switch(cardEffect)
        {
            default:
            {
                    channelOne.clip = hit;
                    channelOne.Play();
                break;
            }
        }
    }
}
