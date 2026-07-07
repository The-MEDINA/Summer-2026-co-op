using UnityEngine;
using Network;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ToPlayButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer color;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            SceneManager.LoadScene("Demo_LocalTwoPlayer");
            return;
        }
        DeckInstanceDeckbuilderScript dBDeck = FindAnyObjectByType<DeckInstanceDeckbuilderScript>();
        if (dBDeck != null)
        {
            if (!dBDeck.SentLoadout)
            {
                Networking.SendLoadout(dBDeck.Deck, dBDeck.CommanderInstance);
                dBDeck.SentLoadout = true;
                color.color = Color.green;
            }
        }
    }
}
