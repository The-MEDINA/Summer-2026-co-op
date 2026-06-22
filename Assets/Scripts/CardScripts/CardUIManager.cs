using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUIManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardHealth;
    [SerializeField] private TextMeshProUGUI cardDamage;
    [SerializeField] private TextMeshProUGUI cardCost;
    [SerializeField] private TextMeshProUGUI cardEquipment;
    [SerializeField] private TextMeshProUGUI cardFlavortext;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private TextMeshProUGUI cardExtraDamage;
    [SerializeField] private CardClickHandler clickHandler;
    [SerializeField] private Image cardArt;
    [SerializeField] private Image DescriptionBackground;

    int initialDamage;
    #endregion

    private void Start()
    {
        if (clickHandler == null)
        {
            clickHandler = GetComponent<CardClickHandler>();
        }
        clickHandler.OwnerPlayer.energyChange += ChangeCostColor;
        if (clickHandler.CardData is MinionParent)
        {
            MinionParent minion = (MinionParent)clickHandler.CardData;
            initialDamage = minion.Damage;
        }
        RefreshCardUI();
    }

    /// <summary>
    /// updates the text on cards to accurately reflect its stats and information
    /// </summary>
    public void RefreshCardUI()
    {
        if (clickHandler == null || clickHandler.CardData == null)
        {
            return;
        }

        // update card text.
        cardName.text = clickHandler.CardData.CardName;
        cardFlavortext.text = clickHandler.CardData.FlavorText;
        cardCost.text = $"{clickHandler.CardData.Cost}";

        if (clickHandler.CardData is MinionParent)
        {
            MinionParent minionData = (MinionParent)clickHandler.CardData;

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
            if (minionData.Damage == initialDamage)
            {
                cardExtraDamage.text = "";
            }
            else
            {
                if (minionData.Damage > initialDamage)
                {
                    cardExtraDamage.text = $"+{minionData.Damage - initialDamage}";
                }
                else
                {
                    cardExtraDamage.text = $"{minionData.Damage - initialDamage}";
                }
            }
        }
        else if (clickHandler.CardData is SpellParent)
        {
            cardHealth.text = "";
            cardDamage.text = "";
        }

        if (clickHandler.OwnerPlayer.Energy >= clickHandler.CardData.Cost)
        {
            cardCost.color = Color.black;
        }
        else if (clickHandler.CardData.CardLocation == NewVirtualCardParent.location.hand)
        {
            cardCost.color = Color.red;
        }
        else
        {
            cardCost.color = Color.black;
        }
        cardDescription.text = cardIndex.Index.GetDetails(cardName.text).description;

        // add sprites.
        cardIndex.Sprites updatedArt;
        updatedArt = cardIndex.Index.GetSprites(cardName.text);
        if (updatedArt.cardImage != null) cardArt.sprite = updatedArt.cardImage;
        if (updatedArt.DescBackground != null) DescriptionBackground.sprite = updatedArt.DescBackground;
    }


    private void ChangeCostColor()
    {
        if (clickHandler.CardData.CardLocation == NewVirtualCardParent.location.hand && 
            clickHandler.OwnerPlayer.Energy < clickHandler.CardData.Cost)
        {
            cardCost.color = Color.red;
        }
        else
        {
            cardCost.color = Color.black;
        }
    }
}