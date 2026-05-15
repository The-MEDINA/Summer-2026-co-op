using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Network;
using TMPro;

public class button : MonoBehaviour
{
    public TMP_InputField inputfield;
    public TextMeshProUGUI modebutton;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Networking.SetLocalDetails();
        Networking.Details();
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

    public void changeMode()
    {
        if (Networking.CurrentMode == Network.mode.unset || Networking.CurrentMode == Network.mode.host)
        {
            Networking.CurrentMode = Network.mode.client;
            modebutton.text = "client";
        }
        else
        {
            Networking.CurrentMode = Network.mode.host;
            modebutton.text = "host";
        }
    }

    public void SetClientIP()
    {
        Networking.OtherIPv4Address = Networking.ResolveIP(inputfield.text);
    }

    public void startConnection()
    {
        if (Networking.CurrentMode == Network.mode.host)
        {
            Networking.StartHost();
        }
        if (Networking.CurrentMode == Network.mode.client)
        {
            Networking.StartClient();
        }
    }

    public void TEMPSendaPacket()
    {
        int i = 0;
        Networking.TEMPsendpacket();
    }
}
