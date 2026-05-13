using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Network;
using TMPro;

public class button : MonoBehaviour
{
    public TMP_InputField TMP_IF;  
    
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

    public void SetClientIP()
    {

    }
}
