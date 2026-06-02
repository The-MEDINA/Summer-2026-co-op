using System;
using UnityEngine;

[System.Serializable]
public class CommanderCardScript : MonoBehaviour
{
    [SerializeField] protected Battleground bg;
    private string name;

    public string Name { get { return name; } set { name = value; } }

    public virtual void PerformAbility() { }
}
