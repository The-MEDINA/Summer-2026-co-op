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
    private float timer;
    [SerializeField] private int timeForEnergy = 5;

    public int Health {  get { return health; } set { health = value; } }
    public int Energy { get { return energy; } set { energy = value; } }

    void Update()
    {
        if (timer > timeForEnergy)
        {
            Energy++;
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
