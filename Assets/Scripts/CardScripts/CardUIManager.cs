using UnityEngine;
using TMPro;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardHealth;
    [SerializeField] private TextMeshProUGUI cardDamage;
    [SerializeField] private CardClickHandler clickHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // set the text on the card through the click handler's reference to the card. 
        cardName.text = clickHandler.CardData.CardName;
        cardHealth.text = $"Health: {clickHandler.CardData.Health}";
        cardDamage.text = $"Damage: {clickHandler.CardData.Damage}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
