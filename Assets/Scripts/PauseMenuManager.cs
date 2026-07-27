using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pausePanel;

    [Header("Other Menus")]
    [SerializeField] private GameObject winPanel;

    [Header("Scene Names")]
    [SerializeField] private string titleSceneName = "TitleScreen";

    private bool isPaused;

    private void Start()
    {
        // Make sure the game begins at normal speed.
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "PauseMenuManager does not have a PausePanel assigned."
            );
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        // Do not open the pause menu after the match has ended.
        if (winPanel != null && winPanel.activeSelf)
        {
            return;
        }

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pausePanel == null)
        {
            return;
        }

        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void RestartMatch()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ReturnToTitleScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }
}