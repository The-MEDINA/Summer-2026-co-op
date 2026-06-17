using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DBButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string cardName = "Cat";
    [SerializeField] private bool isAdding = true;
    [SerializeField] private TMP_Text numCardsText;
    [SerializeField] private TMP_Text cardStatText;

    private void Start()
    {
        MinionParent thisMinion = new MinionParent(cardName, NewVirtualCardParent.location.deck);
        cardStatText.text = $"Name: {thisMinion.CardName}\nCost: {thisMinion.Cost}\nHealth: {thisMinion.StartingHealth}" +
            $"\nDamage: {thisMinion.Damage}\nAbility: {thisMinion.CardEffect}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isAdding) 
        {
            if (DeckInstanceDeckbuilderScript.instance.AddCard(cardName))
            {
                int currentNum = Convert.ToInt32(numCardsText.text);
                currentNum++;
                numCardsText.text = currentNum.ToString();
            }
        }
        else 
        {
            if(DeckInstanceDeckbuilderScript.instance.RemoveCard(cardName))
            {
                int currentNum = Convert.ToInt32(numCardsText.text);
                currentNum--;
                numCardsText.text = currentNum.ToString();
            }
        }
    }
}
