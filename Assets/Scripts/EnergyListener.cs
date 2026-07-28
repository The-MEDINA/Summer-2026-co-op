using UnityEngine;

public class EnergyListener : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SpriteRenderer energySprite;
    private Sprite[] energySections;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        energySections = Resources.LoadAll<Sprite>($"spritesheet Energy");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < energySections.Length; i++)
        {
            if ((player.EnergyTimer / player.TimeForEnergy) >= ((float) i / energySections.Length))
            {
                energySprite.sprite = energySections[i];
            }
        }
    }
}
