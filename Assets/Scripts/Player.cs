using System.Collections.Generic;
using UnityEngine;
using Network;

public class Player : MonoBehaviour
{
    [SerializeField] private int health = 50;
    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private int startingEnergy = 10;
    [SerializeField] private float timeForEnergy = 5f;
    [SerializeField] private bool isPlayerTwo = false;

    [SerializeField] private float moveCooldownTime = 1.5f;

    private int energy;
    private List<NewVirtualCardParent> deck = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> hand = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> inPlay = new List<NewVirtualCardParent>();
    private List<NewVirtualCardParent> discard = new List<NewVirtualCardParent>();
    private CommanderCardScript commanderCard;

    private float timer = 0f;

    private bool hasThorns = false;

    public int Health { get { return health; } set { health = value; } }
    public int Energy { get { return energy; } set { energy = value; } }
    public bool IsPlayerTwo { get { return isPlayerTwo; } }
    public int MaxEnergy { get { return maxEnergy; } }

    public float EnergyTimer { get { return timer; } }
    public float TimeForEnergy { get { return timeForEnergy; } }
    public float EnergyTimerRemaining { get { return Mathf.Max(0f, timeForEnergy - timer); } }

    public List<NewVirtualCardParent> Deck { get { return deck; } set { deck = value; } }
    public List<NewVirtualCardParent> Hand { get { return hand; } set { hand = value; } }
    public List<NewVirtualCardParent> InPlay { get { return inPlay; } set { inPlay = value; } }
    public List<NewVirtualCardParent> Discard { get { return discard; } set { discard = value; } }
    public CommanderCardScript CommanderCard { get { return commanderCard; } set { commanderCard = value; } }

    private void Start()
    {
        energy = startingEnergy;
        if (isPlayerTwo) Networking.PlayerTwo = this;
        else Networking.PlayerOne = this;

        // need player two to be able to act whenever on player 1's side.
        // Easiest way to do this right now is to just make them never run out of energy or have to wait on the timer.
        // doing this should prevent any situations where player 2 does something on their screen, but it doesn't happen on player 1's screen because they didn't have enough energy.
        // Ideally both games would be perfectly in sync somehow so this situation would never happen, but this is an easy workaround just to get it done.
        //TLDR: under no circumstances remove or alter this line without expressed approval from Dave, even if it seems odd - Jake
        // if (isPlayerTwo) energy = 999; 
    }

    private void Update()
    {
        // Don't run if network manager is trying to resolve a desync.
        if (Networking.CurrentState == state.paused) return;
        GainEnergyOverTime();
    }

    private void GainEnergyOverTime()
    {
        if (timer >= timeForEnergy)
        {
            GainEnergy(1);
            // if (!isPlayerTwo) 
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
            if (!IsPlayerTwo) return false;
            else
            {
                Debug.LogWarning("Overriding player two SpendEnergy to prevent desync.");
            }
        }

        //if (!isPlayerTwo)
            Energy -= amount;
        if (Energy < 0)
        {
            Energy = 0;
            if (IsPlayerTwo) Debug.LogWarning("Energy fell below 0! Setting it back to prevent desync.");
        }
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

    public void TakeDamage(int damage, MinionParent minionAttacker)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            Death();
        }

        if (Health > 0 && hasThorns)//needs tested
        {
            minionAttacker.TakeDamage(1);
        }
    }

    public void Death()
    {
        Debug.Log(gameObject.name + " has lost.");
    }

    /// <summary>
    /// goes through every unit inPlay and checks if they 1) have coordinate, 2) if there are enough other units with coordinate to trigger
    /// the reward, and 3) if there is, apply the reward to that unit
    /// </summary>
    private void CheckCoordinate()
    {
        for (int i = 0; i < InPlay.Count; i++)
        {
            if (InPlay[i] is MinionParent)
            {
                MinionParent minion = (MinionParent)InPlay[i];

                if (minion.CardEffect == MinionParent.effect.coordinate && minion.CoordinateAbility.Awarded == false)
                {
                    int coordNum = 0;

                    for (int j = 0; j < InPlay.Count; j++)
                    {
                        MinionParent newMinion = (MinionParent)InPlay[j];

                        if (newMinion.CardEffect == MinionParent.effect.coordinate && newMinion.UnityObject.activeSelf)
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
}