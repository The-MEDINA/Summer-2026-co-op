using cardIndex;
using System.Collections.Generic;
using UnityEngine;
using Network;

public class MinionParent : NewVirtualCardParent
{
    public enum effect //the card's ability
    {
        none,
        deathtouch,
        explode,
        haste,
        sloth,
        coordinate,
        twoAttacks,
        aoe,
        overkill,
        duplicate,
        heal,
        thorns,
        spawnToken,
        spwnTokOnPlay,
        guard,
        lifelink,
        frozen,
        apoptosis,
        hidden,
        split,
        healOnPlay,
        statsUp,
        gainEnergy
    }

    public enum equipment //used to keep track of all the stat changes a card has recieved, so they can be changed/reused/displayed/etc
    {
        m16,
        iHungy,
        terrorize,
        fishTreat,
        empower,
        curse,
        hex,
        catnap,
        distraction,
        geneticEngineering,
        hide
    }

    private int startingHealth;
    private int startingDamage;
    private int health;
    private int damage;
    private effect cardEffect;
    private bool hasGuard = false;
    private bool isDead = false;
    [SerializeField] private bool canAttack = false;
    private CoordinateAbilityScript coordinateAbility;
    private List<equipment> equipmentList;
    private bool isHidden;
    private bool hasStatsUp;

    public int Health { get { return health; } set { health = value; } }
    public int Damage { get { return damage; } set { damage = value; } }
    public bool HasGuard { get { return hasGuard; } set { hasGuard = value; } }
    public bool IsDead { get { return isDead; } }
    public effect CardEffect { get { return cardEffect; } set { cardEffect = value; } }
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    public CoordinateAbilityScript CoordinateAbility { get { return coordinateAbility; } set { coordinateAbility = value; }  }
    public int StartingHealth { get { return startingHealth; } set { startingHealth = value; } }
    public int StartingDamage { get { return startingDamage; } set { startingDamage = value; } }
    public List<equipment> EquipmentList { get { return equipmentList; } set { equipmentList = value; } }
    public bool IsHidden { get { return isHidden; } set { isHidden = value; } }
    public bool HasStatsUp { get { return hasStatsUp; } set { hasStatsUp = value; }  }

    #region SFX_EVENTS
    public delegate void Action(effect cardEffect);
    public event Action cardAction;
    public delegate void Dies(string faction);
    public event Dies cardDeath;
    #endregion
    /// <summary>
    /// hard codes a minion
    /// </summary>
    /// <param name="cost">energy cost of the minion</param>
    /// <param name="health">starting health of the minion</param>
    /// <param name="damage">damage value for the minion</param>
    /// <param name="name">the minion's name</param>
    /// <param name="cardType">what type of card the minion is (it's minion, sometimes token)</param>
    /// <param name="cardEffect">what ability the minion has</param>
    /// <param name="cardLocation">where the minion is rn (it's Hand, unless it's token, whereas it's inPlay)</param>
    public MinionParent(int cost, int health, int damage, string name, type cardType, effect cardEffect, location cardLocation) 
        : base(cost, name, cardType, cardLocation)
    {
        this.health = health;
        this.startingHealth = health;
        this.damage = damage;
        this.startingDamage = damage;
        this.cardEffect = cardEffect;
        if(this.cardEffect == effect.coordinate) { CoordinateAbility = new CoordinateAbilityScript(this.CardName); }
        equipmentList = new List<equipment>();
        if (CardType == NewVirtualCardParent.type.token) { CardLocation = NewVirtualCardParent.location.inPlay; }
        if (cardEffect == effect.hidden) { IsHidden = true; }
        if (cardEffect == effect.statsUp) { HasStatsUp = true; }
    }

    /// <summary>
    /// Construct a card using only its name. It should be noted that this constructor will set any int value that's not defined as -1.
    /// </summary>
    /// <param name="name">Name of the card.</param>
    /// <param name="cardLocation">location of the card.</param>
    public MinionParent(string name, location cardLocation) : base(name, cardLocation)
    { 
        Details cardDetails = cardIndex.Index.GetDetails(name);
        health = cardDetails.health;
        startingHealth = cardDetails.health;
        damage = cardDetails.damage;
        startingDamage = cardDetails.damage;
        if (cardDetails.ability == effect.guard)
        {
            hasGuard = true;
            if (cardDetails.secondAbility != effect.none) cardEffect = cardDetails.secondAbility;
        }
        else
        {
            cardEffect = cardDetails.ability;
        }
        equipmentList = new List<equipment>();
        if (this.cardEffect == effect.coordinate) { CoordinateAbility = new CoordinateAbilityScript(this.CardName); }
        if (CardType == NewVirtualCardParent.type.token) { CardLocation = NewVirtualCardParent.location.inPlay; }
        if (cardEffect == effect.hidden) { IsHidden = true; }
        if (cardEffect == effect.statsUp || cardDetails.secondAbility == effect.statsUp) { HasStatsUp = true; }
    }

