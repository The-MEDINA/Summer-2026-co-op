using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int health = 50;
    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private int startingEnergy = 0;
    [SerializeField] private float timeForEnergy = 5f;
    [SerializeField] private float timeToDraw = 2f;

    private int energy;
    private float energyTimer = 0f;
    private float drawTimer = 0f;

    private List<NewVirtualCardParent> deck = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> hand = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> inPlay = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> discard = new List<NewVirtualCardParent>();

    public bool canDraw = false;

    public int Health { get { return health; } set { health = value; } }
    public int Energy { get { return energy; } set { energy = value; } }
    public int MaxEnergy { get { return maxEnergy; } }

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
        DrawTimer();
    }

    private void GainEnergyOverTime()
    {
        energyTimer += Time.deltaTime;

        if (energyTimer >= timeForEnergy)
        {
            GainEnergy(1);
            energyTimer = 0f;
        }
    }

    private void DrawTimer()
    {
        drawTimer += Time.deltaTime;

        if (drawTimer >= timeToDraw)
        {
            canDraw = true;
            drawTimer = 0f;
        }
    }

    public void RegisterAction()
    {
        energyTimer = 0f;
    }

    public bool CanAfford(NewVirtualCardParent card)
    {
        return card != null && Energy >= card.Cost;
    }

    public bool SpendEnergy(int amount)
    {
        if (amount < 0)
        {
            return false;
        }

        if (Energy < amount)
        {
            Debug.Log("Not enough energy.");
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

    public void MoveCardToInPlay(MinionParent card)
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
        for(int i = 0; i < InPlay.Count; i++)
        {
            MinionParent minion = (MinionParent)InPlay[i];
            if(minion.CardEffect == MinionParent.effect.coordinate && minion.CoordinateAbility.Awarded == false)
            {
                int coordNum = -1;
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