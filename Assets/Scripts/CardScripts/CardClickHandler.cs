using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float magnifiedScale = 1.3f;
    [SerializeField] private float selectedScale = 1.15f;
    [SerializeField] private bool isEnemyCard = false;

    private Vector3 originalScale;
    private CardParent cardData;

    [SerializeField] private float timeToAttack = 3f;
    private float attackTimer = 0f;

    public bool IsEnemyCard { get { return isEnemyCard; } }
    public CardParent CardData { get { return cardData; } set { cardData = value; } }

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        Debug.Log(CardData.Cost);
        if (CardData.CardEffect == CardParent.effect.haste) { timeToAttack /= 2; }
        else if (CardData.CardEffect == CardParent.effect.sloth) { timeToAttack *= 2; }
    }

    private void Update()
    {
        if (CardData != null && CardData.IsDead)
        {
            gameObject.SetActive(false);
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= timeToAttack)
        {
            attackTimer = 0f;
            CardData.CanAttack = true;
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
            CardSelectionManager.Instance.SelectCard(this);
        }
    }

    public void SetSelectedVisual(bool selected)
    {
        if (selected)
        {
            transform.localScale = originalScale * selectedScale;
        }
        else
        {
            transform.localScale = originalScale;
        }
    }
}