using Network;
using UnityEngine;
using UnityEngine.EventSystems;

public class MajorMunchkinScript : CommanderCardScript, IPointerClickHandler
{
    [SerializeField] private GameObject tokenPrefab;
    private float timer = 0f;
    [SerializeField] private float timeToEffect = 3f;
    private bool canAttack = false;

    private void Start()
    {
        Name = "Major Munchkin";//assigns name
    }

    private void Update()
    {
        //controls effect timer
        if (timer > timeToEffect)
        {
            canAttack = true;
            timer = 0f;
        }
        else if (timer < timeToEffect && !canAttack)
        {
            timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// calls PerformAbility() on mouse click
    /// </summary>
    /// <param name="eventData">data for mouse pointer click</param>
    public void OnPointerClick(PointerEventData eventData) { if (canAttack) { PerformAbility(); } }

    /// <summary>
    /// spawns two token creatures
    /// </summary>
    public override void PerformAbility()
    {
        bg.SpawnCardToInPlay(new MinionParent(0, 1, 1, "Kitten", NewVirtualCardParent.type.token, 
            MinionParent.effect.none, NewVirtualCardParent.location.inPlay));
        bg.SpawnCardToInPlay(new MinionParent(0, 1, 1, "Kitten", NewVirtualCardParent.type.token, 
            MinionParent.effect.none, NewVirtualCardParent.location.inPlay));
        if (!bg.P.IsPlayerTwo)
        {
            Networking.SendCardAdd(cardIndex.Index.CreateCard("Kitten", NewVirtualCardParent.location.inPlay), NewVirtualCardParent.location.inPlay);
        }
        canAttack = false;
    }
}
