using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    enum state
    {
        player1lead,
        transition1to2,
        player2lead,
        transition2to1,
        noLoop
    }
    [SerializeField] private bool persistBetweenScenes = false;
    [SerializeField] private string[] excludeScenes;
    [SerializeField] private string SourceScene = "TitleScreen";
    private string lastScene = "";
    /// <summary>
    /// Beginning is optional.
    /// </summary>
    [SerializeField] private AudioClip beginning;
    /// <summary>
    /// This should be the looping part of the music, repeated TWICE.
    /// This is so that the music player can fade it in and out at the middle, to prevent any gaps in the sound.
    /// </summary>
    [SerializeField] private AudioClip loopx2;

    [Header("These should be two separate audio sources.")] 
    [SerializeField] private AudioSource musicPlayer1;
    [SerializeField] private AudioSource musicPlayer2;
    
    [Header("")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float volume;

    /// <summary>
    /// Short transition time recommended. Long ones make the transition too obvious.
    /// </summary>
    [Tooltip("Short transition time recommended. Long ones make the transition too obvious.")]
    [SerializeField] private float transitionTime = 0.2f;

    [Tooltip("change this if there's a gap between the beginning and loop or if they clash at some point.")]
    [SerializeField] private float offset = -0.075f;

    private state current = state.player2lead;
    private float deltaVolume = 0;

    public bool PersistBetweenScenes { get { return persistBetweenScenes; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (persistBetweenScenes)
        {
            MusicPlayer other = FindAnyObjectByType<MusicPlayer>();
            if (other != null && other.persistBetweenScenes && other.gameObject != this.gameObject)
            {
                Destroy(this.gameObject);
                return;
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        StartMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (current != state.noLoop)
        {
            // start fading at the halfway point.
            if (musicPlayer2.time >= (loopx2.length / 2) && current == state.player2lead)
            {
                current = state.transition2to1;
                musicPlayer1.clip = loopx2;
                musicPlayer1.time = 0;
                musicPlayer1.volume = 0;
                musicPlayer1.Play();
            }
            // fade here
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
            // start fading at the halfway point
            else if (musicPlayer1.time >= (loopx2.length / 2) && current == state.player1lead)
            {
                current = state.transition1to2;
                musicPlayer2.clip = loopx2;
                musicPlayer2.time = 0;
                musicPlayer2.volume = 0;
                musicPlayer2.Play();
            }
            // more fading here
            else if (current == state.transition1to2)
            {
                musicPlayer2.volume += Time.deltaTime * deltaVolume;
                musicPlayer1.volume -= Time.deltaTime * deltaVolume;
                if (musicPlayer1.volume <= 0 || musicPlayer2.volume >= volume)
                {
                    current = state.player2lead;
                    musicPlayer2.volume = volume;
                    musicPlayer1.volume = 0;
                }
            }
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // setup
        string[] scenePath = scene.path.Split("/");

        for (int i = 0; i < excludeScenes.Length; i++)
        {
            string excludeScene = $"{excludeScenes[i]}.unity";

            // stop if the scene is in the exclude scenes list
            if (scenePath[scenePath.Length - 1] == excludeScene)
            {
                musicPlayer1.Stop();
                musicPlayer2.Stop();
                lastScene = scenePath[scenePath.Length - 1];
                return;
            }
        }
        // Restart music if the last scene was on the exclude list but the current one isn't
        for (int i = 0; i < excludeScenes.Length;i++)
        {
            if (lastScene == $"{excludeScenes[i]}.unity")
            {
                StartMusic();
                lastScene = scenePath[scenePath.Length - 1];
                return;
            }
        }
        lastScene = scenePath[scenePath.Length - 1];
    }

    private void StartMusic()
    {
        deltaVolume = volume / transitionTime;
        musicPlayer1.volume = volume;
        musicPlayer2.volume = volume;
        if (loopx2 != null) musicPlayer2.clip = loopx2;
        else current = state.noLoop;

        // play the beginning first if there is one.
        if (beginning != null)
        {
            musicPlayer1.clip = beginning;
            musicPlayer1.Play();
            musicPlayer2.PlayDelayed(beginning.length + offset);
        }
        else
        {
            musicPlayer1.clip = loopx2;
            musicPlayer2.Play();
        }
    }
}
