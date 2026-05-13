using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private CardParent card;

    private bool isSelected = false;
    private bool isHolding = false;

    private Vector3 originalScale;

    [SerializeField] private float magnifiedScale = 1.3f;

    private void Awake()
    {
        card = GetComponent<CardParent>();
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        MagnifyCard();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        ShrinkCard();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelected)
        {
            SelectCard();
        }
        else
        {
            ActivateCard();
        }
    }

    private void MagnifyCard()
    {
        transform.localScale = originalScale * magnifiedScale;
        Debug.Log("Magnifying card");
    }

    private void ShrinkCard()
    {
        transform.localScale = originalScale;
        Debug.Log("Shrinking card");
    }

    private void SelectCard()
    {
        isSelected = true;
        Debug.Log("Card selected: " + gameObject.name);
    }

    private void ActivateCard()
    {
        isSelected = false;
        Debug.Log("Card activated: " + gameObject.name);
        //this can call card.OnPlay() or card.Attack() - Can be done later
    }
}