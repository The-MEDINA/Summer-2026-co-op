using UnityEngine;
using TMPro;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardHealth;
    [SerializeField] private TextMeshProUGUI cardDamage;
    [SerializeField] private TextMeshProUGUI cardFlavortext;
    [SerializeField] private CardClickHandler clickHandler;

    private void Start()
    {
        if (clickHandler == null)
        {
            clickHandler = GetComponent<CardClickHandler>();
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

        cardName.text = clickHandler.CardData.CardName;
        cardFlavortext.text = clickHandler.CardData.FlavorText;

        if (clickHandler.CardData is MinionParent)
        {
            MinionParent minionData = (MinionParent)clickHandler.CardData;

            cardHealth.text = "Health: " + minionData.Health;
            cardDamage.text = "Damage: " + minionData.Damage;
        }
    }
}