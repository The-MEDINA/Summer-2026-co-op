using UnityEngine;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    [SerializeField] private string[] startingDeck =
    {
        "Cat",
        "Macho Cat",
        "Cool Cat",
        "Western Cat",
        "Tank Cat"
    };

    private Player p;

    private void Start()
    {
        p = GetComponent<Player>();

        if (p == null)
        {
            Debug.LogWarning("DemoPlayerInstanceScript needs a Player component.");
            return;
        }

        for (int i = 0; i < startingDeck.Length; i++)
        {
            p.Deck.Add(new MinionParent(startingDeck[i], NewVirtualCardParent.location.deck));
        }

        Debug.Log(gameObject.name + " deck loaded with " + p.Deck.Count + " cards.");
    }
}