using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float magnifiedScale = 1.3f;
    [SerializeField] private float selectedScale = 1.15f;

    private Vector3 originalScale;
    private CardParent cardData;

    public CardParent CardData
    {
        get { return cardData; }
        set { cardData = value; }
    }

    private void Awake()
    {
        originalScale = transform.localScale;
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