using UnityEngine;
using System;

public class SimpleAIScript : MonoBehaviour
{
    private Player player;
    private float timer = 0f;
    private float moveTime = 2f;
    private System.Random rng;
    private int drawNum;

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
            //draw
            drawNum = 0;
        }
    }

    private void Move()
    {
        if(player.InPlay.Count == 0 && player.Hand.Count == 0) { return; }
        else if(player.InPlay.Count == 0) 
        {
            /*move to field*/
            return;
        }
        else if(player.Hand.Count == 0)
        {
            //attack
            return;
        }
        int moveNum = rng.Next();
        switch (moveNum)
        {
            //move to field
            //attack
        }
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
