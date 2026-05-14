
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
    // type of packet in the header.
    enum packetType
    {
        handshake
    }
    enum mode
    {
        host,
        client,
        unset
    }
    public static class Networking
    {
        private static int port = 6767;
        private static string handshakeContent = "BLITZBURN HANDSHAKE";

        private static mode currentMode = mode.client;
        private static string localHostName = Dns.GetHostName();
        private static List<IPAddress> IPv4AddressList = new List<IPAddress>();
        private static IPAddress otherIPv4Address = IPAddress.Parse("0.0.0.0");
        private static TcpListener server;
        private static TcpClient client;
        private static NetworkStream stream;

        public static string CurrentMode { 
            get 
            {  
                switch (currentMode)
                {
                    case (mode.host): return "host";
                    case (mode.client): return "client";
                    case (mode.unset):
                        {
#if DEBUG_MODE
                            Debug.Log("Returning unset state in Networking!");
#endif
                            return "unset";
                        }
                }
#if DEBUG_MODE
                Debug.Log("Returning unset state in Networking!");
#endif
                return "unset";
            } 
            set
            {
                switch (value)
                {
                    case ("host"): currentMode = mode.host; break;
                    case ("client"): currentMode = mode.client; break;
                    case ("unset"):
                        {
#if DEBUG_MODE
                            Debug.Log("Network manager disallows setting mode to unset. No changes made.");
#endif
                            break;
                        }
#if DEBUG_MODE
                    default:
                        {
                            Debug.Log($"Expected a mode, found ${value}. No changes made.");
                            break;
                        }
#endif
                }
#if DEBUG_MODE
                Debug.Log($"Network state: {CurrentMode}");
#endif
            }
        }
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
            // return the IPAddress if one was passed.
            if (IPAddress.TryParse(raw, out ip)) return ip;
            // determine the IPAddress from the hostname otherwise.
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

        /// <summary>
        /// Start up a connection as host, and verify the connection.
        /// </summary>
        public static async void StartHost()
        {
            server = new TcpListener(IPAddress.Any, port);
            client = new TcpClient();
            server.Start();
#if DEBUG_MODE
            Debug.Log("Server started, waiting to accept client.");
#endif
            client = await server.AcceptTcpClientAsync();
#if DEBUG_MODE
            Debug.Log("Client found. Verifying...");
#endif
            stream = client.GetStream();
            byte[] handshake = EncodePacket(packetType.handshake, true);
        }
        /*
         * Packets are 1024 byte long arrays that are split differently depending on their type.
         * the first byte of every packet will always contain its type. The type is determined by the packetType enum.
         * --- HANDSHAKE: ---
         * the second byte determines if it's a request or response.
         * currently bytes 3 - 39 store the handshakeContent variable.
         * bytes 40 - 1024 are empty so we can use it later to send more info.
         */

        private static byte[] EncodePacket(packetType type, bool isRequest)
        {
            byte[] packet = new byte[1024];
            switch (type)
            {
                case (packetType.handshake):
                    {
                        // the first byte of each packet will always be its type.
                        packet[0] = (byte) packetType.handshake;

                        // the second byte determines if it's a a request or response.
                        if (isRequest) packet[1] = 0;
                        else packet[1] = 1;

                        for (int i = 0; i < handshakeContent.Length; i++)
                        {
                            // prepare the char to encode into two bytes.
                            char charToEncode = handshakeContent[i];
                            byte highByte = 0;
                            byte lowByte = 0;

                            // mask out the top 8 bits.
                            lowByte = (byte)(charToEncode & 255);

                            // shift right 8 bits and then mask.
                            highByte = (byte)((charToEncode >> 8) & 255);

                            // High bytes are in even indexes, low bytes are in odd indexes.
                            packet[2 + (2 * i)] = highByte;
                            packet[3 + (2 * i)] = lowByte;

#if DEBUG_MODE
                            char reconstructed;
                            short raw = highByte;
                            raw <<= 8;
                            raw += lowByte;
                            reconstructed = (char) raw;
                            Debug.Log($"Reconstructed char: {reconstructed}");
                            Debug.Log($"{3 + (2 * i)}");
#endif
                        }

                        break;
                    }
            }
            return packet;
        }
    }
}