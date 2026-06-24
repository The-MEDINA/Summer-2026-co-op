using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    enum state
    {
        player1lead,
        transition1to2,
        player2lead,
        transition2to1
    }

    [SerializeField] private AudioClip beginning;
    [SerializeField] private AudioClip loopx2;
    [Header("These should be two separate audio sources.")] 
    [SerializeField] private AudioSource musicPlayer1;
    [SerializeField] private AudioSource musicPlayer2;
    [Header("")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float volume;
    [SerializeField] private float transitionTime = 5;
    [SerializeField] private float offset = -0.075f;

    private state current = state.player2lead;
    private float deltaVolume = 0;
    private bool beginningPlayed = false;
    private double time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deltaVolume = volume / transitionTime;
        musicPlayer1.volume = volume;
        musicPlayer2.volume = volume;

        if (beginning != null)
        {
            musicPlayer1.clip = beginning;
        }
        else
        {
            musicPlayer1.clip = loopx2;
            beginningPlayed = true;
        } 
        musicPlayer2.clip = loopx2;
        musicPlayer1.Play();
        musicPlayer2.PlayDelayed(beginning.length + offset);
    }

    // Update is called once per frame
    void Update()
    {
        if (musicPlayer2.time >= (loopx2.length / 2) && current == state.player2lead)
        {
            current = state.transition2to1;
            musicPlayer1.clip = loopx2;
            musicPlayer1.time = 0;
            musicPlayer1.volume = 0;
            musicPlayer1.Play();
        }
        else if (current == state.transition2to1)
        {
            musicPlayer1.volume += Time.deltaTime * deltaVolume;
            musicPlayer2.volume -= Time.deltaTime * deltaVolume;
            if (musicPlayer2.volume <= 0 || musicPlayer1.volume >= volume)
            {
                current = state.player1lead;
                musicPlayer1.volume = volume;
                musicPlayer2.volume = 0;
            }
        }
    }
}
