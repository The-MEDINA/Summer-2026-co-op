using UnityEngine;
using UnityEngine.SceneManagement;
using Network;

public class button : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Networking.SetLocalDetails();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void switchScene()
    {
        Networking.Details();
        Networking.Port = 6767;
        SceneManager.LoadScene("SampleScene");
    }
}
