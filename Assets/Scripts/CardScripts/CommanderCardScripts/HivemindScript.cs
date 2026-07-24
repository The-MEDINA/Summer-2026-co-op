using Network;
using UnityEngine;
using UnityEngine.EventSystems;

public class HivemindScript : CommanderCardScript, IPointerClickHandler
{
    private float timer = 0f;
    [SerializeField] private float timeToEffect = 3f;
    private bool canAttack = false;

    private void Start()
    {
        Name = "Hivemind";//assigns name
    }

    void Update()
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

    public override void PerformAbility()
    {
        // Don't run if network manager is trying to resolve a desync.
        if (Networking.CurrentState == state.paused) return;

        bg.P.Deck.Insert(0, new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.allyCards, 1, 1, 0, "Worms",
            NewVirtualCardParent.type.token, NewVirtualCardParent.location.deck));
        bg.DrawCardToHand();

        if (!bg.P.IsPlayerTwo)
        {
            Networking.SendCommanderAbility(0);
        }

        canAttack = false;
    }
}
