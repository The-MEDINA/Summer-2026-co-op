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
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
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
}