using System.Collections.Generic;
using UnityEngine;

public class HandUIManager : MonoBehaviour
{
    [SerializeField] private float cardSpacing = 2f;
    [SerializeField] private float handYPosition = -3.75f;
    [SerializeField] private float startXPosition = -5.75f;

    private List<GameObject> cardObjects = new List<GameObject>();

    public void AddCardToHand(GameObject cardObject)
    {
        cardObjects.Add(cardObject);
        UpdateHandPositions();
    }

    public void RemoveCardFromHand(GameObject cardObject)
    {
        cardObjects.Remove(cardObject);
        UpdateHandPositions();
    }

    public void UpdateHandPositions()
    {
        for (int i = 0; i < cardObjects.Count; i++)
        {
            float xPos = startXPosition + (i * cardSpacing);
            cardObjects[i].transform.position = new Vector3(xPos, handYPosition, 0);
        }
    }
}