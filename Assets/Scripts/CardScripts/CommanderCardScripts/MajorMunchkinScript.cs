using Network;
using Unity.VisualScripting;
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
        Name = "Major Munchkin";
    }

    private void Update()
    {
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

    public void OnPointerClick(PointerEventData eventData) { if (canAttack) { PerformAbility(); } }

    public override void PerformAbility()
    {
        bg.SpawnCardToInPlay(new MinionParent(0, 1, 1, "Kitten", NewVirtualCardParent.type.token, 
            MinionParent.effect.none, NewVirtualCardParent.location.inPlay));
        bg.SpawnCardToInPlay(new MinionParent(0, 1, 1, "Kitten", NewVirtualCardParent.type.token, 
            MinionParent.effect.none, NewVirtualCardParent.location.inPlay));
        canAttack = false;
    }
}
