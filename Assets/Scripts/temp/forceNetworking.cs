using UnityEngine;
using TMPro;
using Network;

public class forceNetworking : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pauseText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Networking.CurrentState == state.disconnected)
        {
            pauseText.text = "not connected.";
        }
        else if (Networking.CurrentState == state.paused)
        {
            pauseText.text = "force unpause";
        }
        else if (Networking.CurrentState == state.connected)
        {
            pauseText.text = "force pause";
        }
    }

    public void ForcePauseUnpause()
    {
        if (Networking.CurrentState != state.disconnected)
        {
            if (Networking.CurrentState == state.connected)
            {
                Networking.CurrentState = state.paused;
                Networking.SendPauseUnpause(true);
                pauseText.text = "force unpause";
            }
            else
            {
                Networking.CurrentState = state.connected;
                Networking.SendPauseUnpause(false);
                pauseText.text = "force pause";
            }
        }
        else
        {
            pauseText.text = "not connected.";
        }
    }
}
