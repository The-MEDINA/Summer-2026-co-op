using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DeckInstanceDeckbuilderScript : MonoBehaviour
{
    public static DeckInstanceDeckbuilderScript instance;

    private List<NewVirtualCardParent> deck = new List<NewVirtualCardParent>();

    public List<NewVirtualCardParent> Deck { get { return this.deck; } } 

    private void Awake()
    {
        //singleton instance code
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public bool AddCard(string cardName)
    {
        //deck at capacity
        //too many copies of that card

        Deck.Add(new MinionParent(cardName, NewVirtualCardParent.location.deck));
        return true;
    }

    public bool RemoveCard(string cardName)
    {
        if(Deck.Count <= 0) { return false; }

        for (int i = 0; i < Deck.Count; i++)
        {
            if (Deck[i].CardName == cardName)
            {
                Deck.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
