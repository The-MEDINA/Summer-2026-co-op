using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int health = 50;
    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private int startingEnergy = 10;
    [SerializeField] private int timeForEnergy = 5;

    private int energy;
    private List<NewVirtualCardParent> deck = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> hand = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> inPlay = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> discard = new List<NewVirtualCardParent>();

    private float timer = 0f;

    public int Health { get { return health; } set { health = value; } }
    public int Energy { get { return energy; } set { energy = value; } }

    public List<NewVirtualCardParent> Deck { get { return deck; } set { deck = value; } }
    public List<NewVirtualCardParent> Hand { get { return hand; } set { hand = value; } }
    public List<NewVirtualCardParent> InPlay { get { return inPlay; } set { inPlay = value; } }
    public List<NewVirtualCardParent> Discard { get { return discard; } set { discard = value; } }

    private void Start()
    {
        energy = startingEnergy;
    }

    private void Update()
    {
        GainEnergyOverTime();
    }

    private void GainEnergyOverTime()
    {
        if (timer >= timeForEnergy)
        {
            GainEnergy(1);
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public void RegisterAction()
    {
        timer = 0f;
    }

    public bool CanAfford(NewVirtualCardParent card)
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
        RegisterAction();
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

    public void MoveCardToInPlay(NewVirtualCardParent card)
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

        card.CardLocation = NewVirtualCardParent.location.inPlay;
        RegisterAction();
    }

    public void MoveCardToDiscard(NewVirtualCardParent card)
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

        card.CardLocation = NewVirtualCardParent.location.discard;
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