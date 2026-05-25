using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float magnifiedScale = 1.3f;
    [SerializeField] private float selectedScale = 1.15f;
    [SerializeField] private bool isEnemyCard = false;

    private Vector3 originalScale;
    private NewVirtualCardParent cardData;

    [SerializeField] private float timeToAttack = 3f;
    private float attackTimer = 0f;

    public bool IsEnemyCard { get { return isEnemyCard; } }
    public NewVirtualCardParent CardData { get { return cardData; } set { cardData = value; } }

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        if (CardData is MinionParent && IsEnemyCard == false)
        {
            MinionParent MinionData = (MinionParent)CardData;
            if (MinionData.CardEffect == MinionParent.effect.haste) { timeToAttack /= 2; }
            else if (MinionData.CardEffect == MinionParent.effect.sloth) { timeToAttack *= 2; }
        }
    }

    private void Update()
    {
        if (CardData is MinionParent)
        {
            MinionParent MinionData = (MinionParent)CardData;

            if (MinionData != null && MinionData.IsDead)
            {
                gameObject.SetActive(false);
            }

            attackTimer += Time.deltaTime;
            if (attackTimer >= timeToAttack)
            {
                attackTimer = 0f;
                MinionData.CanAttack = true;
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