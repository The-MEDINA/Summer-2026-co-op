using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void LoadLocalDemo()
    {
        DeckInstanceDeckbuilderScript.instance.TitleScreenButtonPressed = "1P Local Play";
        if (DeckInstanceDeckbuilderScript.instance != null)
        {
            if (DeckInstanceDeckbuilderScript.instance.Commander != "" && DeckInstanceDeckbuilderScript.instance.Deck.Count > 0)
            {
                SceneManager.LoadScene("AITestScene");
                return;
            }
            else
            {
                SceneManager.LoadScene("DeckbuilderScene");
                return;
            }
        }
        SceneManager.LoadScene("Demo_LocalTwoPlayer");
    }

    public void LoadLanDemo()
    {
        DeckInstanceDeckbuilderScript.instance.TitleScreenButtonPressed = "LAN Play";
        SceneManager.LoadScene("DirectConnectTest");
    }

    public void LoadDeckbuilder()
    {
        DeckInstanceDeckbuilderScript.instance.TitleScreenButtonPressed = "Deck Builder";
        SceneManager.LoadScene("DeckbuilderScene");
    }
    public void LoadTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}