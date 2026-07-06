using UnityEngine;
using UnityEngine.UI;
using TMPro;
using cardIndex;

public class DeckbuilderCard : MonoBehaviour
{
    #region UI_VARIABLES
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardHealth;
    [SerializeField] private TextMeshProUGUI cardDamage;
    [SerializeField] private TextMeshProUGUI cardCost;
    [SerializeField] private TextMeshProUGUI cardEquipment;
    [SerializeField] private TextMeshProUGUI cardFlavortext;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private TextMeshProUGUI cardExtraDamage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image cardArt;
    [SerializeField] private Image DescriptionBackground;
    #endregion

    private NewVirtualCardParent cardInstance;
    private DeckInstanceDeckbuilderScript deckInstance;
    private int amount;
    public NewVirtualCardParent CardInstance { get { return cardInstance; } set { cardInstance = value; UpdateUI(); } }
    public DeckInstanceDeckbuilderScript DeckInstance { get { return deckInstance; } set { deckInstance = value; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Try to add this card to the deck.
    /// </summary>
    public void TryAddCard()
    {
        if (deckInstance != null)
        {
            if (deckInstance.AddCard(cardInstance.CardName))
            {
                amount++;
                amountText.text = $"Amount: {amount}";
            }
        }
    }

    /// <summary>
    /// Try to remove this card from the deck.
    /// </summary>
    public void TryRemoveCard()
    {
        if (deckInstance != null)
        {
            if (deckInstance.RemoveCard(cardInstance.CardName))
            {
                amount--;
                amountText.text = $"Amount: {amount}";
            }
        }
    }

    private void UpdateUI()
    {
        // update card text.
        cardName.text = cardInstance.CardName;
        cardFlavortext.text = cardInstance.FlavorText;
        cardCost.text = $"{cardInstance.Cost}";

        if (cardInstance is MinionParent)
        {
            MinionParent minionData = (MinionParent)cardInstance;

            cardHealth.text = $"{minionData.Health}";
            cardDamage.text = $"{minionData.Damage}";
            cardEquipment.text = "";
            for (int i = 0; i < minionData.EquipmentList.Count; i++)
            {
                cardEquipment.text += $"{minionData.EquipmentList[i]}\n";
            }
            if (minionData.CardType == NewVirtualCardParent.type.token)
            {
                cardCost.text = "";
            }
        }
        else if (cardInstance is SpellParent)
        {
            cardHealth.text = "";
            cardDamage.text = "";
        }

        cardDescription.text = cardIndex.Index.GetDetails(cardInstance.CardName).description;

        // add sprites.
        cardIndex.Sprites updatedArt;
        updatedArt = cardIndex.Index.GetSprites(cardName.text);
        if (updatedArt.cardImage != null) cardArt.sprite = updatedArt.cardImage;
        if (updatedArt.DescBackground != null) DescriptionBackground.sprite = updatedArt.DescBackground;
    }
}
