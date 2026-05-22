
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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using cardIndex;

namespace Network
{
    /*
     * Packets are 1024 byte long arrays that are split differently depending on their type.
     * the first byte of every packet will always contain its type. The type is determined by the packetType enum.
     * 
     * --- HANDSHAKE: ---
     * the second byte determines if it's a request or response.
     * currently bytes 3 - 39 store the handshakeContent variable.
     * bytes 40 - 43 hold the host system's IP Address.
     * bytes 44 - 1023 are empty so we can use it later to send more info.
     * 
     * --- KEEPALIVE: ---
     * bytes 1 - 1023 are empty so we can use it later to send more info.
     * 
     * --- SCENESWITCH: ---
     * bytes 1-2 hold the length of the scene name.
     * The rest of the bytes hold the scene name.
     * (I'm encoding this in UTF-8 and I don't know how long that is.)
     * (This packet should be more than big enough anyways.)
     * 
     * --- CARDARRAY: ---
     * byte 1 holds the location of the cards.
     * byte 2 holds the length of the array of cards.
     * bytes 3 - 513 holds the cards. It's currently limited to 255 because the length is 1 byte.
     * bytes 514 - 1023 are empty so we can use it later to send more info.
     * 
     * --- CARDMOVE: ---
     * byte 1 holds the old location of the card.
     * byte 2 holds the new location of the card.
     * bytes 3 and 4 hold the card index.
     * bytes 5 - 1024 are empty so we can use it later to send more info.
     * 
     * --- CARDATTACK: ---
     * byte 1 holds the position of the card in the attacker's inplay array.
     * byte 2 holds the position of the card in the target's inplay array.
     * bytes 3 - 1023 are empty so we can use it later to send more info.
     */
    enum packetType
    {
        handshake,
        keepAlive,
        sceneSwitch,
        cardArray,
        cardMove,
        cardAttack
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
        /// These variables are for the network manager to manipulate the game.
        /// </summary>
        private static Player playerOne;
        private static Player playerTwo;

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
        private static NetworkStream stream;
        private static string requestSceneChange = "";

        public static Player PlayerOne { get { return playerOne; } set { playerOne = value; } }
        public static Player PlayerTwo { get { return playerTwo; } set { playerTwo = value; } }
        public static string RequestSceneChange { get { return requestSceneChange; } set { requestSceneChange = value; } }

        /// <summary>
        /// get/set the current state of the network manager.
        /// </summary>
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

