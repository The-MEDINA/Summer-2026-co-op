using UnityEngine;
using Network;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ToPlayButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer color;

    private void Start()
    {
        if (DeckInstanceDeckbuilderScript.instance != null)
        {
            if (DeckInstanceDeckbuilderScript.instance.TitleScreenButtonPressed == "Deck Builder")
            {
                gameObject.SetActive(false);
            }
            else gameObject.SetActive(true);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            SceneManager.LoadScene("Demo_LocalTwoPlayer");
            return;
        }
        else if (Networking.CurrentState == state.disconnected)
        {
            SceneManager.LoadScene("AITestScene");
            return;
        }
        if (DeckInstanceDeckbuilderScript.instance != null)
        {
            if (DeckInstanceDeckbuilderScript.instance.Commander == "" || DeckInstanceDeckbuilderScript.instance.Deck.Count == 0)
            {
                Debug.Log("You need at least 1 minion and a commander!");
                return;
            }
            if (!DeckInstanceDeckbuilderScript.instance.SentLoadout)
            {
                Networking.SendLoadout(DeckInstanceDeckbuilderScript.instance.Deck, DeckInstanceDeckbuilderScript.instance.CommanderInstance);
                DeckInstanceDeckbuilderScript.instance.SentLoadout = true;
                color.color = Color.green;
            }
        }
    }
}
