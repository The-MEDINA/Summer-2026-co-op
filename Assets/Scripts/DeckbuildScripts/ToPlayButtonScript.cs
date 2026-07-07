using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ToPlayButtonScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        DeckInstanceDeckbuilderScript dBDeck = FindAnyObjectByType<DeckInstanceDeckbuilderScript>();
        if (dBDeck != null)
        {
            dBDeck.CardObjects = new List<GameObject>();
        }
        SceneManager.LoadScene("Demo_LocalTwoPlayer");
    }
}