        public static IPAddress OtherIPv4Address { get { return otherIPv4Address; } set { otherIPv4Address = value; } }

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
                // terminate the connection
                CloseConnection();
            }
        }

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
                // terminate connection
                CloseConnection();
            }
        }

        private static byte[] EncodePacket(NewVirtualCardParent card, NewVirtualCardParent.location oldLocation, NewVirtualCardParent.location newLocation)
        {
            byte[] packet = new byte[1024];

            // type and location info.
            packet[0] = (byte) packetType.cardMove;
            packet[1] = (byte) oldLocation;
            packet[2] = (byte) newLocation;

            // prepare the card's index to encode into two bytes.
            cardIndex.Details cardDetails = cardIndex.Index.GetDetails(card.CardName);
            short cardToEncode = (short)cardDetails.nameIndexPosition;
            byte highByte = 0;
            byte lowByte = 0;

            // mask out the top 8 bits.
            lowByte = (byte)(cardToEncode & 255);

            // shift right 8 bits and then mask.
            highByte = (byte)((cardToEncode >> 8) & 255);

            // High bytes are in odd indexes, low bytes are in even indexes.
            packet[3] = highByte;
            packet[4] = lowByte;

            return packet;
        }

        private static byte[] EncodePacket(NewVirtualCardParent attacker, NewVirtualCardParent target)
        {
            byte[] packet = new byte[1024];
            packet[0] = (byte) packetType.cardAttack;
            packet[1] = (byte) playerOne.InPlay.IndexOf(attacker);
            packet[2] = (byte) playerTwo.InPlay.IndexOf(target);
            return packet;
        }

        private static byte[] EncodePacket(string sceneName)
        {
            byte[] packet = new byte[1024];
            // I forget sometimes I'm using C# that has these helper functions built in.
            // Spending so much time in C for my MOPS class really made me build every little thing myself.
            byte[] nameAsBytes = Encoding.UTF8.GetBytes(sceneName);
            short length = (short) nameAsBytes.Length;

            packet[0] = (byte) packetType.sceneSwitch;

            byte highByte = 0;
            byte lowByte = 0;

            // mask out the top 8 bits.
            lowByte = (byte)(length & 255);

            // shift right 8 bits and then mask.
            highByte = (byte)((length >> 8) & 255);

            // High byte in index 1, low byte in index 2.
            packet[1] = highByte;
            packet[2] = lowByte;

            // encode the scene name.
            for (int i = 0; i < length; i++)
            {
                packet[3 + i] = nameAsBytes[i];
            }

            return packet;
        }

        private static byte[] EncodePacket(List<NewVirtualCardParent> cards, NewVirtualCardParent.location location)
        {
            byte[] packet = new byte[1024];

            // type and extra info about the array
            packet[0] = (byte) packetType.cardArray;
            packet[1] = (byte) location;
            packet[2] = (byte) cards.Count;

            // the array itself.
            for (int i = 0; i < cards.Count; i++)
            {
                // prepare the card's index to encode into two bytes.
                cardIndex.Details cardDetails = cardIndex.Index.GetDetails(cards[i].CardName);
                short cardToEncode = (short) cardDetails.nameIndexPosition;
                byte highByte = 0;
                byte lowByte = 0;

                // mask out the top 8 bits.
                lowByte = (byte)(cardToEncode & 255);

                // shift right 8 bits and then mask.
                highByte = (byte)((cardToEncode >> 8) & 255);

                // High bytes are in odd indexes, low bytes are in even indexes.
                packet[3 + (2 * i)] = highByte;
                packet[4 + (2 * i)] = lowByte;
            }
            return packet;
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
                case (packetType.keepAlive):
                {
                    packet = EncodePacket(packetType.keepAlive);
                    break;
                }
            }
            return packet;
        }
        
        private static byte[] EncodePacket(packetType type)
        {
            byte[] packet = new byte[1024];
            switch (type)
            {
                case (packetType.handshake):
                {
#if DEBUG_MODE
                    Debug.LogWarning("No request/response parameter passed for handshake. Assuming request. Please use EncodePacket(type, isRequest).");
#endif
                    packet = EncodePacket(packetType.handshake, true);
                    break;
                }
                case (packetType.keepAlive):
                {
                    // the first byte will always be its type.
                    packet[0] = (byte) packetType.keepAlive;
                    break;
                }
            }
            return packet;
        }

        /// <summary>
        /// Decode a packet and manipulate data accordingly. CAN AND WILL throw exceptions if it receives a broken or unidentified packet.
        /// </summary>
        /// <param name="packet">raw packet to manipulate, should be a byte[1024].</param>
        private static async void DecodePacket(byte[] packet)
        {
            bool brokenPacket = false;
            Exception e = new Exception("Unidentified, corrupted or otherwise invalid packet received.");
            switch(packet[0])
            {
                case ((byte) packetType.handshake):
                {
#if DEBUG_MODE
                    Debug.Log("found handshake packet");
#endif
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
                case ((byte) packetType.keepAlive):
                {
#if DEBUG_MODE
                    Debug.Log("found keepAlive packet");
#endif
                    // only the host should respond with a packet on receiving a keepalive.
                    if (currentMode == mode.host)
                    {
                        byte[] responsePacket = EncodePacket(packetType.keepAlive);
                        await stream.WriteAsync(responsePacket, 0, responsePacket.Length);
                    }
                    break;
                }
                case ((byte) packetType.sceneSwitch):
                {
#if DEBUG_MODE
                    Debug.Log("found sceneSwitch packet");
#endif
                    // figure out the length.
                    short length = packet[1];
                    length <<= 8;
                    length += packet[2];

                    // get the bytes and convert them to string.
                    byte[] stringAsBytes = new byte[length];
                    for (int i = 0; i < length; i++)
                    {
                        stringAsBytes[i] = packet[3 + i];
                    }
                    string sceneName = Encoding.UTF8.GetString(stringAsBytes);

                    // request a scene change.
                    requestSceneChange = sceneName;
                    break;
                }
                case ((byte) packetType.cardArray):
                {
                        // New array to replace the old one.
                        List<NewVirtualCardParent> cards = new List<NewVirtualCardParent>();

                        // For every card in the array.
                        for (int i = 0; i < packet[2]; i++)
                        {
                            NewVirtualCardParent card;

                            // rebuild the card from the info.
                            short indexOfCard = packet[3 + (2 * i)];
                            indexOfCard <<= 8;
                            indexOfCard += packet[4 + (2 * i)];

                            // grab its details, check its type, and construct it accordingly.
                            cardIndex.Details cardDetails = cardIndex.Index.GetDetails(indexOfCard);
                            card = ReconstructCard(cardDetails, (NewVirtualCardParent.location)packet[1]);
                            cards.Add(card);
                        }

                        // place the array in the correct spot.
                        switch ((NewVirtualCardParent.location)packet[1])
                        {
                            case (NewVirtualCardParent.location.deck): { playerTwo.Deck = cards; break; }
                            case (NewVirtualCardParent.location.discard): { playerTwo.Discard = cards; break; }
                            case (NewVirtualCardParent.location.hand): { playerTwo.Hand = cards; break; }
                            case (NewVirtualCardParent.location.inPlay): { playerTwo.InPlay = cards; break; }
                        }
                        break;
                }
                case ((byte) packetType.cardMove):
                {
                    // set up these to make code nicer
                    NewVirtualCardParent.location oldLocation = (NewVirtualCardParent.location)packet[1];
                    NewVirtualCardParent.location newLocation = (NewVirtualCardParent.location)packet[1];

                    // get the card's name from the info.
                    short indexOfCard = packet[3];
                    indexOfCard <<= 8;
                    indexOfCard += packet[4];
                    string cardName = cardIndex.Index.GetName(indexOfCard);

                    // Get a copy of the card array to check.
                    // doing this in theory eliminates a LOT of duplicated code.
                    List<NewVirtualCardParent> cardArray = null; 
                    switch ((NewVirtualCardParent.location) packet[1])
                    {
                        case (NewVirtualCardParent.location.deck): { cardArray = playerTwo.Deck; break; }
                        case (NewVirtualCardParent.location.discard): { cardArray = playerTwo.Discard; break; }
                        case (NewVirtualCardParent.location.hand): { cardArray = playerTwo.Hand; break; }
                        case (NewVirtualCardParent.location.inPlay): { cardArray = playerTwo.InPlay; break; }
                    }

                    // Search for the card.
                    for (int i = 0; i < cardArray.Count; i++)
                    {
                        if (cardArray[i].CardName == cardName)
                        {
                            NewVirtualCardParent cardToMove = cardArray[i];
                            // FINALLY figure out where to move the card.
                            if (oldLocation == NewVirtualCardParent.location.hand && newLocation == NewVirtualCardParent.location.inPlay)
                            {
                                // uhhh I don't think I should have to cast here.
                                // Seems like an error.
                                playerTwo.MoveCardToInPlay((MinionParent) cardToMove);
                            }
                            else if (oldLocation == NewVirtualCardParent.location.inPlay && newLocation == NewVirtualCardParent.location.discard) playerTwo.MoveCardToDiscard(cardToMove);
#if DEBUG_MODE
                            else
                            {
                                Debug.LogWarning($"Illegal move from old location {oldLocation} to new location {newLocation}! Double check if a method to move it exists?");
                            }
#endif
                            break;
                        }
                    }
                    break;
                }
                case ((byte)packetType.cardAttack):
                {
                    MinionParent attacker = (MinionParent) playerTwo.InPlay[packet[1]];
                    MinionParent target = (MinionParent) playerOne.InPlay[packet[2]];
                    attacker.Attack(target);
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
            // run in the background constantly listening for info as host.
            if (currentMode == mode.host)
            {
                while (currentState == state.connected)
                {
                    byte[] packet = new byte[1024];

                    // (As far as I know) Make a task to check if this ever finishes on time.
                    Task<int> result = stream.ReadAsync(packet, 0, packet.Length);

                    // wait for either the task to finish or for timeout.
                    // because of how keepalive packets work right now, this essentially means the host will automatically disconnect if the client doesn't do anything for 1 minute.
                    await Task.WhenAny(result, Task.Delay(TimeSpan.FromSeconds(60)));

                    // close the connection on timeout.
                    if (!result.IsCompleted)
                    {
                        CloseConnection();
#if DEBUG_MODE
                    Debug.LogWarning($"timeout on host, closing connection.");
#endif
                    }
                    // close the connection if 0 was received.
                    // 0 means the connection was closed cleanly on the other side. (I think?)
                    else if (result.Result == 0)
                    {
                        CloseConnection();
                    }
                    // decode the packet if one was received in time.
                    else
                    {
                        DecodePacket(packet);
                    }
#if DEBUG_MODE
                    Debug.Log($"post read");
#endif
                }
            }
            // run in the background constantly listening as client.
            if (currentMode == mode.client)
            {
                while (currentState == state.connected)
                {
#if DEBUG_MODE
                    Debug.Log("Client connection while loop");
#endif
                    // setup
                    byte[] packet = new byte[1024];
                    CancellationTokenSource receivedPacket = new CancellationTokenSource();
                    // Ok so... what this basically does is run 2 separate thread-like tasks.
                    // The read and keepalive task run simultaneously and don't run again in the while loop until both are done.
                    List<Task> connectionTasks = new List<Task>();

                    // read task.
                    connectionTasks.Add(Task.Run(async () =>
                    {
#if DEBUG_MODE
                        Debug.Log($"read task");
#endif
                        // wait 10 seconds or until read
                        Task<int> result = stream.ReadAsync(packet, 0, packet.Length);
                        await Task.WhenAny(result, Task.Delay(TimeSpan.FromSeconds(60)));

                        // close if nothing was received.
                        if (!result.IsCompleted)
                        {
#if DEBUG_MODE
                            Debug.LogWarning($"timeout on client, closing connection.");
#endif
                            CloseConnection();
                        }
                        // Also close if connection was cleanly closed.
                        else if (result.Result == 0)
                        {
                            CloseConnection();
                        }
                        // Decode the packet if one was found in time.
                        // also stop the keepalive packet from being sent if it's been less than 5 seconds.
                        else if (result.IsCompleted)
                        {
#if DEBUG_MODE
                            Debug.Log($"found {result.Result} bytes.");
#endif
                            receivedPacket.Cancel();
                            DecodePacket(packet);
                        }
                    }));

                    // keepalive task.
                    connectionTasks.Add(Task.Run(async () =>
                    {
#if DEBUG_MODE
                        Debug.Log($"keeepalive task");
#endif
                        try
                        {
                            // wait 5 seconds or until task was cancelled.
                            await Task.Delay(TimeSpan.FromSeconds(5), receivedPacket.Token);

                            // if it wasn't cancelled, send the keepalive packet.
                            if (!receivedPacket.IsCancellationRequested)
                            {
                                byte[] keepalive = EncodePacket(packetType.keepAlive);
                                await stream.WriteAsync(keepalive, 0, keepalive.Length);
                            }
                        }
                        catch
                        {
#if DEBUG_MODE
                            Debug.LogWarning($"keeepalive task was cancelled.");
#endif
                        }
                    }));

                    // wait for both tasks to finish before doing it again, if there's still a connection.
                    await Task.WhenAll(connectionTasks);
                }
            }
        }

        /// <summary>
        /// Closes the connection by stopping, closing and flushing everything networking related.
        /// </summary>
        public static void CloseConnection()
        {
            try
            {
                server.Stop();
            }
            catch (Exception e)
            {
#if DEBUG_MODE
                Debug.LogWarning($"Exception raised when stopping server: {e.Message}");
#endif
            }
            try
            {
                client.Dispose();
            }
            catch (Exception e)
            {
#if DEBUG_MODE
                Debug.LogWarning($"Exception raised when disposing client: {e.Message}");
#endif
            }
            try
            {
                stream.Flush();
                stream.Close();
            }
            catch (Exception e)
            {
#if DEBUG_MODE
                Debug.LogWarning($"Exception raised when closing stream: {e.Message}");
#endif
            }
            CurrentState = state.disconnected;
        }

        /// <summary>
        /// Reconstruct a card based on its details and location.
        /// </summary>
        /// <param name="cardDetails">Details struct of the card.</param>
        /// <param name="location">Location of the card.</param>
        /// <returns>The card requested from its details.</returns>
        private static NewVirtualCardParent ReconstructCard(cardIndex.Details cardDetails, NewVirtualCardParent.location location)
        {
            NewVirtualCardParent card = null;
            switch (cardDetails.type)
            {
                case (NewVirtualCardParent.type.minion):
                {
                    card = new MinionParent(cardDetails.cost, cardDetails.health, cardDetails.damage, cardDetails.name, cardDetails.type, cardDetails.ability, location);
                    break;
                }
                // Don't make this card if its type is not implemented here.
#if DEBUG_MODE
                default:
                {
                    Debug.LogWarning($"Found unknown card type {cardDetails.type}. Returning null.");
                    break;
                }
#endif
            }
            return card;
        }

        /// <summary>
        /// Send a scene switch to a peer.
        /// </summary>
        /// <param name="sceneName">name of the scene to switch to.</param>
        public static void SendSceneSwitch(string sceneName)
        {
            byte[] packet = EncodePacket(sceneName);
            if (currentState == state.connected)
            {
                stream.Write(packet);
            }
#if DEBUG_MODE
            else
            {
                Debug.LogWarning("Tried to send a scene switch while disconnected! Double check that network manager is connected to a peer.");
            }
#endif
        }
        /// <summary>
        /// Send an array of cards to a connected peer.
        /// </summary>
        /// <param name="cards">List of cards to send.</param>
        /// <param name="location"></param>
        public static void SendCardArray(List<NewVirtualCardParent> cards, NewVirtualCardParent.location location)
        {
            byte[] packet = EncodePacket(cards, location);
            if (currentState == state.connected)
            {
                stream.WriteAsync(packet);
            }
#if DEBUG_MODE
            else
            {
                Debug.LogWarning("Tried to send a deck of cards while disconnected! Double check that network manager is connected to a peer.");
            }
#endif
        }

        /// <summary>
        /// Tell the peer what card you're moving and where it moved to.
        /// </summary>
        /// <param name="card">The card you're moving.</param>
        /// <param name="Oldlocation">The old location of this card.</param>
        /// <param name="newLocation">The new location of this card.</param>
        public static void SendMoveCard(NewVirtualCardParent card, NewVirtualCardParent.location Oldlocation, NewVirtualCardParent.location newLocation)
        {
            byte[] packet = EncodePacket(card, Oldlocation, newLocation);
            if (currentState == state.connected)
            {
                stream.WriteAsync(packet);
            }
#if DEBUG_MODE
            else
            {
                Debug.LogWarning("Tried to send a card while disconnected! Double check that network manager is connected to a peer.");
            }
#endif
        }

        /// <summary>
        /// Tell the peer you attacked a card.
        /// </summary>
        /// <param name="attacker">The card attacking.</param>
        /// <param name="target">The target of the attack.</param>
        public static void SendAttack(MinionParent attacker, MinionParent target)
        {
            byte[] packet = EncodePacket(attacker, target);
            if (currentState == state.connected)
            {
                stream.WriteAsync(packet);
            }
#if DEBUG_MODE
            else
            {
                Debug.LogWarning("Tried to send an attack while disconnected! Double check that network manager is connected to a peer.");
            }
#endif
        }

        public static void TEMPsendpacket()
        {
            byte[] packet = new byte[1024];
            packet = EncodePacket(packetType.keepAlive);
            stream.Write(packet, 0, packet.Length);
        }
    }
}
/*
 * TODO: ADD A CARD'S INDEX IN ITS ARRAY TO THE PACKET
 * This should add support for duplicate cards.
 */