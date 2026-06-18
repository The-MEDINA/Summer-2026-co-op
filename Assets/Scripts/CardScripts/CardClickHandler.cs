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

    [SerializeField] private float magnifiedScale = 1.3f;
    [SerializeField] private float selectedScale = 1.15f;

    private Vector3 originalScale;
    private NewVirtualCardParent cardData;
    private Player ownerPlayer;

    private float timer = 0f;
    private float timeToAttack = 2f;

    public NewVirtualCardParent CardData { get { return cardData; } set { cardData = value; } }
    public Player OwnerPlayer { get { return ownerPlayer; } set { ownerPlayer = value; } }

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        if (CardData is MinionParent)
        {
            MinionParent minion = (MinionParent)CardData;
            if (minion.CardEffect == MinionParent.effect.haste)
            {
                SetSpeed(speed.haste);
            }
            else if (minion.CardEffect == MinionParent.effect.sloth)
            {
                SetSpeed(speed.sloth);
            }
            else
            {
                SetSpeed(speed.normal);
            }

            SetColor(minion.CardEffect);
        }
    }

    private void Update()
    {
        if (CardData is MinionParent)
        {
            MinionParent minion = (MinionParent)CardData;

            if (minion.IsDead)
            {
                gameObject.SetActive(false);
            }

            if(timer > timeToAttack)
            {
                minion.CanAttack = true;
                timer = 0f;
            }
            else if (timer < timeToAttack && !minion.CanAttack)
            {
                timer += Time.deltaTime;
            }
        }
    }

    public void SetSpeed(speed newSpeed)
    {
        switch(newSpeed)
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
                    timeToAttack = 9999999f;
                    if (cardData is MinionParent)
                    {
                        MinionParent minion = ( MinionParent)cardData;
                        minion.CanAttack = false;
                    }
                    break;
                }
        }
    }
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
}