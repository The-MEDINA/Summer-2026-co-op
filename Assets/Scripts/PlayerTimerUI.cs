using UnityEngine;
using TMPro;

public class PlayerTimerUI : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI energyTimerText;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (energyText != null)
        {
            energyText.text = "Energy: " + player.Energy + " / " + player.MaxEnergy;
        }

        if (energyTimerText != null)
        {
            energyTimerText.text = "Next Energy: " + player.EnergyTimerRemaining.ToString("0.0") + "s";
        }
        if (healthText != null)
        {
            healthText.text = $"{player.Health}";
            if (player.Health <= 25)
            {
                healthText.color = Color.red;
            }
            else
            {
                healthText.color = Color.white;
            }
        }
    }
}