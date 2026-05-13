// This #define enables console output, among other helpful debug code.
// Comment it out to remove any debugging code.
#define DEBUG_MODE

using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] [Tooltip("Should be a value over 1024.")] private int port = 6000;
    private string localHostName;
    private List<IPAddress> IPv4AddressList;

    // Unity calls Awake when loading an instance of a script component.
    void Awake()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize.
        IPv4AddressList = new List<IPAddress>();
        // get and set the host name and any IPv4 addresses associated with this device.
        // host name.
        localHostName = Dns.GetHostName();
        // IPv4 Addresses.
        IPHostEntry host = Dns.GetHostEntry(localHostName);
        // filter out IPv4 and IPv6 addresses.
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                IPv4AddressList.Add(ip);
            }
        }

#if DEBUG_MODE
        // print out the details in debug mode only.
        Debug.Log($"port: {port}");
        Debug.Log($"hostname: {localHostName}");
        Debug.Log($"IPv4 Address(es):");
        // maybe foreach loops aren't so bad after all.
        // I used to hate these things.
        foreach (IPAddress ip in IPv4AddressList)
        {
            Debug.Log(ip.ToString());
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}