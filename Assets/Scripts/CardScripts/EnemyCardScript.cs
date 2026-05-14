using UnityEngine;

public class EnemyCardScript : MonoBehaviour
{
    private CardParent card;

    public CardParent Card { get { return card; } set { card = value; } }

    public EnemyCardScript()
    {
        card = new CardParent(1, 4, 3, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck);
    }

    void Update()
    {
        if (Card.IsDead) { gameObject.SetActive(false); }
    }
}
