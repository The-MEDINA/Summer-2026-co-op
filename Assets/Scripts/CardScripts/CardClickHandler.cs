using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
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
            if(minion.CardEffect == MinionParent.effect.haste)
            {
                timeToAttack = 0.5f;
            }
            if (minion.CardEffect == MinionParent.effect.sloth)
            {
                timeToAttack = 4f;
            }
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

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * magnifiedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
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