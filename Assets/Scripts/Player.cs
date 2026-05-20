using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int health = 50;
    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private int timeForEnergy = 5;

    private int energy;
    private List<CardParent> deck = new List<CardParent>();
    private List<CardParent> hand = new List<CardParent>();
    private List<CardParent> inPlay = new List<CardParent>();
    private List<CardParent> discard = new List<CardParent>();

    private CardParent commander;
    private float timer = 0f;

    private float drawTimer = 0f;
    private float timeToDraw = 2f;

    public bool canDraw = false;

    public int Health { get { return health; } set { health = value; } }
    public int Energy { get { return energy; } set { energy = value; } }

    public List<CardParent> Deck { get { return deck; } set { deck = value; } }
    public List<CardParent> Hand { get { return hand; } set { hand = value; } }
    public List<CardParent> InPlay { get { return inPlay; } set { inPlay = value; } }
    public List<CardParent> Discard { get { return discard; } set { discard = value; } }

    private void Update()
    {
        GainEnergyOverTime();
        DrawTimer();
    }

    private void GainEnergyOverTime()
    {
        if (timer >= timeForEnergy)
        {
            GainEnergy(1);
            timer -= timeForEnergy;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void DrawTimer()
    {
        if (drawTimer >= timeToDraw)
        {
            canDraw = true;
            drawTimer = 0f;
        }
        else
        {
            drawTimer += Time.deltaTime;
        }
    }

    public bool CanAfford(CardParent card)
    {
        return card != null && Energy >= card.Cost;
    }

    public bool SpendEnergy(int amount)
    {
        if (Energy < amount)
        {
            return false;
        }

        Energy -= amount;
        return true;
    }

    public void GainEnergy(int amount)
    {
        Energy += amount;

        if (Energy > maxEnergy)
        {
            Energy = maxEnergy;
        }
    }

    public void MoveCardToInPlay(CardParent card)
    {
        if (card == null)
        {
            return;
        }

        Hand.Remove(card);

        if (!InPlay.Contains(card))
        {
            InPlay.Add(card);
        }

        card.CardLocation = CardParent.location.inPlay;
    }

    public void MoveCardToDiscard(CardParent card)
    {
        if (card == null)
        {
            return;
        }

        Hand.Remove(card);
        InPlay.Remove(card);

        if (!Discard.Contains(card))
        {
            Discard.Add(card);
        }

        card.CardLocation = CardParent.location.discard;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            Death();
        }
    }

    public void Death()
    {
        Debug.Log(gameObject.name + " has lost.");
    }
}