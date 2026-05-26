using UnityEngine;

public class EnemyCard : MonoBehaviour
{
    private CardClickHandler thisClickHandler;

    [SerializeField] private string cardName = "Mad Scientist Cat";

    private void Start()
    {
        thisClickHandler = GetComponent<CardClickHandler>();

        if (thisClickHandler == null)
        {
            Debug.LogWarning("EnemyCard needs CardClickHandler.");
            return;
        }

        thisClickHandler.CardData = new MinionParent(cardName, NewVirtualCardParent.location.inPlay);
    }
}