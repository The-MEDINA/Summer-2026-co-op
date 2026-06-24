using UnityEngine;
using TMPro;

public class DamagePUTextScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    private float timer = 0.8f;

    void Update()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f * Time.deltaTime);
        timer -= Time.deltaTime;

        if (timer <=0) { Destroy(gameObject); }
        else if (timer < 0.4f)
        {
            text.alpha -= 2.5f * Time.deltaTime;
            text.color = Color.black;
        }
    }

    public void SetNumber(int damage) { text.text = damage.ToString(); }
}
