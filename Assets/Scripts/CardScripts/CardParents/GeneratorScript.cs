using UnityEngine;

//attatched through the minion constructor
public class GeneratorScript : MonoBehaviour
{
    private float timer = 0f;
    private float timeToEnergy = 10f;
    private Player p;
    private bool generateIsOn = false;

    public Player P { get { return p; } set { p = value; } }
    public bool GenerateIsOn { get { return generateIsOn; } set { generateIsOn = value; }  }

    void Start()
    {
        GenerateIsOn = false;
    }

    public void Update()
    {
        if (GenerateIsOn)
        {
            if (timer >= timeToEnergy)
            {
                Debug.Log("Energy gained");
                P.Energy++;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }
}
