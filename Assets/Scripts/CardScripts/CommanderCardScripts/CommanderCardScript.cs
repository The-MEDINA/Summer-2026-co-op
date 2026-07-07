using System;
using UnityEngine;

[System.Serializable]
public class CommanderCardScript : MonoBehaviour
{
    [SerializeField] protected Battleground bg; //ALL COMMANDERS NEED THIS NO MATTER WHAT
    private string name = "";
    private bool deckbuilderOverride = false;

    public bool DeckbuilderOverride { get { return deckbuilderOverride; } set { deckbuilderOverride = value; } }
    public Battleground BG { get { return bg; } set { bg = value; } }
    public string Name { get { return name; } set { name = value; } }

    /// <summary>
    /// does whatever the Commander does
    /// </summary>
    public virtual void PerformAbility() { }
}
