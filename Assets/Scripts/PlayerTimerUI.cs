using UnityEngine;
using TMPro;

public class PlayerTimerUI : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI energyTimerText;
    [SerializeField] private TextMeshProUGUI moveTimerText;

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

        if (moveTimerText != null)
        {
            if (player.CanMove)
            {
                moveTimerText.text = "Move: Ready";
            }
            else
            {
                moveTimerText.text = "Move: " + player.MoveCooldownRemaining.ToString("0.0") + "s";
            }
        }
    }
}