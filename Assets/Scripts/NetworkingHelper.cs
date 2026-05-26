using UnityEngine;
using Network;
using UnityEngine.SceneManagement;

public class NetworkingHelper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Networking.RequestSceneChange != "")
        {
            SceneManager.LoadScene(Networking.RequestSceneChange);
            Networking.RequestSceneChange = "";
        }
    }
}
