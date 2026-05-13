
/*
 * Networking.cs is where all of the networking is done.
 * It's a static class, so it's not tied to any object. It's essentially a singleton.
 * In order to access it from other scripts, you'll need to include the namespace.
 * using Network; <--- Like this
 * Then to access something from it, you'll need to use the keyword 'Networking'.
 * Networking.something <--- Methods, properties, etc
 * Y'all will likely need to pass stuff like players and cards to Networking.
 * It's gonna need to directly modify a LOT of stuff.
 * Anyways if yall have a question about this ASK ME PLEASE!!!
 *  - Dave :>
 */

// This #define enables console output, among other helpful debug code.
// Comment it out to remove any debugging code.
#define DEBUG_MODE

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Network
{
    enum mode
    {
        host,
        client,
        unset
    }
    public static class Networking
    {
        private static mode currentMode = mode.unset;
        private static int port = 6000;
        private static string localHostName = Dns.GetHostName();
        private static List<IPAddress> IPv4AddressList = new List<IPAddress>();

        private static IPAddress otherIPv4Address = IPAddress.Parse("0.0.0.0");

        public static int Port { get { return port; } 
            set 
            {
#if DEBUG_MODE
                if (value < 1024)
                {
                    Debug.Log($"Please use a port larger than 1024. Sticking with a port value of {port}");
                }
#endif
                if (value > 1024) port = value;
            } 
        }

        public static IPAddress OtherIPv4Address { get { return otherIPv4Address; } set { otherIPv4Address = value; }
        }
        /// <summary>
        /// Take a string containing EITHER an ip address or hostname and convert it to an IPAddress object.
        /// </summary>
        /// <param name="raw">raw string to parse.</param>
        /// <returns>IPAddress object interpretation of the raw string.</returns>
        /// <exception cref="Exception">Exception raised if no IPv4 address is found at the hostname or ip.</exception>
        public static IPAddress ResolveIP(string raw)
        {
            IPAddress ip = null;
            if (IPAddress.TryParse(raw, out ip)) return ip;
            IPHostEntry hostEntry = Dns.GetHostEntry(raw);
            foreach (var ipEntry in hostEntry.AddressList)
            {
                if (ipEntry.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = ipEntry;
                }
            }
            if (ip == null) throw new Exception("No ipv4 address found.");
            return ip;
        }
        /// <summary>
        /// Sets the local details using the hostname and IPv4 addresses from this PC.
        /// These are used to connect to other machines.
        /// </summary>
        public static void SetLocalDetails()
        {
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
        }
        /// <summary>
        /// Prints out the hostname, port and IPv4 addresses to the Unity console.
        /// </summary>
        public static void Details()
        {
            Debug.Log($"port: {port}");
            Debug.Log($"hostname: {localHostName}");
            Debug.Log($"IPv4 Address(es):");
            // maybe foreach loops aren't so bad after all.
            // I used to hate these things.
            foreach (IPAddress ip in IPv4AddressList)
            {
                Debug.Log(ip.ToString());
            }
        }
    }
}
