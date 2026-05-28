using System.Collections.Generic;
using UnityEngine;

public class CoordinateAbilityScript
{
    private int numToHit;
    private bool awarded = false;
    public int NumToHit {  get { return numToHit; } }
    public bool Awarded { get { return awarded; } }

    public CoordinateAbilityScript(string name)
    {
        switch (name)
        {
            case "Night Vision Cat":
                {
                    numToHit = 3;
                    break;
                }

            case "Ninja Cat":
                {
                    numToHit = 3;
                    break;
                }

            case "Beeg Cat":
                {
                    numToHit = 5;
                    break;
                }

            case "Cat Demolition Crew":
                {
                    numToHit = 5;
                    break;
                }
        }
    }

    public void RewardAbility(MinionParent minion)
    {
        switch (minion.CardName)
        {
            case "Night Vision Cat":
                {
                    minion.Damage++;
                    minion.Health++;
                    Debug.Log($"{minion.CardName}, {minion.Health}, {minion.Damage}");
                    break;
                }

            case "Ninja Cat":
                {
                    minion.Damage += 2;
                    minion.Health += 2;
                    Debug.Log($"{minion.CardName}, {minion.Health}, {minion.Damage}");
                    break;
                }

            case "Beeg Cat":
                {
                    minion.Health += 3;
                    break;
                }

            case "Cat Demolition Crew": //not implemented YET!!!
                {
                  //  minion.
                    break;
                }
        }
        awarded = true;
    }
}
