using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour
{
    [Header("Brightness UI")]
    [SerializeField] private Image brightnessOverlay;
    [SerializeField] private Slider brightnessSlider;

    private const string BrightnessKey = "Brightness";

    private void Start()
    {
        float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, 1f);

        SetBrightness(savedBrightness);

        if (brightnessSlider != null)
        {
            brightnessSlider.value = savedBrightness;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }
    }

    public void SetBrightness(float brightness)
    {
        brightness = Mathf.Clamp01(brightness);

        if (brightnessOverlay != null)
        {
            Color overlayColor = brightnessOverlay.color;
            overlayColor.a = 1f - brightness;
            brightnessOverlay.color = overlayColor;
        }

        PlayerPrefs.SetFloat(BrightnessKey, brightness);
        PlayerPrefs.Save();
    }
}