using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DBButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string cardName = "Cat";
    [SerializeField] private bool isAdding = true;
    [SerializeField] private TMP_Text numCardsText;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isAdding) 
        {
            DeckInstanceDeckbuilderScript.instance.AddCard(cardName);
            int currentNum = Convert.ToInt32(numCardsText.text);
            currentNum++;
            numCardsText.text = currentNum.ToString();
        }
        else 
        {
            DeckInstanceDeckbuilderScript.instance.RemoveCard(cardName);
            int currentNum = Convert.ToInt32(numCardsText.text);
            currentNum--;
            numCardsText.text = currentNum.ToString();
        }
    }
}
