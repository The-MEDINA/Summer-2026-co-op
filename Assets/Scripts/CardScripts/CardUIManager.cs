using UnityEngine;
using TMPro;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardHealth;
    [SerializeField] private TextMeshProUGUI cardDamage;
    [SerializeField] private TextMeshProUGUI cardFlavortext;
    [SerializeField] private CardClickHandler clickHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // set the text on the card through the click handler's reference to the card. 
        cardName.text = clickHandler.CardData.CardName;
        cardFlavortext.text = clickHandler.CardData.FlavorText;

        if(clickHandler.CardData is MinionParent)
        {
            MinionParent MinionData = (MinionParent)clickHandler.CardData;
            cardHealth.text = $"Health: {MinionData.Health}";
            cardDamage.text = $"Damage: {MinionData.Damage}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
