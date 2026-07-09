using Network;
using UnityEngine;
using UnityEngine.EventSystems;

public class MajorMunchkinScript : CommanderCardScript, IPointerClickHandler
{
    [SerializeField] private GameObject tokenPrefab; //cardPrefab
    private float timer = 0f;
    [SerializeField] private float timeToEffect = 3f;
    private bool canAttack = false;

    public GameObject TokenPrefab { get { return tokenPrefab; } set { tokenPrefab = value; } }

    private void Start()
    {
        Name = "Major Munchkin";//assigns name
    }

    private void Update()
    {
        //controls effect timer
        if (timer > timeToEffect)
        {
            canAttack = true;
            timer = 0f;
        }
        else if (timer < timeToEffect && !canAttack)
        {
            timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// calls PerformAbility() on mouse click
    /// </summary>
    /// <param name="eventData">data for mouse pointer click</param>
    public void OnPointerClick(PointerEventData eventData) 
    {
        if (!DeckbuilderOverride) // need this to prevent this method from activating in the deck builder
        {
            //when testing locally, enable bool isLocalTesting in inspector on CardSelectionManager.Ins, when playing online, disable it - Jacob
            if (!bg.P.IsPlayerTwo || CardSelectionManager.Instance.IsLocalTesting)
            {
                if (canAttack) { PerformAbility(); }
            }
            else { Debug.LogWarning("Cannot interact with Player 2's commander."); }
        }
    }

    /// <summary>
    /// spawns two token creatures
    /// </summary>
    public override void PerformAbility()
    {
        // Don't run if network manager is trying to resolve a desync.
        if (Networking.CurrentState == state.paused) return;

        bg.SpawnCardToInPlay(cardIndex.Index.CreateCard("Kitten", NewVirtualCardParent.location.inPlay));
        bg.SpawnCardToInPlay(cardIndex.Index.CreateCard("Kitten", NewVirtualCardParent.location.inPlay));

        if (!bg.P.IsPlayerTwo)
        {
            // this should really be its own packet and not sendCardAdd.
            // this should work for now though. - Dave
            Networking.SendCardAdd(cardIndex.Index.CreateCard("Kitten", NewVirtualCardParent.location.inPlay), NewVirtualCardParent.location.inPlay);
            Networking.SendCardArray(bg.P.InPlay, NewVirtualCardParent.location.inPlay);
        }
        canAttack = false;
    }
}
