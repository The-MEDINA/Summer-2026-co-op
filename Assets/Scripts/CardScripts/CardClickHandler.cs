using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float magnifiedScale = 1.3f;
    [SerializeField] private float selectedScale = 1.15f;

    private Vector3 originalScale;
    private CardParent cardData;

    //test var
    [SerializeField] private bool isEnemyCard = false;
    //test property
    public bool IsEnemyCard { get { return isEnemyCard; } }

    public CardParent CardData { get { return cardData; } set { cardData = value; } }

    private void Awake()
    {
        originalScale = transform.localScale;
        //this carddata shouldn't be made here but for testing purposes I have it standerdised - Jake
        //CardData = new CardParent(3, 20, 4, CardParent.type.minion, CardParent.effect.none, CardParent.location.hand);
    }

    void Update()
    {
        if (CardData.IsDead) { gameObject.SetActive(false); }
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