using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("Volume UI")]
    [SerializeField] private Slider musicSlider;

    private const string MusicVolumeKey = "MusicVolume";

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

        AudioListener.volume = savedVolume;

        if (musicSlider != null)
        {
            musicSlider.value = savedVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        AudioListener.volume = volume;

        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
    }
}