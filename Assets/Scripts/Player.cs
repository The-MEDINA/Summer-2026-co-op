using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int health = 50;
    private int energy;
    private List<CardParent> deck = new List<CardParent>();
    private List<CardParent> hand = new List<CardParent>();
    private List<CardParent> inPlay = new List<CardParent>();
    private List<CardParent> discard = new List<CardParent>();
    private CardParent commander; //probably should be its own parent file, not CardParent
    // I think commanders *can* be subclasses of card parent though it also does make sense for it to be a completely separate entity imo
    // If they're not necessarily being played like cards then they should be separate, else they should be subclasses - Dave
    private float timer;
    [SerializeField] private int timeForEnergy = 5;

    public int Health {  get { return health; } set { health = value; } }
    public int Energy { get { return energy; } set { energy = value; } }
    public List<CardParent> Deck { get { return deck; } set { deck = value; } }

    void Update()
    {
        if (timer > timeForEnergy)
        {
            Energy++;
            // this line should be timer -= timeForEnergy imo - Dave
            timer = 0;
            //possible int to track energy gain to draw cards
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    //BeginGame()

    //PlayCard()?

    //DrawCard()

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if(health < 0) { Death(); }
    }

    public void Death()
    {
        //might be worth a GameManager method for win/loss
    }
}
