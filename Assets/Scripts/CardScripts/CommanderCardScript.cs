using UnityEngine;

public class CommanderCardScript : NewVirtualCardParent
{
    public enum ability
    {
        none,
        spawnTokens
    }

    private ability commanderAbility;

    public ability CommanderAbility { get { return commanderAbility; } }

    public CommanderCardScript(ability commAbility, int cost, string name, NewVirtualCardParent.type cardType, NewVirtualCardParent.location cardLocation)
         : base(cost, name, cardType, cardLocation)
    {
        commanderAbility = commAbility;
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

    }
}
