using Network;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectionManager : MonoBehaviour
{
    public static CardSelectionManager Instance;

    [Header("Player 1")]
    [SerializeField] private Player player1;
    [SerializeField] private GameObject player1CardField;
    [SerializeField] private HandUIManager player1HandUI;
    [SerializeField] private GameObject player1CommanderSquare;
    [SerializeField] private Battleground player1Battleground;

    [Header("Player 2")]
    [SerializeField] private Player player2;
    [SerializeField] private GameObject player2CardField;
    [SerializeField] private HandUIManager player2HandUI;
    [SerializeField] private GameObject player2CommanderSquare;
    [SerializeField] private Battleground player2Battleground;

    [Header("Field Layout")]
    [SerializeField] private float player1StartX = -5f;
    [SerializeField] private float player2StartX = -5f;
    [SerializeField] private float cardSpacing = 2f;

    [SerializeField] private bool isLocalTesting = false;

    [Header("Sound Effects")]
    [SerializeField] private SFXManager sfxManager;

    private CardClickHandler selectedCardObject;

    public CardClickHandler SelectedCardObject
    {
        get { return selectedCardObject; }
        set { selectedCardObject = value; }
    }
    public SFXManager SfxManager { get { return sfxManager; } }
    public Player Player1 { get { return player1; } }
    public Player Player2 { get { return player2; } }

    public GameObject Player2CommanderSquare { get { return player2CommanderSquare; } }
    public bool IsLocalTesting { get { return isLocalTesting; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // check for a commander in deck builder if it exists, else default to zoomie
        DeckInstanceDeckbuilderScript dBDeck = FindAnyObjectByType<DeckInstanceDeckbuilderScript>();
        if (dBDeck != null && dBDeck.Commander != "")
        {
            cardIndex.Index.AttachCommanderCard(player1CommanderSquare, dBDeck.Commander, player1Battleground);
            if (player1CommanderSquare.GetComponent<DeckbuilderCard>() != null) 
            { 
                player1CommanderSquare.GetComponent<DeckbuilderCard>().UpdateUI(); 
            }
        }
        else
        {
            Debug.LogWarning("No commander found for player 1, defaulting to Zoomie.");
            cardIndex.Index.AttachCommanderCard(player1CommanderSquare, "Sergeant Zoomie", player1Battleground);
            if (player1CommanderSquare.GetComponent<DeckbuilderCard>() != null) 
            { 
                player1CommanderSquare.GetComponent<DeckbuilderCard>().UpdateUI(); 
            }
        }

        // check for a commander in networking
        if (Networking.P2CommanderName != "")
        {
            cardIndex.Index.AttachCommanderCard(player2CommanderSquare, Networking.P2CommanderName, player2Battleground);
            Networking.P2CommanderName = "";
            if (player2CommanderSquare.GetComponent<DeckbuilderCard>() != null) 
            { 
                player2CommanderSquare.GetComponent<DeckbuilderCard>().UpdateUI(); 
            }
        }
        else
        {
            Debug.LogWarning("No commander found for player 2, defaulting to Zoomie.");
            cardIndex.Index.AttachCommanderCard(player2CommanderSquare, "Sergeant Zoomie", player2Battleground);
            if (player2CommanderSquare.GetComponent<DeckbuilderCard>() != null) 
            { 
                player2CommanderSquare.GetComponent<DeckbuilderCard>().UpdateUI(); 
            }
        }
    }

    public void SelectCard(CardClickHandler clickedCard, PointerEventData eventData)
    {
        // Don't run if network manager is trying to resolve a desync.
        if (Networking.CurrentState == state.paused) return;

        if (clickedCard == null || clickedCard.CardData == null)
        {
            Debug.LogWarning("Clicked card has no card data.");
            return;
        }

        if (selectedCardObject != null && selectedCardObject != clickedCard)
        {
            if (selectedCardObject.CardData is MinionParent)
            {
                MinionParent minion = (MinionParent)selectedCardObject.CardData;

                if (minion.CardEffect == MinionParent.effect.twoAttacks && eventData.button == PointerEventData.InputButton.Right)
                {
                    TryAttackTarget(clickedCard, true);
                }
                else
                {
                    TryAttackTarget(clickedCard, false);
                }
            }
            else if (selectedCardObject.CardData is SpellParent)
            {
                SpellParent spell = (SpellParent)selectedCardObject.CardData;

                if (spell.Target != SpellParent.spellTarget.none && (spell.Target != SpellParent.spellTarget.owner && spell.Target != SpellParent.spellTarget.opponent))
                {
                    TrySpellTarget(clickedCard);
                }
                else
                {
                    TrySpellNoTarget();

                    if (!spell.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.IsPlayerTwo)
                    {
                        Networking.SendCardArray(spell.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.InPlay, NewVirtualCardParent.location.inPlay);
                    }
                }
            }

            return;
        }

        if (selectedCardObject == clickedCard)
        {
            if (clickedCard.CardData is MinionParent)
            {
                ActivateCard(clickedCard);
                return;
            }

            if (clickedCard.CardData is SpellParent)
            {
                TrySpellNoTarget();

                if (!clickedCard.OwnerPlayer.IsPlayerTwo)
                {
                    Networking.SendCardArray(clickedCard.OwnerPlayer.InPlay, NewVirtualCardParent.location.inPlay);
                }
            }
        }

        // when testing locally, enable bool isLocalTesting in inspector on CardSelectionManager.Ins, when playing online, disable it - Jacob
        if (!clickedCard.OwnerPlayer.IsPlayerTwo || IsLocalTesting)
        {
            selectedCardObject = clickedCard;
            selectedCardObject.SetSelectedVisual(true);
            Debug.Log("Selected card: " + clickedCard.CardData.CardName);
        }
        else
        {
            Debug.LogWarning("Playing player 2's cards are not allowed.");
        }
    }

    private void ActivateCard(CardClickHandler cardObject)
    {
        if (cardObject == null || cardObject.CardData == null)
        {
            ClearSelection();
            return;
        }

        if (cardObject.CardData.CardLocation == NewVirtualCardParent.location.hand)
        {
            PlayCardToBattleground(cardObject);
        }

        // send the current inplay array to peer.
        if (!cardObject.OwnerPlayer.IsPlayerTwo)
        {
            Networking.SendCardArray(cardObject.OwnerPlayer.InPlay, NewVirtualCardParent.location.inPlay);
        }

        ClearSelection();
    }

    public void PlayCardToBattleground(CardClickHandler cardObject)
    {
        if (cardObject == null || cardObject.CardData == null)
        {
            Debug.LogWarning("Tried to play a null card.");
            return;
        }

        Player owner = cardObject.OwnerPlayer;

        if (owner == null)
        {
            Debug.LogWarning("This card has no owner player.");
            return;
        }

        if (!owner.CanAfford(cardObject.CardData))
        {
            Debug.Log("Cannot play card. Not enough energy.");

            if (owner.IsPlayerTwo)
            {
                Debug.LogWarning("Overriding player two CanAfford to prevent desync.");
            }
            else
            {
                return;
            }
        }

        if (!owner.SpendEnergy(cardObject.CardData.Cost))
        {
            return;
        }

        if (owner == player1)
        {
            // tell the peer you moved a card to Inplay.
            Networking.SendCardMove(
                cardObject.CardData,
                NewVirtualCardParent.location.hand,
                cardObject.OwnerPlayer.Hand.IndexOf(cardObject.CardData),
                NewVirtualCardParent.location.inPlay
            );
        }

        owner.MoveCardToInPlay(cardObject.CardData);

        if (owner == player1)
        {
            if (player1HandUI != null)
            {
                player1HandUI.RemoveCardFromHand(cardObject.gameObject);
            }

            RepositionInPlayCards(player1);
        }
        else if (owner == player2)
        {
            if (player2HandUI != null)
            {
                player2HandUI.RemoveCardFromHand(cardObject.gameObject);
            }

            RepositionInPlayCards(player2);
        }
        else
        {
            Debug.LogWarning("Card owner does not match Player 1 or Player 2.");
        }

        for (int i = 0; i < owner.InPlay.Count; i++)
        {
            if (owner.InPlay[i] != null && owner.InPlay[i].UnityObject != null)
            {
                RefreshCardVisual(owner.InPlay[i].UnityObject.GetComponent<CardClickHandler>());
            }
        }

        if (cardObject.CardData is MinionParent)
        {
            MinionParent minion = (MinionParent)cardObject.CardData;
            minion.OnPlay();
        }

        cardObject.InPlay = true;
        Debug.Log("Card moved to battleground. Energy left: " + owner.Energy);
    }

    private void MoveCardToField(CardClickHandler cardObject, GameObject field, float startX, int position)
    {
        if (field == null)
        {
            Debug.LogWarning("Missing card field reference.");
            return;
        }

        cardObject.transform.position = new Vector3(
            startX + (cardSpacing * position),
            field.transform.position.y,
            -0.1f
        );
    }

    // Hi Brandon
    // network manager needs this so this is public now
    // if I shouldn't do this tell me and i'll find some alternate way to get this done - Dave
    public void RepositionInPlayCards(Player owner)
    {
        if (owner == null)
        {
            return;
        }

        GameObject field = null;
        float startX = 0f;

        if (owner == player1)
        {
            field = player1CardField;
            startX = player1StartX;
        }
        else if (owner == player2)
        {
            field = player2CardField;
            startX = player2StartX;
        }
        else
        {
            return;
        }

        if (field == null)
        {
            Debug.LogWarning("Missing card field reference.");
            return;
        }

        int visiblePosition = 0;

        for (int i = 0; i < owner.InPlay.Count; i++)
        {
            NewVirtualCardParent cardData = owner.InPlay[i];

            if (cardData == null || cardData.UnityObject == null)
            {
                continue;
            }

            GameObject cardObject = cardData.UnityObject;

            if (!cardObject.activeInHierarchy)
            {
                continue;
            }

            CardClickHandler clickHandler = cardObject.GetComponent<CardClickHandler>();

            if (clickHandler == null)
            {
                continue;
            }

            MoveCardToField(clickHandler, field, startX, visiblePosition);
            visiblePosition++;
        }
    }

    public void TryAttackTarget(CardClickHandler targetCard, bool wasSecondAttack)
    {
        if (selectedCardObject == null || targetCard == null)
        {
            if (selectedCardObject != null && selectedCardObject.OwnerPlayer != null && selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Player two's attacking or target card was null");
            }

            ClearSelection();
            return;
        }

        Player attackingOwner = selectedCardObject.OwnerPlayer;

        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Card must be in play before it can attack.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Player two is attacking with card not in play");
            }

            ClearSelection();
            return;
        }

        if (targetCard.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Target card must be in play.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Player two is attacking a card that's not in play");
            }

            ClearSelection();
            return;
        }

        MinionParent attacker = selectedCardObject.CardData as MinionParent;
        MinionParent target = targetCard.CardData as MinionParent;

        bool isThisMinionATwoAttackHealer = false;

        if (target.IsHidden)
        {
            Debug.Log("Target card must not be hidden.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                //Networking.DesyncWarning("Player two is attacking a card that's not in play");
                //I don't know if you need this, Dave - Jake
            }

            ClearSelection();
            return;
        }

        if (attacker is TwoAttackParent)
        {
            TwoAttackParent thisTestSucksBro = (TwoAttackParent)attacker;

            if (thisTestSucksBro.SecondaryCardEffect == MinionParent.effect.heal)
            {
                isThisMinionATwoAttackHealer = true;
            }
        }

        if (selectedCardObject.OwnerPlayer == targetCard.OwnerPlayer && attacker.CardEffect != MinionParent.effect.heal && !isThisMinionATwoAttackHealer)
        {
            Debug.Log("You cannot attack your own card.");
            ClearSelection();
            return;
        }

        if (selectedCardObject.OwnerPlayer != targetCard.OwnerPlayer && attacker.CardEffect == MinionParent.effect.heal)
        {
            Debug.Log("You cannot heal your opponent's card.");
            ClearSelection();
            return;
        }

        if (attacker == null || target == null)
        {
            Debug.Log("Only minion cards can attack right now.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Only minion cards can attack right now for player two");
            }

            ClearSelection();
            return;
        }

        if (!selectedCardObject.OwnerPlayer.IsPlayerTwo)
        {
            Networking.SendCardAttack(attacker, target, wasSecondAttack);
        }

        if (attacker is TwoAttackParent)
        {
            TwoAttackParent twoAttackMinion = (TwoAttackParent)attacker;

            if (twoAttackMinion.SecondaryCardEffect == MinionParent.effect.heal && ((targetCard.OwnerPlayer == selectedCardObject.OwnerPlayer &&
                !wasSecondAttack) || (targetCard.OwnerPlayer != selectedCardObject.OwnerPlayer && wasSecondAttack)))
            {
                ClearSelection();
                return;
            }

            if (twoAttackMinion.SecondaryCardEffect == MinionParent.effect.aoe)
            {
                if (wasSecondAttack)
                {
                    twoAttackMinion.CheckAOEAttack(2, target, targetCard.OwnerPlayer.InPlay);
                    for (int i = 0; i < targetCard.OwnerPlayer.InPlay.Count; i++)
                    {
                        RefreshCardVisual(targetCard.OwnerPlayer.InPlay[i].UnityObject.GetComponent<CardClickHandler>());
                    }
                }
                else
                {
                    twoAttackMinion.CheckAttack(1, target);
                }
            }
            else
            {
                if (wasSecondAttack)
                {
                    twoAttackMinion.CheckAttack(2, target);
                }
                else
                {
                    twoAttackMinion.CheckAttack(1, target);
                }
            }

            Debug.Log("Attacked enemy card. Enemy health: " + target.Health);
            selectedCardObject.OwnerPlayer.RegisterAction();
            RefreshCardVisual(selectedCardObject);
            RefreshCardVisual(targetCard);
            ClearSelection();
            return;
        }

        if (attacker.CardEffect == MinionParent.effect.aoe)
        {
            attacker.AOEAttack(targetCard.OwnerPlayer.InPlay, false);
            selectedCardObject.OwnerPlayer.RegisterAction();
            
            for(int i =0; i < targetCard.OwnerPlayer.InPlay.Count; i++)
            {
                RefreshCardVisual(targetCard.OwnerPlayer.InPlay[i].UnityObject.GetComponent<CardClickHandler>());
            }

            RefreshCardVisual(selectedCardObject);
            RefreshCardVisual(targetCard);
            ClearSelection();
            return;
        }

        attacker.Attack(target);

        if (attackingOwner != null)
        {
            attackingOwner.RegisterAction();
        }

        RefreshCardVisual(selectedCardObject);
        RefreshCardVisual(targetCard);

        Debug.Log(attacker.CardName + " attacked " + target.CardName + ". Target health: " + target.Health);

        ClearSelection();
    }

    public void TrySpellTarget(CardClickHandler targetCard)
    {
        if (selectedCardObject == null || targetCard == null)
        {
            if (selectedCardObject != null && selectedCardObject.OwnerPlayer != null && selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Player two's attacking or target card was null");
            }

            ClearSelection();
            return;
        }

        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.hand)
        {
            Debug.Log("Card must be in your hand before it can be played.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Player two is playing card from hand");
            }

            ClearSelection();
            return;
        }

        SpellParent attacker = selectedCardObject.CardData as SpellParent;
        Player owner = selectedCardObject.OwnerPlayer;
        MinionParent target = targetCard.CardData as MinionParent;
        // SpellParent targetSpell = targetCard.CardData as SpellParent;

        if (targetCard.CardData.CardLocation != NewVirtualCardParent.location.inPlay && attacker.Effect != SpellParent.spellEffect.copy)
        {
            Debug.Log("Target card must be in play.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Player two is targetting card not in play");
            }

            ClearSelection();
            return;
        }

        if (attacker == null || targetCard.CardData == null)
        {
            Debug.Log("Only minion cards can attack right now.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Only minion cards can attack right now for player two");
            }

            ClearSelection();
            return;
        }

        if (!owner.CanAfford(attacker))
        {
            Debug.Log("Cannot play card. Not enough energy.");

            if (owner.IsPlayerTwo)
            {
                Debug.LogWarning("Overriding player two CanAfford to prevent desync.");
            }
            else
            {
                return;
            }
        }

        if (owner == targetCard.OwnerPlayer && attacker.Target == SpellParent.spellTarget.enemyCards)
        {
            Debug.Log("You cannot play spell targetting enemies on allies.");
            ClearSelection();
            return;
        }


        if (targetCard.OwnerPlayer != owner && attacker.Target == SpellParent.spellTarget.allyCards)
        {
            Debug.Log("You cannot play spell targetting allies on enemies.");
            ClearSelection();
            return;
        }

        if (!owner.SpendEnergy(attacker.Cost))
        {
            return;
        }

        if (!selectedCardObject.OwnerPlayer.IsPlayerTwo)
        {
            Networking.SendCardAttack(attacker, targetCard.CardData, false);
        }

        if (attacker.Target == SpellParent.spellTarget.allEnemies)
        {
            attacker.OnPlayAOE(target.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.InPlay);
        }
        else if (attacker.Target == SpellParent.spellTarget.allAllies)
        {
            attacker.OnPlayAOE(attacker.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.InPlay);
        }
        else if (attacker.Target == SpellParent.spellTarget.any)
        {
            attacker.OnPlayAny(targetCard.CardData);
        }
        else
        {
            attacker.OnPlay(target);
        }

        Debug.Log(attacker.CardName + " played on " + targetCard.CardData.CardName);

        RemoveSelectedCardFromHandUI(owner);
        owner.MoveCardToDiscard(attacker);
        selectedCardObject.gameObject.SetActive(false);

        RefreshCardVisual(selectedCardObject);
        RefreshCardVisual(targetCard);

        selectedCardObject.OwnerPlayer.RegisterAction();

        ClearSelection();
    }

    public void TrySpellNoTarget()
    {
        if (selectedCardObject == null)
        {
            ClearSelection();
            return;
        }

        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.hand)
        {
            Debug.Log("Card must be in your hand before it can be played.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Player two is playing card from hand");
            }

            ClearSelection();
            return;
        }

        SpellParent attacker = selectedCardObject.CardData as SpellParent;
        Player owner = selectedCardObject.OwnerPlayer;

        if (attacker == null || (attacker.Target != SpellParent.spellTarget.none && (attacker.Target != SpellParent.spellTarget.owner && attacker.Target != SpellParent.spellTarget.opponent)))
        {
            Debug.Log("Only spell cards can be played this way.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Only spell cards can be played this way for player two");
            }

            ClearSelection();
            return;
        }

        if (!owner.CanAfford(attacker))
        {
            Debug.Log("Cannot play card. Not enough energy.");

            if (owner.IsPlayerTwo)
            {
                Debug.LogWarning("Overriding player two CanAfford to prevent desync.");
            }
            else
            {
                return;
            }
        }

        if (!owner.SpendEnergy(attacker.Cost))
        {
            return;
        }

        attacker.OnPlay();

        if (!selectedCardObject.OwnerPlayer.IsPlayerTwo)
        {
            Networking.SendCardAttack(attacker, null, true);
        }

        RemoveSelectedCardFromHandUI(owner);
        owner.MoveCardToDiscard(attacker);
        selectedCardObject.gameObject.SetActive(false);

        RefreshCardVisual(selectedCardObject);

        Debug.Log(attacker.CardName + " played");
        selectedCardObject.OwnerPlayer.RegisterAction();

        ClearSelection();
    }

    public void TryAttackPlayer(bool wasSecondAttack)
    {
        // check that card exists.
        if (selectedCardObject == null)
        {
            if (selectedCardObject != null && selectedCardObject.OwnerPlayer != null && selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Player two's attacking or target card was null");
            }

            ClearSelection();
            return;
        }

        // grab players
        Player attackingOwner = selectedCardObject.OwnerPlayer;
        if (attackingOwner == null)
        {
            Debug.Log("Attacking card has no owner.");
            return;
        }
        Player opposingPlayer = null;
        if (attackingOwner.IsPlayerTwo) opposingPlayer = player1;
        else opposingPlayer = player2;

        // make sure card is in play.
        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Card must be in play before it can attack.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Player two is attacking with card not in play");
            }

            ClearSelection();
            return;
        }

        // convert card to minion or twoattack
        MinionParent attacker = selectedCardObject.CardData as MinionParent;
        bool isThisMinionATwoAttackHealer = false;

        if (attacker is TwoAttackParent)
        {
            TwoAttackParent thisTestSucksBro = (TwoAttackParent)attacker;

            if (thisTestSucksBro.SecondaryCardEffect == MinionParent.effect.heal)
            {
                isThisMinionATwoAttackHealer = true;
            }
        }

        // Don't heal player
        if (attacker.CardEffect == MinionParent.effect.heal)
        {
            Debug.Log("You cannot heal the other player.");
            ClearSelection();
            return;
        }

        if (attacker == null)
        {
            Debug.Log("Only minion cards can attack right now.");

            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.DesyncWarning("Only minion cards can attack right now for player two");
            }

            ClearSelection();
            return;
        }

        // twoattacks specific behavior
        if (attacker is TwoAttackParent)
        {
            TwoAttackParent twoAttackMinion = (TwoAttackParent)attacker;

            // avoid healing player again
            if (twoAttackMinion.SecondaryCardEffect == MinionParent.effect.heal && wasSecondAttack)
            {
                Debug.Log("You cannot heal the other player with your second attack.");
                ClearSelection();
                return;
            }

            /* TwoAttack AoE can be checked here if we want it to still hit cards when attacking player */

            // send this attack on player to peer
            if (!selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Networking.SendCardAttackPlayer(attacker, opposingPlayer, wasSecondAttack);
            }

            // handle specifically the first or second attack
            int damage = twoAttackMinion.FirstDamage;
            if (wasSecondAttack)
            {
                damage = twoAttackMinion.SecondDamage;
                twoAttackMinion.CardEffect = twoAttackMinion.SecondaryCardEffect;
            }
            if (twoAttackMinion.CanAttack)
            {
                opposingPlayer.TakeDamage(damage, twoAttackMinion);
                twoAttackMinion.CanAttack = false;
                twoAttackMinion.ForceActionSFX();
                twoAttackMinion.CardEffect = MinionParent.effect.twoAttacks;
                selectedCardObject.OwnerPlayer.RegisterAction();
            }
            else
            {
                ClearSelection();
                return;
            }
            RefreshCardVisual(selectedCardObject);
            // ClearSelection();
            // Debug.Log(attacker.CardName + " attacked other player. Target health: " + opposingPlayer.Health);
            /* AoE check can be done here if we want it to still hit cards when attacking player */

        }
        // regular minion
        else
        {
            if (attackingOwner != null && attacker.CanAttack)
            {
                // send this attack on player to peer
                if (!selectedCardObject.OwnerPlayer.IsPlayerTwo)
                {
                    Networking.SendCardAttackPlayer(attacker, opposingPlayer, wasSecondAttack);
                }

                int minionDamage = attacker.Damage;
                opposingPlayer.TakeDamage(minionDamage, attacker);
                attacker.CanAttack = false;
                attacker.ForceActionSFX();
                attackingOwner.RegisterAction();
            }
            else
            {
                ClearSelection();
                return;
            }
        }

        RefreshCardVisual(selectedCardObject);

        Debug.Log(attacker.CardName + " attacked other player. Target health: " + opposingPlayer.Health);

        ClearSelection();
    }

    private void RemoveSelectedCardFromHandUI(Player owner)
    {
        if (selectedCardObject == null || owner == null)
        {
            return;
        }

        if (owner == player1)
        {
            if (player1HandUI != null)
            {
                player1HandUI.RemoveCardFromHand(selectedCardObject.gameObject);
            }
        }
        else if (owner == player2)
        {
            if (player2HandUI != null)
            {
                player2HandUI.RemoveCardFromHand(selectedCardObject.gameObject);
            }
        }
    }

    private void RefreshCardVisual(CardClickHandler card)
    {
        if (card == null)
        {
            return;
        }

        CardUIManager uiManager = card.GetComponent<CardUIManager>();

        if (uiManager != null)
        {
            uiManager.RefreshCardUI();
        }
    }

    public void AttackOpposingTeamButton()
    {
        if (selectedCardObject == null || selectedCardObject.CardData == null)
        {
            Debug.Log("No card selected.");
            return;
        }

        Player attackerOwner = selectedCardObject.OwnerPlayer;

        if (attackerOwner == null)
        {
            Debug.Log("Selected card has no owner.");
            ClearSelection();
            return;
        }

        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Selected card must be in play before attacking.");
            ClearSelection();
            return;
        }

        Player targetPlayer = null;

        if (attackerOwner == player1)
        {
            targetPlayer = player2;
        }
        else if (attackerOwner == player2)
        {
            targetPlayer = player1;
        }

        if (targetPlayer == null)
        {
            Debug.Log("Target player is null.");
            ClearSelection();
            return;
        }

        TryAttackPlayer(false);
    }

    public void ClearSelection()
    {
        if (selectedCardObject != null)
        {
            selectedCardObject.SetSelectedVisual(false);
        }

        selectedCardObject = null;
    }
}