using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void LoadLocalDemo()
    {
        SceneManager.LoadScene("Demo_LocalTwoPlayer");
    }

    public void LoadLanDemo()
    {
        SceneManager.LoadScene("DirectConnectTest");
    }

    public void LoadDeckbuilder()
    {
        SceneManager.LoadScene("DeckbuilderScene");
    }
}