using UnityEngine;
using System;

public class SimpleAIScript : MonoBehaviour
{
    private Player player;
    private float timer = 0f;
    private float moveTime = 2f;
    private System.Random rng;
    private int drawNum;
    [SerializeField] private Battleground bg;
    [SerializeField] private Player opponent;

    void Start()
    {
        player = new Player();
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
        if(player.InPlay.Count == 0 && player.Hand.Count == 0) { return; }
        else if(player.InPlay.Count == 0) 
        {
            MoveCardToBattleground();
            return;
        }
        else if(player.Hand.Count == 0)
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
        int moveNum = rng.Next(1, player.Hand.Count + 1);
        CardSelectionManager.Instance.PlayCardToBattleground(player.Hand[moveNum].UnityObject.GetComponent<CardClickHandler>());
    }

    private void AttackSomething()
    {
        int attackNum = rng.Next(1, player.InPlay.Count + 1);
        int targetNum = rng.Next(1, opponent.InPlay.Count + 1);

        if (player.InPlay[attackNum] is TwoAttackParent)
        {
            int randAttack = rng.Next(1, 3);
            if (randAttack == 1)
            {
                CardSelectionManager.Instance.TryAttackTarget(opponent.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>(), true);
            }
            else
            {
                CardSelectionManager.Instance.TryAttackTarget(opponent.InPlay[targetNum].UnityObject.GetComponent<CardClickHandler>(), false);

            }
        }
        else
        {
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
