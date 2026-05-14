using UnityEngine;

public class EnemyCard : MonoBehaviour
{
    private CardClickHandler thisClickHandler;

    //this script exists to test out interactions against opposing cards
    //because card creation is currently handled in battleground, and will eventually be moved to player, enemy cards need a player of their own
    //they'll get one eventually, both in an ai player and in the opposing player from the network
    //but for now this is a simple patch fix - Jake

    void Start()
    {
        thisClickHandler = GetComponent<CardClickHandler>();
        thisClickHandler.CardData = new CardParent(1, 11, 3, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck);
    }
}
