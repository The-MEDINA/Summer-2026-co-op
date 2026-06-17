using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DBButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string cardName = "Cat";
    [SerializeField] private bool isAdding = true;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isAdding) { DeckInstanceDeckbuilderScript.instance.AddCard(cardName); }
        else { DeckInstanceDeckbuilderScript.instance.RemoveCard(cardName); }
    }
}
