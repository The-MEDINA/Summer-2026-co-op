
/*
 * Networking.cs is where all of the networking is done.
 * It's a static class, so it's not tied to any object. It's essentially a singleton.
 *
 * In order to access it from other scripts, you'll need to include the namespace.
 * using Network; <--- Like this
 *
 * Then to access something from it, you'll need to use the keyword 'Networking'.
 * Networking.something <--- Methods, properties, etc
 *
 * To access any enums, you'll need to access Network, NOT Networking.
 * Network.enum.something <--- Like this
 *
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
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    // type of packet in the header.
    enum packetType
    {
        handshake,
        keepAlive
    }
    // the mode the machine's set to for networking.
    public enum mode
    {
        host,
        client,
        unset
    }
    // the state of the network manager.
    public enum state
    {
        disconnected,
        connected
    }
    public static class Networking
    {
        /// <summary>
        /// All these variables are for the network manager to actually do the networking.
        /// </summary>
        private static int port = 6767;
        private static readonly string handshakeContent = "BLITZBURN HANDSHAKE";

        private static state currentState = state.disconnected;
        private static mode currentMode = mode.client;
        private static string localHostName = Dns.GetHostName();
        private static List<IPAddress> IPv4AddressList = new List<IPAddress>();
        private static IPAddress otherIPv4Address = IPAddress.Parse("0.0.0.0");
        private static TcpListener server;
        private static TcpClient client;
        // private static IPEndPoint endpoint;
        private static NetworkStream stream;
        private static CancellationTokenSource lostConnection = new CancellationTokenSource(TimeSpan.FromSeconds(10));


        public static state CurrentState { 
            get
            {
#if DEBUG_MODE
                Debug.Log($"Current state: {currentState}");          
#endif
                return currentState;
            } 
            set
            {
#if DEBUG_MODE
                Debug.Log($"changing state to: {value}");          
#endif
                currentState = value;
            } 
        }

        /// <summary>
        /// Get/set the current mode of the network manager.
        /// </summary>
        public static mode CurrentMode { 
            get 
            {  
                switch (currentMode)
                {
                    case (mode.host): return mode.host;
                    case (mode.client): return mode.client;
                    case (mode.unset):
                        {
#if DEBUG_MODE
                            Debug.Log("Returning unset state in Networking!");
#endif
                            return mode.unset;
                        }
                }
#if DEBUG_MODE
                Debug.Log("Returning unset state in Networking!");
#endif
                return mode.unset;
            } 
            set
            {
                switch (value)
                {
                    case (mode.host): currentMode = mode.host; break;
                    case (mode.client): currentMode = mode.client; break;
                    case (mode.unset):
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
        /// <summary>
        /// get/set the current port.
        /// </summary>
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
        /// CAN AND WILL THROW EXCEPTIONS if a connection cannot be verified.
        /// </summary>
        public static async void StartHost()
        {
            // should probably wrap a lot of this into a do/while loop to retry connections.
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
            // setup the handshake process.
            stream = client.GetStream();
            byte[] handshake = EncodePacket(packetType.handshake, true);
            byte[] response = new byte[1024];
            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            // send a handshake packet to the client and wait for a response.
            await stream.WriteAsync(handshake, 0, handshake.Length);

#if DEBUG_MODE
            Debug.Log("Post write Async");
#endif
            // start waiting for a response.
            // Will currently close the socket to trigger a response upon timeout.
            // This will cause a SocketException to exit the function early.
            // can probably change this from stream.close to something else.
            using(cts.Token.Register(() => {stream.Close(); server.Stop(); }))
            {
                int receivedCount;
                try
                {
                    receivedCount = await stream.ReadAsync(response, 0, response.Length, cts.Token).ConfigureAwait(false);
                }
                catch (TimeoutException)
                {
                    receivedCount = -1;
                }
            }
#if DEBUG_MODE
            Debug.Log("Post read Async. Verifying response...");
#endif
            // double check the received packet is a handshake response.
            try
            {
                DecodePacket(response);
                CurrentState = state.connected;
#if DEBUG_MODE
            Debug.Log("Success! This is a valid client.");
#endif
                Connection();
            }
            catch
            {
#if DEBUG_MODE
                Debug.Log("Broken, unknown or otherwise invalid packet received. aborting.");
#endif  
                // proper code for terminating a connection here
                server.Stop();
                stream.Close();
            }
        }
        /*
         * Packets are 1024 byte long arrays that are split differently depending on their type.
         * the first byte of every packet will always contain its type. The type is determined by the packetType enum.
         * --- HANDSHAKE: ---
         * the second byte determines if it's a request or response.
         * currently bytes 3 - 39 store the handshakeContent variable.
         * bytes 40 - 43 hold the host system's IP Address.
         * bytes 44 - 1024 are empty so we can use it later to send more info.
         */

        /// <summary>
        /// Start a connection as client, and verify the connection.
        /// </summary>
        public static async void StartClient()
        {
#if DEBUG_MODE
            Debug.Log("starting client.");
#endif
            client = new();
            // both of these lines of code can and will throw exceptions if the ipaddress or endpoint are invalid.
            // endpoint = new IPEndPoint(OtherIPv4Address, port);
            await client.ConnectAsync(OtherIPv4Address, port);

            // start the handshake process here.
            stream = client.GetStream();
            byte[] handshake = EncodePacket(packetType.handshake, false);
            byte[] response = new byte[1024];

#if DEBUG_MODE
            Debug.Log("Connection made, waiting for packet");
#endif

            await stream.ReadAsync(response, 0, response.Length);

#if DEBUG_MODE
            Debug.Log("Packet received. Verifying...");
#endif
            try
            {
                DecodePacket(response);
#if DEBUG_MODE
                Debug.Log("Success! Valid handshake packet. Sending response.");
#endif
                await stream.WriteAsync(handshake, 0, handshake.Length);
                CurrentState = state.connected;
                Connection();
            }
            catch
            {
#if DEBUG_MODE
                Debug.Log("Broken, unknown or otherwise invalid packet received. aborting.");
#endif               
                // proper code for terminating a connection here
                stream.Close();
            }
        }

        /// <summary>
        /// Encode a packet to send to someone else.
        /// </summary>
        /// <param name="type">packet type.</param>
        /// <param name="isRequest">whether it's a request or a response. This is mainly for the handshake.</param>
        /// <returns>a byte[1024] array that is formatted to a certain type of packet, as specified by the type parameter.</returns>
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

                        // encode handshakeContent here.
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
                        }
                        // encode the ip address of this system.
                        byte[] splitIPAddress = IPv4AddressList[0].GetAddressBytes();
                        for (int i = 0; i < 4; i++)
                        {
                            packet[40 + i] = splitIPAddress[i];
                        }
                        break;
                    }
            }
            return packet;
        }

