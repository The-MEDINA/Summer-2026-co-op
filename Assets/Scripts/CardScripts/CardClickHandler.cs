using UnityEngine;
using Network;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum speed
    {
        normal,
        haste,
        sloth,
        frozen
    }

    [SerializeField] private float magnifiedScale = 1.6f;
    [SerializeField] private float selectedScale = 1.5f;
    [SerializeField] private float defaultScale = 0.2f;

    private Vector3 originalScale;
    private NewVirtualCardParent cardData;
    private Player ownerPlayer;

    private float timer = 0f;
    private float timeToAttack = 2f;
    private speed currentSpeed = speed.normal;
    private float freezeTimer = 0f;
    private float timeToUnfreeze = 10f;
    private bool inPlay = false;

    [SerializeField] private GameObject damagePopUpTextPrefab;

    public NewVirtualCardParent CardData { get { return cardData; } set { cardData = value; } }
    public Player OwnerPlayer { get { return ownerPlayer; } set { ownerPlayer = value; } }
    public bool InPlay { get { return inPlay; } set { inPlay = value; } }
    public speed CurrentSpeed { get { return currentSpeed; } }
   
    private void Awake()
    {
        originalScale = transform.localScale * defaultScale;
        transform.localScale = originalScale;
    }

    private void Start()
    {
        if (CardData is MinionParent)
        {
            MinionParent minion = (MinionParent)CardData;
            FindSpeed(minion.CardEffect);
            SetColor(minion.CardEffect);
        }
        else if (CardData is SpellParent) 
        {
            SpellParent spell = (SpellParent)CardData;
            SetColor(spell.Effect);
        }
    }

    private void Update()
    {
        if (CardData is MinionParent && inPlay)
        {
            MinionParent minion = (MinionParent)CardData;

            if (minion.IsDead)
            {
                gameObject.SetActive(false);
            }

            if(timer > timeToAttack)
            {
                Debug.Log(currentSpeed);
                minion.CanAttack = true;
                timer = 0f;
            }
            else if(currentSpeed == speed.frozen)
            {
                minion.CanAttack = false;
                timer = 0f;

                if(freezeTimer >= timeToUnfreeze)
                {
                    FindSpeed(minion.CardEffect);
                }
                else if (freezeTimer < timeToUnfreeze && !minion.CanAttack)
                {
                    freezeTimer += Time.deltaTime;
                }

            }
            else if (timer < timeToAttack && !minion.CanAttack)
            {
                timer += Time.deltaTime;
            }

            float timePercent = timer / timeToAttack;

            if (timePercent >= 1f)
            {
                CardData.UnityObject.GetComponent<CardUIManager>().AddProgress(4f);
            }
            else if (1f > timePercent && timePercent >= 0.75f)
            {
                CardData.UnityObject.GetComponent<CardUIManager>().AddProgress(3f);

            }
            else if (0.75f > timePercent && timePercent >= 0.5f)
            {
                CardData.UnityObject.GetComponent<CardUIManager>().AddProgress(2f);
            }
            else if (0.5f > timePercent && timePercent >= 0.25f)
            {
                CardData.UnityObject.GetComponent<CardUIManager>().AddProgress(1f);
            }
            else if (timePercent > 0f)
            {
                CardData.UnityObject.GetComponent<CardUIManager>().AddProgress(0f);
            }
        }
    }

    public void PopUpDamageText(int damage)
    {
        Vector2 spawnPos = new Vector2(transform.position.x + 0.1f * damage, transform.position.y + 1.0f);
        damagePopUpTextPrefab.GetComponent<DamagePUTextScript>().SetNumber(damage);
        GameObject instanceOfPopUpText = Instantiate(damagePopUpTextPrefab, spawnPos, Quaternion.identity);
    }

    public void FindSpeed(MinionParent.effect speed)
    {
        Debug.Log(currentSpeed);
        switch(speed)
        {
            case MinionParent.effect.haste:
                {
                    SetSpeed(CardClickHandler.speed.haste);
                    currentSpeed = CardClickHandler.speed.haste;
                    break;
                }

            case MinionParent.effect.sloth:
                {
                    SetSpeed(CardClickHandler.speed.sloth);
                    currentSpeed = CardClickHandler.speed.sloth;
                    break;
                }

            default:
                {
                    SetSpeed(CardClickHandler.speed.normal);
                    currentSpeed = CardClickHandler.speed.normal;
                    break;
                }
        }
        Debug.Log(currentSpeed);
    }

    public void SetSpeed(speed newSpeed)
    {
        Debug.Log(currentSpeed);
        switch (newSpeed)
        {
            default:
            case speed.normal:
                {
                    timeToAttack = 2f;
                    break;
                }

            case speed.haste:
                {
                    timeToAttack = 1f;
                    break;
                }

            case speed.sloth:
                {
                    timeToAttack = 4f;
                    break;
                }

            case speed.frozen:
                {
                    timeToAttack = 10f;
                    currentSpeed = speed.frozen;
                    if (cardData is MinionParent)
                    {
                        MinionParent minion = ( MinionParent)cardData;
                        minion.CanAttack = false;
                    }
                    break;
                }
        }
        Debug.Log(currentSpeed);

    }

    public void SetSpeedToFull() { timer = timeToAttack; }

    /// <summary>
    /// sets color of a minion card based on its ability
    /// </summary>
    /// <param name="ability">minion's ability</param>
    private void SetColor(MinionParent.effect ability)
    {
        switch (ability)
        {
            case MinionParent.effect.overkill:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.softRed;
                    break;
                }

            case MinionParent.effect.deathtouch:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.mediumVioletRed;
                    break;
                }

            case MinionParent.effect.explode:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.orangeRed;
                    break;
                }

            case MinionParent.effect.thorns:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.darkOrange;
                    break;
                }

            case MinionParent.effect.spawnToken:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.lawnGreen;
                    break;
                }

            case MinionParent.effect.guard:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.darkGreen;
                    break;
                }

            case MinionParent.effect.haste:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.lightBlue;
                    break;
                }

            case MinionParent.effect.sloth:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.deepSkyBlue;
                    break;
                }

            case MinionParent.effect.coordinate:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.purple;
                    break;
                }

            case MinionParent.effect.twoAttacks:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.rebeccaPurple;
                    break;
                }

            case MinionParent.effect.heal:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.lightPink;
                    break;
                }

            case MinionParent.effect.aoe:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.deepPink;
                    break;
                }

            case MinionParent.effect.none:
            default:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;
                }
        }
    }

    /// <summary>
    /// sets the color of a spell based on its ability
    /// </summary>
    /// <param name="ability">the spell's ability</param>
    private void SetColor(SpellParent.spellEffect ability)
    {
        switch (ability)
        {
            case SpellParent.spellEffect.damage:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.coral;                    
                    break;
                }

            case SpellParent.spellEffect.heal:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.hotPink;
                    break;
                }

            case SpellParent.spellEffect.unique:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.beige;
                    break;
                }

            case SpellParent.spellEffect.equipment:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.slateGray;
                    break;
                }

            case SpellParent.spellEffect.spawnTokens:
                {
                    CardData.UnityObject.GetComponent<SpriteRenderer>().color = Color.olive;
                    break;
                }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Don't run if network manager is trying to resolve a desync.
        if (Networking.CurrentState == state.paused) return;

        transform.localScale = originalScale * magnifiedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Don't run if network manager is trying to resolve a desync.
        if (Networking.CurrentState == state.paused) return;

        if (CardSelectionManager.Instance != null && CardSelectionManager.Instance.SelectedCardObject == this)
        {
            transform.localScale = originalScale * selectedScale;
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Don't run if network manager is trying to resolve a desync.
        if (Networking.CurrentState == state.paused) return;

        if (CardSelectionManager.Instance != null)
        {
            CardSelectionManager.Instance.SelectCard(this, eventData);
        }
    }

    public void SetSelectedVisual(bool selected)
    {
        transform.localScale = selected ? originalScale * selectedScale : originalScale;
    }

    /// <summary>
    /// resets a minion's attack timer
    /// </summary>
    public void ResetTimer() { timer = 0f; }
}