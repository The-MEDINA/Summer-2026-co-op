using System.Collections.Generic;
using UnityEngine;

public class DBHandInstanceScript : MonoBehaviour
{
    private Player p;

    void Start()
    {
        p = GetComponent<Player>();

        if (p == null)
        {
            Debug.LogWarning("DBHandInstanceScript needs a Player component.");
            return;
        }

        DeckInstanceDeckbuilderScript deck = FindAnyObjectByType<DeckInstanceDeckbuilderScript>();

        if (deck != null)
        {
            p.Deck = new List<NewVirtualCardParent>(deck.Deck);
        }
    }
}
