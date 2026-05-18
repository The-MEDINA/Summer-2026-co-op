using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battleground : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Player p;
    [SerializeField] private GameObject cardProto;
    private List<GameObject> cardList = new List<GameObject>();
    private CardClickHandler currentCard;
    [SerializeField] private HandUIManager handUIManager;

    void Start()
    {
        //this will probably get deleted soon it just matters based on when I get around to something vaguely related - Jake

        /*p = gameObject.AddComponent<Player>();
        for(int i = 0; i < 10; i++)
        {
            p.Deck.Add(new CardParent(1, 8, 5, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        }*/
    }

    void Update()
    {
        if (p.canDraw == true && p.Deck.Count > 0)
        {
            p.Hand.Add(p.Deck[0]);
            p.Deck.RemoveAt(0);

            GameObject newCard = Instantiate(cardProto, new Vector3(-5.75f + ((p.Hand.Count - 1) * 2f), -3.75f, -0.1f), Quaternion.identity);
            cardList.Add(newCard);
            currentCard = cardList[cardList.Count - 1].GetComponent<CardClickHandler>();
            currentCard.CardData = p.Hand[p.Hand.Count - 1];

            //error due to no instance of handUIManager in scene
            //handUIManager.AddCardToHand(newCard);
    
            p.canDraw = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (p.Deck.Count > 0)
        {
            //should be refactored to same as update if this is going to continue to be used for testing purposes
            p.Hand.Add(p.Deck[0]);
            p.Deck.RemoveAt(0);
            cardList.Add(Instantiate(cardProto, new Vector3(-5.75f + ((p.Hand.Count - 1) * 2f), -3.75f, -0.1f), Quaternion.identity));
            currentCard = cardList[cardList.Count - 1].GetComponent<CardClickHandler>();
            currentCard.CardData = p.Hand[p.Hand.Count - 1];
        }
    }
}
