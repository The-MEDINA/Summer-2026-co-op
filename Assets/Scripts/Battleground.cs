using UnityEngine;
using UnityEngine.EventSystems;

public class Battleground : MonoBehaviour, IPointerClickHandler
{
    private Player p;

    void Start()
    {
        p = new Player();
        for(int i = 0; i < 10; i++)
        {
            p.Deck.Add(new CardParent(0, 0, 0, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        }
    }

    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("c");
    }
}
