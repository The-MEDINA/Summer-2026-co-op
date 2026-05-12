using UnityEngine;

public abstract class Card : ScriptableObject
{
    public string cardName;
    public string description;

    public int cost;
    public int damage;
    public int health;

    public Sprite cardArt;

    public abstract void OnPlay(Player player, BattlegroundManager battleground);
}