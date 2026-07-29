using System;
using UnityEngine;

[System.Serializable]
public class CommanderCardScript : MonoBehaviour
{
    [SerializeField] protected Battleground bg; //ALL COMMANDERS NEED THIS NO MATTER WHAT
    private string name = "";
    private bool deckbuilderOverride = false;
    private float frozenTimeDelay = 0;
    private DeckCardClickHandler clickHandler = null;
    private bool searchedForClickHandler = false;

    public bool DeckbuilderOverride { get { return deckbuilderOverride; } set { deckbuilderOverride = value; } }
    public Battleground BG { get { return bg; } set { bg = value; } }
    public string Name { get { return name; } set { name = value; } }
    public float FrozenTimeDelay { get { return frozenTimeDelay; } set { frozenTimeDelay = value; } }
    public DeckCardClickHandler ClickHandler { get { return clickHandler; } set { clickHandler = value; } }
    public bool SearchedForClickHandler { get { return searchedForClickHandler; } set { searchedForClickHandler = value; } }

    /// <summary>
    /// does whatever the Commander does
    /// </summary>
    public virtual void PerformAbility() { }

    /// <summary>
    /// Updates the attack timer
    /// </summary>
    public virtual void Progressbar() { }
}
