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

    void Start()
    {
        PopulatePlayer();
        rng = new System.Random();
        drawNum = 2;
    }

    
    void Update()
    {
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

        if(drawNum >= 2)
        {
            Draw();
            drawNum = 0;
        }

        if(opponent == null) { Debug.Log("Attach a player object in inspector."); }
        if(bg == null) { Debug.Log("Attach a battleground object in inspector."); }
    }

    private void Move()
    {
        Debug.Log(player.Hand.Count);//inplay cards are moving weird once then arent moving anymore
        Debug.Log(player.InPlay.Count);//its all ui the actual interactions seem to work
        Debug.Log(opponent.InPlay.Count);
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
        int moveNum = rng.Next(1, 4);
        switch (moveNum)
        {
            case 1:
                {
                    MoveCardToBattleground();
                    break;
                }

            case 2:
            case 3:
            default:
                {
                    AttackSomething();
                    break;
                }
        }
    }

    private void MoveCardToBattleground()
    {
        int moveNum = rng.Next(0, player.Hand.Count);
        if (player.Hand[moveNum] is MinionParent)
        {
            CardSelectionManager.Instance.PlayCardToBattleground(player.Hand[moveNum].UnityObject.GetComponent<CardClickHandler>());
        }
        else if (player.Hand[moveNum] is SpellParent)
        {
            if (opponent.InPlay.Count == 0) { return; }
            int targetNum = rng.Next(0, opponent.InPlay.Count);
            CardSelectionManager.Instance.TrySpellTarget(opponent.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>());
        }
    }

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

    private void Draw()
    {
        bg.DrawCardToHand();
    }

    private void PopulatePlayer()
    {
        player.Deck.Add(new MinionParent(4, 3, 3, "Single Celled Cat",
                NewVirtualCardParent.type.minion, MinionParent.effect.duplicate, NewVirtualCardParent.location.deck));
        player.Deck.Add(cardIndex.Index.CreateCard("M16", NewVirtualCardParent.location.deck));
        player.Deck.Add(new SpellParent(SpellParent.spellEffect.spawnTokens, SpellParent.spellTarget.none, 3, 2, 1, "Conscript",
            NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        player.Deck.Add(new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.allyCards, 2, 2, 1, "I Hungy!!!",
            NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        player.Deck.Add(new TwoAttackParent(3, 1, MinionParent.effect.aoe, 4, 4, 0, "Mage Cat",
            NewVirtualCardParent.type.minion, MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
    }
}