/// <summary>
/// Decode a packet and manipulate data accordingly. CAN AND WILL throw exceptions if it receives a broken or unidentified packet.
/// </summary>
/// <param name="packet">raw packet to manipulate, should be a byte[1024].</param>
        private static void DecodePacket(byte[] packet)
        {
            bool brokenPacket = false;
            Exception e = new Exception("Unidentified, corrupted or otherwise invalid packet received.");
            switch(packet[0])
            {
                case((byte) packetType.handshake):
                {
                    string handshakeContentFromPacket = "";

                    // check the request / response byte.
                    if (packet[1] > 1) 
                    {
                        brokenPacket = true;
                        break;
                    }

                    // check the handshake text.
                    for (int i = 0; i < handshakeContent.Length; i++)
                    {
                        short rawChar = packet[2 + (2 * i)];
                        rawChar <<= 8;
                        rawChar += packet[3 + (2 * i)];
                        handshakeContentFromPacket += (char) rawChar;
                    }
                    if (handshakeContentFromPacket != handshakeContent) brokenPacket = true;
                    break;
                }
                default:
                {
                    throw e;
                }
            }
            if (brokenPacket) throw e;
        }
        private static async void Connection()
        {
#if DEBUG_MODE
            Debug.Log($"entering Connection()");
#endif
            byte[] packet = new byte[1024];
            Task<int> result = stream.ReadAsync(packet, 0, packet.Length);

            await Task.WhenAny(result, Task.Delay(TimeSpan.FromSeconds(10)));

            if (!result.IsCompleted)
            {
                server.Stop();
                client.Close();
#if DEBUG_MODE
            Debug.Log($"timeout, closing connection.");
#endif
            }
#if DEBUG_MODE
            Debug.Log($"post read");
#endif
        }

        public static void TEMPsendpackettoself()
        {
            byte[] packet = new byte[1024];
            packet = EncodePacket(packetType.handshake, false);
            stream.Write(packet, 0, packet.Length);
        }
    }
}