    /// <summary>
    /// resolve any onPlay abilities
    /// </summary>
    public void OnPlay()
    {
        if (CardEffect == effect.duplicate) //create a temporary clone of a minion
        {
            UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(new MinionParent(0, Health, Damage, CardName, 
                NewVirtualCardParent.type.token, MinionParent.effect.none, NewVirtualCardParent.location.inPlay));
        }
        if (CardEffect == effect.spwnTokOnPlay) //spawn token creatures when entering play
        {
            UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(cardIndex.Index.CreateCard("Kitten", location.inPlay));
            UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(cardIndex.Index.CreateCard("Kitten", location.inPlay));
        }
        if (CardEffect == effect.frozen || CardName == "Frozen Horror")
        {
            UnityObject.GetComponent<CardClickHandler>().SetSpeed(CardClickHandler.speed.frozen);
            UnityObject.GetComponent<CardClickHandler>().ResetTimer();
            UnityObject.GetComponent<CardUIManager>().AddProgress(5f);
        }
        if (CardEffect == effect.healOnPlay)
        {
            UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Health += Damage;
            Debug.Log($"Healed player for {Damage} health. (now {UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Health})");
        }
    }

    /// <summary>
    /// attack an opposing minion
    /// </summary>
    /// <param name="target">the minion being attacked</param>
    public void Attack(MinionParent target)
    {
        if (canAttack)
        {
            if (target == null || isDead || target.IsDead || CheckGuard(target)) //parameters upon which attack is impossible
            {
                return;
            }
            else if (CardEffect == effect.heal) //if the card has the heal ability then target will be healed, not damaged
            {
                target.Health += Damage;
                if(target.Health > target.StartingHealth) { target.Health = target.StartingHealth; }
            }
            else
            {
                if (Damage != 0) 
                { 
                    target.TakeDamage(this, Damage, false);
                }
            }

            cardAction.Invoke(cardEffect);

            if (CardEffect == effect.spawnToken)
            { //if this card has the ability to spawn tokens upon attack (i.e. Vampire Cat) it's resolved here
                UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(cardIndex.Index.CreateCard("Kitten", location.inPlay));
            }
            if(CardEffect == effect.lifelink)
            { //lifelink is tied directly to cost. This isn't fantasic implementation, but it doubles as locking the ability balance wise
                int lLDamage = Cost / 2;
                UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Health += lLDamage;
            }

            if (IsHidden) { IsHidden = false; }
            canAttack = false;
            UnityObject.GetComponent<CardUIManager>().ResetProgress();
        }
    }

    /// <summary>
    /// an alt version of TakeDamage used by SpellParent, as spells cannot take revenge damage (thorns, explode, etc) and spells cannot
    /// be taken as the "attacker" parameter
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        Health -= damage;
        UnityObject.GetComponent<CardClickHandler>().PopUpDamageText(damage);
        UnityObject.GetComponent<CardUIManager>().RefreshCardUI();

