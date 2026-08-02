using UnityEngine;
using Network;

public class SetAICommander : MonoBehaviour
{
    public void SetCommander()
    {
        switch(PlayerPrefs.GetFloat("UnlockNum"))
        {
            case 0:
            case 1:
                {
                    Networking.P2CommanderName = "Major Munchkin";
                    break;
                }

            case 2:
            case 3:
                {
                    Networking.P2CommanderName = "Seargent Zoomie";
                    break;
                }

            case 4:
                {
                    Networking.P2CommanderName = "Hivemind";
                    break;
                }

            case 5:
                {
                    Networking.P2CommanderName = "Witchdoctor";
                    break;
                }
        }
    }
}
