using UnityEngine;
using System;
using System.Collections.Generic;

public class SimpleAIScript : MonoBehaviour
{
    [SerializeField] private Player player;
    private float timer = 0f;
    private float moveTime = 2f;
    private System.Random rng;
    private int drawNum;
    [SerializeField] private Battleground bg;
    [SerializeField] private Player opponent;

    /// <summary>
    /// menial set up for the bot
    /// </summary>
    void Start()
    {
        PopulatePlayer();
        rng = new System.Random();
        drawNum = 2;
    }

    
    void Update()
    {
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
        Debug.Log($"Cards in Deck: {player.Deck.Count}, Cards in Hand: {player.Hand.Count}, Cards in Play: {player.InPlay.Count}");
        if((player.InPlay.Count == 0 || opponent.InPlay.Count == 0)&& player.Hand.Count == 0) { return; }
        else if(player.InPlay.Count == 0 || opponent.InPlay.Count < 0) 
        {
            MoveCardToBattleground();
            return;
        }
        else if(player.Hand.Count == 0 && opponent.InPlay.Count > 0)
        {
            AttackSomething();
            return;
        }
        int moveNum = rng.Next(1, 7);
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

        while (loopbreaker)
        {
            moveNum = rng.Next(0, player.Hand.Count);
            if (!player.CanAfford(player.Hand[moveNum])) { tries++; }
            else { loopbreaker = false; }
            if (tries >= 3) { return; }
        }

        if (player.Hand[moveNum] is MinionParent)
        {
            Debug.Log("Spent " + player.Hand[moveNum].Cost + " energy from " + player.Hand[moveNum].CardName);
            player.SpendEnergy(player.Hand[moveNum].Cost);
            CardSelectionManager.Instance.PlayCardToBattleground(player.Hand[moveNum].UnityObject.GetComponent<CardClickHandler>());
        }
        else if (player.Hand[moveNum] is SpellParent)
        {
            SpellParent aiSpell = (SpellParent)player.Hand[moveNum];
            CardSelectionManager.Instance.SelectedCardObject = player.Hand[moveNum].UnityObject.GetComponent<CardClickHandler>();

            switch (aiSpell.Target)
            {
                case SpellParent.spellTarget.enemyCards:
                    {
                        if (opponent.InPlay.Count == 0) { return; }
                        int targetNum = rng.Next(0, opponent.InPlay.Count);
                        CardSelectionManager.Instance.TrySpellTarget(opponent.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>());
                        break;
                    }

                case SpellParent.spellTarget.allyCards:
                    {
                        if (player.InPlay.Count == 0) { return; }
                        int targetNum = rng.Next(0, player.InPlay.Count);
                        CardSelectionManager.Instance.TrySpellTarget(player.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>());
                        break;
                    }

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

            Debug.Log("Spent " + aiSpell.Cost + " energy from " + aiSpell.CardName);
            player.SpendEnergy(aiSpell.Cost);
        }
    }

    /// <summary>
    /// lets the ai's minions attack opposing minions
    /// </summary>
    private void AttackSomething()
    {
        if(opponent.InPlay.Count == 0) { return; }
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

    /// <summary>
    /// lets the ai activate its commander card ability
    /// </summary>
    private void UseCommanderCard()
    {
        Debug.Log("COMMANDER CARD USED");
        bg.CommanderCard.PerformAbility();
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
        player.Deck.Add(new MinionParent(4, 3, 3, "Single Celled Cat",
                NewVirtualCardParent.type.minion, MinionParent.effect.duplicate, NewVirtualCardParent.location.deck));
        player.Deck.Add(cardIndex.Index.CreateCard("M16", NewVirtualCardParent.location.deck));
        player.Deck.Add(new SpellParent(SpellParent.spellEffect.spawnTokens, SpellParent.spellTarget.none, 3, 0, 4, "Conscript",
            NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
    }
}