        if (Health <= 0)
        {
            Health = 0;
            Death();
        }
    }

    /// <summary>
    /// causes this minion to take damage when attacked, and deal revenge dammage to an attacker if applicable
    /// </summary>
    /// <param name="attacker">the minion that attacked this one</param>
    /// <param name="damage">how much damage is being dealt</param>
    /// <param name="wasRevenge">records whether or not this was triggered by revenge damage to prevent infinite loops</param>
    public void TakeDamage(MinionParent attacker, int damage, bool wasRevenge)
    {
        if(attacker.CardEffect == effect.deathtouch) //instantly kills b/c of deathtouch
        {
            Health = 0;
        }
        
        Health -= damage;
        UnityObject.GetComponent<CardClickHandler>().PopUpDamageText(damage);

        if (CardEffect == effect.thorns && !wasRevenge) //covers thorns damage
        {
            int thornsDamage = 0;
            switch (CardName)
            {
                case "Shoto Cat":
                    {
                        thornsDamage = 3;
                        break;
                    }

                case "Chonkmeister":
                    {
                        thornsDamage = 2;
                        break;
                    }

                case "Nacho Cat":
                default:
                    {
                        thornsDamage = 1;
                        break;
                    }
            }
            attacker.TakeDamage(this, thornsDamage, true);
        }

        UnityObject.GetComponent<CardUIManager>().RefreshCardUI();

        if (Health <= 0) //if this minion has died
        {
            if (attacker.CardEffect == effect.overkill)
            { 
                //if the attacker had overkill and the target (this) dies, damage is dealt to the target's owner player
                UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.TakeDamage(-1 * Health);
                Debug.Log(UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Health);
            }
            Health = 0;

            if (cardEffect == effect.explode) //covers explode damage
            {
                int explodeDamage = 0;
                switch(CardName)
                {
                    case "Mad Scientist Cat":
                        {
                            explodeDamage = 3;
                            break;
                        }
                    case "Spectral Beings":
                        {
                            explodeDamage = 5;
                            break;
                        }
                    case "Exploding Cat":
                    default:
                        {
                            explodeDamage = 1;
                            break;
                        }
                }
                attacker.TakeDamage(this, explodeDamage, true); 
            }
            Death();
        }
    }

    /// <summary>
    /// causes code to run upon the death of a minion
    /// </summary>
    public void Death()
    {
        cardDeath.Invoke(Faction);
        isDead = true;
        if (UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.IsPlayerTwo && Networking.CurrentState != state.paused)
        {
            Networking.SendCardDeath(UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.IsPlayerTwo, this);
        }

        Debug.Log(StartingHealth);
        if (CardEffect == effect.split && StartingHealth / 2 > 0)
        {
            UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(new MinionParent(0, StartingHealth / 2,
                Damage / 2, CardName, NewVirtualCardParent.type.token, MinionParent.effect.split, NewVirtualCardParent.location.inPlay));
            UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(new MinionParent(0, StartingHealth / 2,
                Damage / 2, CardName, NewVirtualCardParent.type.token, MinionParent.effect.split, NewVirtualCardParent.location.inPlay));
        }

        if (CardType == NewVirtualCardParent.type.token) 
        {
            //if this card is a token it should not enter discard
            UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.InPlay.Remove(this);
            return; 
        }
        CardLocation = location.discard;
        UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.MoveCardToDiscard(this);
        SFXManager.Instance.UnregisterCard(this);
    }

    /// <summary>
    /// attack method for cards that have aoe (hit all opposing cards)
    /// </summary>
    /// <param name="targetList">list of all opposing cards</param>
    /// <param name="isSecond">records whether this was a secondary attack from a TwoAttackParent (i.e. Magic Cat)</param>
    public void AOEAttack(List<NewVirtualCardParent> targetList, bool isSecond)
    {
        //isSecond is a fast work around to TwoAttackParent's complications
        if(canAttack && (cardEffect == effect.aoe || isSecond))
        {
            if (targetList == null)
            {
                return;
            }
            cardAction.Invoke(cardEffect);
            //hits every minion inPlay
            for (int i = targetList.Count - 1; i >= 0; i--)
            {
                Debug.Log(targetList[i].CardName);
                if (targetList[i] is MinionParent)
                {
                    MinionParent enemyTarget = (MinionParent)targetList[i];
                    enemyTarget.TakeDamage(this, Damage, false);
                    Debug.Log(enemyTarget.CardName);
                }
            }
            canAttack = false;
            UnityObject.GetComponent<CardUIManager>().ResetProgress();
        }
    }

    /// <summary>
    /// adds a piece of equipment to a minion's equipment list
    /// </summary>
    /// <param name="addToList">equipment piece being added to the list</param>
    public void AddEquipment(equipment addToList)
    {
        equipmentList.Add(addToList);
    }

    /// <summary>
    /// checks to make sure there are no minions with guard that need to be attacked first
    /// </summary>
    /// <param name="target">the target of the Attack(MinionParent target)</param>
    /// <returns>bool stating whether or not the target can be attacked</returns>
    public bool CheckGuard(MinionParent target)
    {
        if (target.hasGuard) { return false; }

        for (int i = 0; i < target.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.InPlay.Count; i++)
        {
            if (target.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.InPlay[i] is MinionParent)
            {
                MinionParent otherMinion = (MinionParent)target.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.InPlay[i];
                if (otherMinion.hasGuard) { return true; }
            }
        }

        return false;
    }

    public void ForceActionSFX()
    {
        cardAction.Invoke(cardEffect);
    }
}
