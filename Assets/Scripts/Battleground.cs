using UnityEngine;
using UnityEngine.EventSystems;

public class Battleground : MonoBehaviour, IPointerClickHandler
{
    private Player p;
    [SerializeField] private GameObject cardProto;

    void Start()
    {
        p = gameObject.AddComponent<Player>();
        for(int i = 0; i < 10; i++)
        {
            p.Deck.Add(new CardParent(0, 0, 0, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        }
    }

    void Update()
    {
        if (p.canDraw == true)
        {
            p.Hand.Add(p.Deck[0]);
            p.Deck.RemoveAt(0);
            Instantiate(cardProto, new Vector3(-5.75f + ((p.Hand.Count - 1) * 2f), -3.75f, -0.1f), Quaternion.identity);
            p.canDraw = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        p.Hand.Add(p.Deck[0]);
        p.Deck.RemoveAt(0);
        Instantiate(cardProto, new Vector3(-5.75f + ((p.Hand.Count - 1) * 2f), -3.75f, -0.1f), Quaternion.identity);
    }
}
