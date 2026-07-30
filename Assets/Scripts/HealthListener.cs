using UnityEngine;
using UnityEngine.UI;

public class HealthListener : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image healthBackground;
    [SerializeField] private Image healthBar;
    [Header("Colors")]
    [SerializeField] private Color healthColor;
    [SerializeField] private Color overhealthColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.healthChange += Healthbar;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Healthbar(int health)
    {
        if (health > 100)
        {
            healthBackground.color = healthColor;
            healthBar.color = overhealthColor;
            healthBar.fillAmount = ((health - 100) / 100f);
        }
        else
        {
            healthBackground.color = Color.black;
            healthBar.color = healthColor;
            healthBar.fillAmount = (health / 100f);
        }
    }
}
