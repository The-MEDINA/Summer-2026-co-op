using UnityEngine;
using TMPro;

public class PlayerTimerUI : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI energyTimerText;

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

    }
}