using System;
using UnityEngine;

[System.Serializable]
public class CommanderCardScript : MonoBehaviour
{
    public enum ability
    {
        none,
        spawnTokens
    }

    private ability commanderAbility;
    [SerializeField] private GameObject tokenPrefab;
    private Battleground bg;
    private string name;

    public ability CommanderAbility { get { return commanderAbility; } }
    public string Name { get { return name; } set { name = value; } }

    public CommanderCardScript(ability commAbility, string name, Battleground batGro)
    {
        commanderAbility = commAbility;
        this.name = name;
        bg = batGro;
    }

    private void PerformAbility()
    {
        switch (CommanderAbility)
        {
            case ability.spawnTokens:
                {
                    SpawnTokenMinions();
                    break;
                }

            default:
            case ability.none:
                {
                    break;
                }
        }
    }

    private void SpawnTokenMinions()
    {
        bg.SpawnCardToInPlay(new MinionParent(0, 1, 1, "Kitten", NewVirtualCardParent.type.token, MinionParent.effect.none, NewVirtualCardParent.location.inPlay));
        bg.SpawnCardToInPlay(new MinionParent(0, 1, 1, "Kitten", NewVirtualCardParent.type.token, MinionParent.effect.none, NewVirtualCardParent.location.inPlay));
    }
}
