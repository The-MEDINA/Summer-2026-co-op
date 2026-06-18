using UnityEngine;
// This script is attached to a minion through the minion constructor
public class CoordinateAbilityScript
{
    private int numToHit; //when to award the coordinate bonus
    private bool awarded = false;
    public int NumToHit {  get { return numToHit; } set { numToHit = value; } }
    public bool Awarded { get { return awarded; } }

    //because the methods are public, the script doesn't need to be added to anything. In the future this should be changed to private (or protected?)
    //and attatched to the game object, but this works for now - Jake
    //the above comment may no longer be applicable - Future Jake

    /// <summary>
    /// assigns numToHit because there isn't a constructor
    /// </summary>
    /// <param name="name"></param>
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

    /// <summary>
    /// rewards the minion with its coordinate reward
    /// </summary>
    /// <param name="minion">the minion being awarded</param>
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
                    minion.Damage += 3;
                    break;
                }

            case "Cat Demolition Crew": 
                {
                    minion.UnityObject.GetComponent<CardClickHandler>().SetSpeed(CardClickHandler.speed.haste);
                    break;
                }
        }
        awarded = true;
    }
}
