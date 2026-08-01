using Network;
using UnityEngine;
using UnityEngine.EventSystems;

public class SeargentZoomieScript : CommanderCardScript, IPointerClickHandler
{
    private float timer = 0f;
    [SerializeField] private float timeToEffect = 12f;
    private bool canAttack = false;

    private void Start()
    {
        Name = "Seargent Zoomie";//assigns name
    }

    private void Update()
    {
        //controls effect timer
        if (timer > timeToEffect + FrozenTimeDelay)
        {
            canAttack = true;
            timer = 0f;
            FrozenTimeDelay = 0;
        }
        else if (timer < (timeToEffect + FrozenTimeDelay) && !canAttack)
        {
            timer += Time.deltaTime;
        }
        if (FrozenTimeDelay != 0)
        {
            canAttack = false;
        }
        Progressbar();
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
            if ((bg.P.IsPlayerTwo && CardSelectionManager.Instance.IsLocalTesting) || !bg.P.IsPlayerTwo)
            {
                if (canAttack) { PerformAbility(); }
            }
            else { Debug.LogWarning("Cannot interact with Player 2's commander."); }
        }
    }

    /// <summary>
    /// gives owner player +2 energy upon use
    /// </summary>
    public override void PerformAbility()
    {
        // Don't run if network manager is trying to resolve a desync.
        if (Networking.CurrentState == state.paused) return;

        //implement energy
        bg.P.Energy += 2;

        if(bg.P.Energy > 10)
        {
            bg.P.Energy = 10;
        }

        if (!bg.P.IsPlayerTwo)
        {
            Networking.SendCommanderAbility(0);
        }

        canAttack = false;
    }

    public override void Progressbar()
    {
        if (ClickHandler != null)
        {
            // Frozen
            if (FrozenTimeDelay != 0)
            {
                ClickHandler.AddProgress(5);
            }
            // Ready
            else if (canAttack)
            {
                ClickHandler.AddProgress(4);
            }
            // Calculate the progress otherwise
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if ((timer / timeToEffect) >= ((float)i / 4))
                    {
                        ClickHandler.AddProgress(i);
                    }
                }
            }
        }

        // find a deck card click handler if one wasn't already searched for
        if (!SearchedForClickHandler)
        {
            ClickHandler = gameObject.GetComponent<DeckCardClickHandler>();
            SearchedForClickHandler = true;
        }
    }
}
