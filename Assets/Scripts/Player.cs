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

    [Header("Move Timer")]
    [SerializeField] private float moveCooldownTime = 1.5f;

    private int energy;
    private List<NewVirtualCardParent> deck = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> hand = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> inPlay = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> discard = new List<NewVirtualCardParent>();

    private float timer = 0f;
    private float moveCooldownTimer = 0f;

    public int Health { get { return health; } set { health = value; } }
    public int Energy { get { return energy; } set { energy = value; } }
    public int MaxEnergy { get { return maxEnergy; } }

    public float EnergyTimer { get { return timer; } }
    public float TimeForEnergy { get { return timeForEnergy; } }
    public float EnergyTimerRemaining { get { return Mathf.Max(0f, timeForEnergy - timer); } }

    public float MoveCooldownTime { get { return moveCooldownTime; } }
    public float MoveCooldownRemaining { get { return Mathf.Max(0f, moveCooldownTimer); } }
    public bool CanMove { get { return moveCooldownTimer <= 0f; } }

    public List<NewVirtualCardParent> Deck { get { return deck; } set { deck = value; } }
    public List<NewVirtualCardParent> Hand { get { return hand; } set { hand = value; } }
    public List<NewVirtualCardParent> InPlay { get { return inPlay; } set { inPlay = value; } }
    public List<NewVirtualCardParent> Discard { get { return discard; } set { discard = value; } }

    private void Start()
    {
        energy = startingEnergy;

        if (isPlayerTwo)
        {
            Networking.PlayerTwo = this;
        }
        else
        {
            Networking.PlayerOne = this;
        }
    }

    private void Update()
    {
        GainEnergyOverTime();
        UpdateMoveCooldown();
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

    private void UpdateMoveCooldown()
    {
        if (moveCooldownTimer > 0f)
        {
            moveCooldownTimer -= Time.deltaTime;

            if (moveCooldownTimer < 0f)
            {
                moveCooldownTimer = 0f;
            }
        }
    }

    public void RegisterAction()
    {
        timer = 0f;
        moveCooldownTimer = moveCooldownTime;
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

            if (minion.CardEffect == MinionParent.effect.coordinate && minion.CoordinateAbility.Awarded == false)
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

                if (coordNum >= minion.CoordinateAbility.NumToHit)
                {
                    minion.CoordinateAbility.RewardAbility(minion);
                }
            }
        }
    }
}