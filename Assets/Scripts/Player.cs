using System.Collections.Generic;
using UnityEngine;
using Network;

public class Player : MonoBehaviour
{
    [SerializeField] private int health = 50;
    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private int startingEnergy = 10;
    [SerializeField] private int timeForEnergy = 5;
    [SerializeField] private bool isPlayerTwo = false;


    private int energy;
    private List<NewVirtualCardParent> deck = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> hand = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> inPlay = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> discard = new List<NewVirtualCardParent>();

    private float timer = 0f;

    public int Health { get { return health; } set { health = value; } }
    public int Energy { get { return energy; } set { energy = value; } }
    public bool IsPlayerTwo { get { return isPlayerTwo; } }

    public List<NewVirtualCardParent> Deck { get { return deck; } set { deck = value; } }
    public List<NewVirtualCardParent> Hand { get { return hand; } set { hand = value; } }
    public List<NewVirtualCardParent> InPlay { get { return inPlay; } set { inPlay = value; } }
    public List<NewVirtualCardParent> Discard { get { return discard; } set { discard = value; } }

    private void Start()
    {
        energy = startingEnergy;
        if (isPlayerTwo) Networking.PlayerTwo = this;
        else Networking.PlayerOne = this;

        // need player two to be able to act whenever on player 1's side.
        // Easiest way to do this right now is to just make them never run out of energy or have to wait on the timer.
        // doing this should prevent any situations where player 2 does something on their screen, but it doesn't happen on player 1's screen because they didn't have enough energy.
        // Ideally both games would be perfectly in sync somehow so this situation would never happen, but this is an easy workaround just to get it done.
        if (isPlayerTwo) energy = 999; 
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
            if (!isPlayerTwo) timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public void RegisterAction()
    {
        if (!isPlayerTwo) timer = 0f;
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

        if (!isPlayerTwo) Energy -= amount;
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
        CheckCoordinate();
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

    private void CheckCoordinate()
    {
        for (int i = 0; i < InPlay.Count; i++)
        {
            MinionParent minion = (MinionParent)InPlay[i];
            if(minion.CardEffect == MinionParent.effect.coordinate && minion.CoordinateAbility.Awarded == false)
            {
                int coordNum = 0;
                for (int j = 0; j < InPlay.Count; j++)
                {
                    MinionParent newMinion = (MinionParent)InPlay[j];
                    if (newMinion.CardEffect == MinionParent.effect.coordinate)
                    {
                        coordNum++;
                    }
                }
                if(coordNum >= minion.CoordinateAbility.NumToHit) { minion.CoordinateAbility.RewardAbility(minion); }
            }
        }
    }
}