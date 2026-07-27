using TMPro;
using UnityEngine;

public class PlayerHealthDisplay : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [Header("Health Text")]
    [SerializeField] private TMP_Text player1HealthText;
    [SerializeField] private TMP_Text player2HealthText;

    private void Start()
    {
        UpdateHealthText();
    }

    private void Update()
    {
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (player1 != null && player1HealthText != null)
        {
            int displayedHealth = Mathf.Max(0, player1.Health);

            player1HealthText.text =
                $"Player 1 Health: {displayedHealth}";
        }

        if (player2 != null && player2HealthText != null)
        {
            int displayedHealth = Mathf.Max(0, player2.Health);

            player2HealthText.text =
                $"Player 2 Health: {displayedHealth}";
        }
    }
}