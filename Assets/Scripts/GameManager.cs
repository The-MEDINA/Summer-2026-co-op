using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [Header("Win Screen UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text resultText;

    [Header("Scene Names")]
    [SerializeField] private string titleSceneName = "TitleScreen";

    [Header("Other Managers")]
    [SerializeField] private MusicPlayer BattleTheme;
    [SerializeField] private MusicPlayer MenuPiano;

    private bool gameEnded;

    private void Start()
    {
        //This is important in case the scene was loaded after a paused match.
        Time.timeScale = 1f;
        gameEnded = false;

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "GameManager does not have a WinPanel assigned."
            );
        }

        // listen for network changes when we're not in the 1 player scene
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Demo_LocalTwoPlayer")
        {
            Networking.stateChange += CheckNetworkChange;
        }
    }

    private void Update()
    {
        if (gameEnded)
        {
            return;
        }

        if (player1 == null || player2 == null)
        {
            return;
        }

        //Check both players before ending the game. This prevents the match from continuing after health reaches zero.
        if (player1.Health <= 0)
        {
            EndGame("PLAYER 2 WINS!");
        }
        else if (player2.Health <= 0)
        {
            EndGame("PLAYER 1 WINS!");
        }
    }

    private void EndGame(string message)
    {
        gameEnded = true;

        if (resultText != null)
        {
            resultText.text = message;
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        //Pauses gameplay while still allowing UI buttons to work.
        Time.timeScale = 0f;

        BattleTheme.StopMusic();
        MenuPiano.StartMusic();
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        if (Networking.CurrentState == state.connected)
        {
            Networking.SendSceneSwitch(currentScene.name);
        }
    }

    public void ReturnToTitleScreen()
    {
        if (Networking.CurrentState == state.connected)
        {
            Networking.SendSceneSwitch("Titlescreen");
            Networking.CloseConnection();
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }

    /// <summary>
    /// Change to the titlescreen if a disconnect was detected midgame.
    /// </summary>
    /// <param name="state">State of the network manager.</param>
    private void CheckNetworkChange(Network.state state)
    {
        if (state == state.disconnected)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(titleSceneName);
        }
    }
}