using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Network;
using TMPro;
using System.Net;
using System.Net.Sockets;

public class button : MonoBehaviour
{
    public TMP_InputField inputfield;
    public TextMeshProUGUI modebutton;
    public TextMeshProUGUI startbutton;
    public TextMeshProUGUI IPbutton;
    public TextMeshProUGUI hostname;
    public TextMeshProUGUI IPtext;
    public TextMeshProUGUI statusText;
    private bool showIP = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Networking.SetLocalDetails();
        Networking.Details();
        if (hostname != null) hostname.text = $"Hostname: {Dns.GetHostName()}";
        Network.Networking.stateChange += status;
        Network.Networking.networkError += error;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void switchScene()
    {
        Networking.SendSceneSwitch("Demo_LocalTwoPlayer");
        SceneManager.LoadScene("Demo_LocalTwoPlayer");
    }

    public void showIPs()
    {
        if (showIP) showIP = false;
        else showIP = true;

        if (showIP)
        {
            IPbutton.text = "Hide IPs";
            IPtext.text = "IP Address(es): \n";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPtext.text += $"{ip.ToString()}\n";
                }
            }
        }
        else
        {
            IPbutton.text = "Show IPs";
            IPtext.text = "";
        }
    }

    public void changeMode()
    {
        if (Networking.CurrentMode == Network.mode.unset || Networking.CurrentMode == Network.mode.host)
        {
            Networking.CurrentMode = Network.mode.client;
            modebutton.text = "client"; 
            startbutton.text = "Find game";
        }
        else
        {
            Networking.CurrentMode = Network.mode.host;
            modebutton.text = "host";
            startbutton.text = "Start hosting";
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

    private void status(Network.state currentState)
    {
        if (statusText != null)
        {
            if (currentState == state.disconnected)
            {
                statusText.text += "Disconnected.\n";
            }
            else if (currentState == state.searching)
            {
                statusText.text = "Searching...\n";
            }
            else if (currentState == state.connected)
            {
                statusText.text += "Connected!\n";
            }
        }
    }

    private void error(string error)
    {
        if (statusText != null)
        {
            statusText.text += $"{error}\n";
        }
    }

    public void TitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
