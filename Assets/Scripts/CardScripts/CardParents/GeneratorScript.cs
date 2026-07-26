using UnityEngine;

//attatched through the minion constructor
public class GeneratorScript : MonoBehaviour
{
    private float timer = 0f;
    private float timeToEnergy = 10f;
    private Player p;
    private bool generateIsOn = false;

    public bool GenerateIsOn { get { return generateIsOn; } set { generateIsOn = value; }  }

    void Start()
    {
        p = GetComponent<CardClickHandler>().OwnerPlayer;
        GenerateIsOn = false;
    }

    void Update()
    {
        if (GenerateIsOn)
        {
            if (timer >= timeToEnergy)
            {
                p.Energy++;
                timer = 0;
            }
            else
            {
                timer++;
            }
        }
    }
}
