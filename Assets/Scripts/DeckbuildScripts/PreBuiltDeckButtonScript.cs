using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreBuiltDeckButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int deckNum = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        string[] deck = new string[12];
        switch (deckNum)
        {
            case 0:
                {
                    string[] tutorialDeck =
                    {
                        "Cat",
                        "Cat",
                        "Cool Cat",
                        "Cool Cat",
                        "Gold Miner Cat",
                        "Cave Cat",
                        "Tank Cat",
                        "Spontaneous Combustion",
                        "Spontaneous Combustion",
                        "M16",
                        "M16",
                        "Terrorize"
                    };

                    deck = tutorialDeck;
                    break;
                }

            case 1:
                {
                    string[] firstDeck =
                    {
                        "Ninja Cat",
                        "Ninja Cat",
                        "Exploding Cat",
                        "Mad Scientist Cat",
                        "Astro Cat",
                        "Night Vision Cat",
                        "Cat Demolition Crew",
                        "Patch Up",
                        "Patch Up",
                        "Hex",
                        "Smite",
                        "I Hungy!!!",
                    };
                    deck = firstDeck;
                    break;
                }

            case 2:
                {
                    string[] secondDeck =
                    {
                        "Mother Cat",
                        "Mother Cat",
                        "Ratta-tat-Cat",
                        "Ratta-tat-Cat",
                        "Cat Man",
                        "Single-Celled Cat",
                        "Vampire Cat",
                        "Cat Fusion",
                        "Conscript",
                        "Blizzard",
                        "Pspspsps!",
                        "Pspspsps!"
                    };
                    deck = secondDeck;
                    break;
                }

            case 3:
                {
                    string[] thirdDeck =
                    {
                        "Nacho Cat",
                        "Bobby",
                        "Ice Cream Cat",
                        "Chonkmeister",
                        "The Mad Catter",
                        "Viking Cat",
                        "Doctor House(Cat)",
                        "Roughly a Cat",
                        "I'm Sure That Wasn't Important",
                        "I'm Sure That Wasn't Important",
                        "Distraction",
                        "Duplicate"
                    };
                    deck = thirdDeck;
                    break;
                }

            case 4:
                {
                    string[] fourthDeck =
                    {
                        "Digger",
                        "Digger",
                        "Living Planet",
                        "Slime",
                        "Frozen Horror",
                        "Frostback",
                        "Lost in Space",
                        "Lost in Space",
                        "Hide",
                        "Hide",
                        "Genetic Engineering",
                        "Sabotage"
                    };
                    deck = fourthDeck;
                    break;
                }

            case 5:
                {
                    string[] fifthDeck =
                    {
                        "Reptoid",
                        "Reptoid",
                        "Slate Skate",
                        "Slate Skate",
                        "Uncanny Valley",
                        "Mimic",
                        "Blood Eater",
                        "Solar Panels",
                        "Solar Panels",
                        "Nuclear Waste",
                        "Abduction",
                        "Parasite"
                    };
                    deck = fifthDeck;
                    break;
                }
        }

        for(int i = 0; i < 12; i++)
        {
            DeckInstanceDeckbuilderScript.instance.AddCard(deck[i]);
        }
    }
}
