using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAIScript : MonoBehaviour
{
    [SerializeField] private Player player;
    private float timer = 0f;
    private float moveTime = 2f;
    private System.Random rng;
    private int drawNum;
    [SerializeField] private Battleground bg;
    [SerializeField] private Player opponent;

    private bool saving;
    private NewVirtualCardParent savingFor;
    private int savingIndex = -1;

    /// <summary>
    /// menial set up for the bot
    /// </summary>
    void Start()
    {
        rng = new System.Random();
        PopulatePlayer();
        drawNum = 0;
    }

    
    void Update()
    {
        if (player.Health <= 0)
        {
            LoseBot();
            return;
        }

        //timer to make multiple moves after set amounts of time
        if (timer >= moveTime)
        {
            Move();
            timer = 0f;
            drawNum++;
        }
        else
        {
            timer += Time.deltaTime;
        }

        //draw slower than available moves
        if(drawNum >= 2)
        {
            Draw();
            drawNum = 0;
        }

        //null error checks
        if(opponent == null) { Debug.Log("Attach a player object in inspector."); }
        if(bg == null) { Debug.Log("Attach a battleground object in inspector."); }
    }

    /// <summary>
    /// lets the ai bot "decide" what move to make and call that method
    /// </summary>
    private void Move()
    {
        int moveNum = rng.Next(1, 7);
        Debug.Log($"Cards in Deck: {player.Deck.Count}, Cards in Hand: {player.Hand.Count}, Cards in Play: {player.InPlay.Count}");
        if((player.InPlay.Count == 0 || opponent.InPlay.Count == 0)&& player.Hand.Count == 0) { return; }
        else if(player.InPlay.Count == 0 || opponent.InPlay.Count == 0) 
        {
            MoveCardToBattleground();
            return;
        }
        else if(player.Hand.Count == 0 && opponent.InPlay.Count > 0)
        {
            AttackSomething();
            return;
        }
        //int moveNum = rng.Next(1, 6);
        Debug.Log("Rolled a " + moveNum);
        switch (moveNum)
        {
            case 1:
            case 2:
                {
                    MoveCardToBattleground();
                    break;
                }

            case 3:
            case 4:
            case 5:
            default:
                {
                    AttackSomething();
                    break;
                }

            case 6:
                {
                    UseCommanderCard();
                    break;
                }
        }
    }

    /// <summary>
    /// moves a card from the ai's hand to inPlay
    /// </summary>
    private void MoveCardToBattleground()
    {
        int moveNum = 0;
        int tries = 0;
        bool loopbreaker = true;

        if (saving && !player.CanAfford(savingFor)) { return; }
        else if (!saving)
        {
            while (loopbreaker)
            {
                moveNum = rng.Next(0, player.Hand.Count);
                if (!player.CanAfford(player.Hand[moveNum])) { tries++; }
                else { loopbreaker = false; }
                if (tries >= 3)
                {
                    SaveForCard(player.Hand[moveNum]);
                    savingIndex = moveNum;
                    return;
                }
            }
        }
        else { moveNum = savingIndex; }

        if (player.Hand[moveNum] is MinionParent)
        {
            Debug.Log("Spent " + player.Hand[moveNum].Cost + " energy from " + player.Hand[moveNum].CardName);
            player.SpendEnergy(player.Hand[moveNum].Cost);
            CardSelectionManager.Instance.PlayCardToBattleground(player.Hand[moveNum].UnityObject.GetComponent<CardClickHandler>());
        }
        else if (player.Hand[moveNum] is SpellParent)
        {
            // save the last selected card
            CardClickHandler previousSelection = null;
            if (CardSelectionManager.Instance.SelectedCardObject != null) previousSelection = CardSelectionManager.Instance.SelectedCardObject;

            SpellParent aiSpell = (SpellParent)player.Hand[moveNum];
            CardSelectionManager.Instance.SelectedCardObject = player.Hand[moveNum].UnityObject.GetComponent<CardClickHandler>();

            switch (aiSpell.Target)
            {
                case SpellParent.spellTarget.inplay:
                case SpellParent.spellTarget.allEnemies:
                case SpellParent.spellTarget.any:
                case SpellParent.spellTarget.enemyCards:
                    {
                        if (opponent.InPlay.Count == 0) 
                        {
                            if (previousSelection != null) CardSelectionManager.Instance.SelectedCardObject = previousSelection;
                            if (saving) { saving = false; }
                            return; }
                        int targetNum = rng.Next(0, opponent.InPlay.Count);
                        CardSelectionManager.Instance.TrySpellTarget(opponent.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>());
                        break;
                    }

                case SpellParent.spellTarget.allAllies:
                case SpellParent.spellTarget.allyCards:
                    {
                        if (player.InPlay.Count == 0) 
                        {
                            if (previousSelection != null) CardSelectionManager.Instance.SelectedCardObject = previousSelection;
                            if (saving) { saving = false; }
                            return; }
                        int targetNum = rng.Next(0, player.InPlay.Count);
                        CardSelectionManager.Instance.TrySpellTarget(player.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>());
                        break;
                    }

                case SpellParent.spellTarget.opponent:
                case SpellParent.spellTarget.owner:
                case SpellParent.spellTarget.none:
                    {
                        CardSelectionManager.Instance.TrySpellNoTarget();
                        break;
                    }

                default: 
                    {
                        Debug.Log("NOT IMPLEMENTED");
                        break; 
                    }
            }
            // restore the previous selection if needed
            if (previousSelection != null) CardSelectionManager.Instance.SelectedCardObject = previousSelection;

            Debug.Log("Spent " + aiSpell.Cost + " energy from " + aiSpell.CardName);
            player.SpendEnergy(aiSpell.Cost);
            if(saving) { saving = false; }
            Debug.Log(saving);
        }
    }

    /// <summary>
    /// lets the ai's minions attack opposing minions
    /// </summary>
    private void AttackSomething()
    {
        int attackTarget = rng.Next(1, 5);

        // save the last selected card
        CardClickHandler previousSelection = null;
        if (CardSelectionManager.Instance.SelectedCardObject != null) previousSelection = CardSelectionManager.Instance.SelectedCardObject;

        if (attackTarget == 1 || opponent.InPlay.Count == 0)
        {
            int attackNum = rng.Next(0, player.InPlay.Count);

            if (player.InPlay[attackNum] is TwoAttackParent)
            {
                int randAttack = rng.Next(1, 3);
                if (randAttack == 1)
                {
                    CardSelectionManager.Instance.SelectedCardObject = player.InPlay[attackNum].UnityObject.GetComponent<CardClickHandler>();
                    CardSelectionManager.Instance.TryAttackPlayer(true);
                }
                else
                {
                    CardSelectionManager.Instance.SelectedCardObject = player.InPlay[attackNum].UnityObject.GetComponent<CardClickHandler>();
                    CardSelectionManager.Instance.TryAttackPlayer(false);
                }
            }
            else
            {
                CardSelectionManager.Instance.SelectedCardObject = player.InPlay[attackNum].UnityObject.GetComponent<CardClickHandler>();
                CardSelectionManager.Instance.TryAttackPlayer(false);
            }
        }
        else
        {
            int attackNum = rng.Next(0, player.InPlay.Count);
            int targetNum = rng.Next(0, opponent.InPlay.Count);

            if (player.InPlay[attackNum] is TwoAttackParent)
            {
                int randAttack = rng.Next(1, 3);
                if (randAttack == 1)
                {
                    CardSelectionManager.Instance.SelectedCardObject = player.InPlay[attackNum].UnityObject.GetComponent<CardClickHandler>();
                    CardSelectionManager.Instance.TryAttackTarget(opponent.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>(), true);
                }
                else
                {
                    CardSelectionManager.Instance.SelectedCardObject = player.InPlay[attackNum].UnityObject.GetComponent<CardClickHandler>();
                    CardSelectionManager.Instance.TryAttackTarget(opponent.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>(), false);
                }
            }
            else
            {
                CardSelectionManager.Instance.SelectedCardObject = player.InPlay[attackNum].UnityObject.GetComponent<CardClickHandler>();
                CardSelectionManager.Instance.TryAttackTarget(opponent.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>(), false);
            }
        }

        // restore the previous selection if needed
        if (previousSelection != null) CardSelectionManager.Instance.SelectedCardObject = previousSelection;
    }

    /// <summary>
    /// lets the ai activate its commander card ability
    /// </summary>
    private void UseCommanderCard()
    {
        Debug.Log("COMMANDER CARD USED");
        player.CommanderCard.PerformAbility();
    }

    /// <summary>
    /// draws a card for the ai into its hand
    /// </summary>
    private void Draw()
    {
        bg.DrawCardToHand();
    }

    /// <summary>
    /// add cards here for the ai to have in its hand
    /// </summary>
    private void PopulatePlayer()
    {
        //int randDeckNum = rng.Next(1, 5);
        int deckNum = 1;
        switch(deckNum)
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

                    for (int i = 0; i < tutorialDeck.Length; i++)
                    {
                        player.Deck.Add(cardIndex.Index.CreateCard(tutorialDeck[i], NewVirtualCardParent.location.deck));
                    }
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

                    for (int i = 0; i < firstDeck.Length; i++)
                    {
                        player.Deck.Add(cardIndex.Index.CreateCard(firstDeck[i], NewVirtualCardParent.location.deck));
                    }
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

                    for (int i = 0; i < secondDeck.Length; i++)
                    {
                        player.Deck.Add(cardIndex.Index.CreateCard(secondDeck[i], NewVirtualCardParent.location.deck));
                    }
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

                    for (int i = 0; i < thirdDeck.Length; i++)
                    {
                        player.Deck.Add(cardIndex.Index.CreateCard(thirdDeck[i], NewVirtualCardParent.location.deck));
                    }
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

                    for (int i = 0; i < fourthDeck.Length; i++)
                    {
                        player.Deck.Add(cardIndex.Index.CreateCard(fourthDeck[i], NewVirtualCardParent.location.deck));
                    }
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

                    for (int i = 0; i < fifthDeck.Length; i++)
                    {
                        player.Deck.Add(cardIndex.Index.CreateCard(fifthDeck[i], NewVirtualCardParent.location.deck));
                    }
                    break;
                }
        }
    }

    private void SaveForCard(NewVirtualCardParent saveTarget)
    {
        savingFor = saveTarget;
        saving = true;
    }

    private void LoseBot()
    {
        float newUNum = PlayerPrefs.GetFloat("UnlockNum");
        newUNum++;
        PlayerPrefs.SetFloat("UnlockNum", newUNum);
        PlayerPrefs.Save();
    }
}
