using UnityEngine;

public class EnemyCard : MonoBehaviour
{
    private CardClickHandler thisClickHandler;

    void Start()
    {
        thisClickHandler = GetComponent<CardClickHandler>();
        thisClickHandler.CardData = new CardParent(1, 11, 3, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck);
    }
}
