
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cardIndex;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

namespace Network
{
    /*
     * Packets are 1024 byte long arrays that are split differently depending on their type.
     * the first byte of every packet will always contain its type. The type is determined by the packetType enum.
     * 
     * --- HANDSHAKE: ---
     * byte 2 determines if it's a request or response.
     * bytes 3 - 39 store the handshakeContent variable.
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
     * byte 3 holds the position of the card in the old location.
     * bytes 4 - 1024 are empty so we can use it later to send more info.
     * 
     * --- CARDADD: ---
     * Byte 1 holds the location of the card to be made.
     * bytes 2 - 3 hold the card index.
     * bytes 4 - 1023 are empty so we can use it later to send more info.
     * 
     * --- CARDATTACK: ---
     * byte 1 holds the position of the card in the attacker's inplay array.
     * byte 2 holds the position of the card in the target's inplay array.
     * byte 3 holds whether this is a card's second attack. 1 means there is one, 0 means there isn't.
     * byte 4 holds an override for the array to grab from. If it's 0, ignore. If it's 1, grab from the hand instead.
     * byte 5 holds an override for the player whose cards to target. 1 means target the other player's cards. 2 means target the opponent player.
     * bytes 6 - 1023 are empty so we can use it later to send more info.
     * 
     * --- CARDDEATH: ---
     * byte 1 holds whether to check player 1 or player 2.
     * bytes 2 - 3 hold the card's nameIndexPosition.
     * byte 4 holds the position of the card in the player's inplay array.
     * byte 5 holds the length of the player's inplay array.
     * bytes 6 - 1023 are empty so we can use it later to send more info.
     * 
     * --- PAUSE_UNPAUSE ---
     * byte 1 holds whether to pause or unpause.
     */
    enum packetType
    {
        handshake,
        keepAlive,
        sceneSwitch,
        cardArray,
        cardMove,
        cardAdd,
        cardAttack,
        cardDeath,
        pause_unpause
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
        searching,
        connected,
        paused
    }
    public static class Networking
    {
        #region VARIABLES_PROPERTIES
        /// <summary>
        /// These variables are for the network manager to manipulate the game.
        /// </summary>
        private static Player playerOne;
        private static Player playerTwo;
        private static Battleground p2Battleground;

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
        private static List<byte[]> previousPackets = new List<byte[]>();
        private static List<List<NewVirtualCardParent>> previousInplay = new List<List<NewVirtualCardParent>>();

        /// <summary>
        /// These variables contain info that needs something else to do what it's asking.
        /// Usually, these are updated by DecodePacket and taken care of in CompleteRequests.
        /// The values here are the default values for each variable. 
        /// When they are checked, they need to be reset to these variables to prevent triggering multiple times.
        /// </summary>
        private static string requestSceneChange = "";
        private static int requestCardInstantiation = -1;
        private static NewVirtualCardParent requestMoveToBattleground = null;
        private static NewVirtualCardParent[] requestAttack = { null, null };
        private static bool requestSecondAttack = false;
        private static int[] requestKill = { -1, -1, -1 };
        private static Player requestPlayer = null;
        private static bool requestInplayCheck = false;

        /// <summary>
        /// delegates to set up events.
        /// </summary>
        public delegate void StateChange(state newState);
        public delegate void NetworkError(string error);

        /// <summary>
        /// These are the actual events that other scripts can subscribe to.
        /// Use these to basically track what network manager is doing from the outside.
        /// </summary>
        public static event StateChange stateChange;
        public static event NetworkError networkError;

        public static Player PlayerOne { get { return playerOne; } set { playerOne = value; } }
        public static Player PlayerTwo { get { return playerTwo; } set { playerTwo = value; } }
        public static Battleground P2Battleground { get { return p2Battleground; } set { p2Battleground = value; } }

        /// <summary>
        /// get/set the current state of the network manager.
        /// </summary>
        public static state CurrentState { 
            get
            {
#if DEBUG_MODE
                // Debug.Log($"Current state: {currentState}");          
#endif
                return currentState;
            } 
            set
            {
#if DEBUG_MODE
                Debug.Log($"changing state to: {value}");          
#endif
                currentState = value;
                stateChange.Invoke(currentState);
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
        #endregion
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
            IPHostEntry hostEntry;
            try
            {
                hostEntry = Dns.GetHostEntry(raw);
                foreach (var ipEntry in hostEntry.AddressList)
                {
                    if (ipEntry.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip = ipEntry;
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG_MODE
                Debug.LogWarning($"Error while resolving IP Address! {e.Message}");
#endif
                networkError.Invoke(e.Message);
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
            // Close existing connection and check if we're already searching.
            if (CurrentState == state.connected) CloseConnection();
            if (CurrentState == state.searching)
            {
#if DEBUG_MODE
                Debug.LogWarning("Host was already started!");
#endif
                return;
            }
            // setup the server
            server = new TcpListener(IPAddress.Any, port);
            client = new TcpClient();
            server.Start();
            CurrentState = state.searching;
#if DEBUG_MODE
            Debug.Log("Server started, waiting to accept client.");
#endif
            // wait for a client
            client = await server.AcceptTcpClientAsync();

#if DEBUG_MODE
            Debug.Log("Client found. Verifying...");
#endif
            // setup the handshake process.
            stream = client.GetStream();
            byte[] handshake = EncodePacket(packetType.handshake, true);
            byte[] response = new byte[1024];
            bool success = false;
            List<Task> handshakeTask = new List<Task>();

            // send a handshake packet to the client and wait for a response.
            await stream.WriteAsync(handshake, 0, handshake.Length);

            // handshake task.
            handshakeTask.Add(Task.Run(async () =>
            {
#if DEBUG_MODE
                Debug.Log($"wait for handshake");
#endif
                // wait 10 seconds or until read
                Task<int> result = stream.ReadAsync(response, 0, response.Length);
                await Task.WhenAny(result, Task.Delay(TimeSpan.FromSeconds(10)));

                // close if nothing was received.
                if (!result.IsCompleted)
                {
#if DEBUG_MODE
                    Debug.LogWarning($"timeout on host handshake, closing connection.");
#endif
                    CloseConnection();
                }
                // Also close if connection was cleanly closed.
                else if (result.Result == 0)
                {
                    CloseConnection();
                }
                // Decode the packet if one was found in time.
                // exit early if the packet is broken, or continue to connection if the packet is valid.
                else if (result.IsCompleted)
                {
#if DEBUG_MODE
                    Debug.Log($"found {result.Result} bytes.");
#endif
                    if (DecodePacket(response).IsFaulted)
                    {
                        Debug.LogWarning("Broken, unknown or otherwise invalid packet received. aborting.");
                    }
                    else
                    {
                        success = true;
                    }
                }
            }));

            // wait for the task to finish.
            await Task.WhenAll(handshakeTask);

            if (success)
            {
#if DEBUG_MODE
                Debug.Log("Success! This is a valid client.");
#endif
                CurrentState = state.connected;
                Connection();
            }
            else
            {
                networkError.Invoke("Unknown error when attempting to connect to a peer. closing connection.");
                CloseConnection();
            }
        }

        /// <summary>
        /// Start a connection as client, and verify the connection.
        /// </summary>
        public static async void StartClient()
        {
            // Close existing connection and check if we're already searching.
            if (CurrentState == state.connected) CloseConnection();
            if (CurrentState == state.searching)
            {
#if DEBUG_MODE
                Debug.LogWarning("Client was already started!");
#endif
                return;
            }
#if DEBUG_MODE
            Debug.Log("starting client.");
#endif
            client = new();
            CurrentState = state.searching;
            bool validClient = false;

            // connect to a valid client, or exit early.
            try
            {
                await client.ConnectAsync(OtherIPv4Address, port);
                validClient = true;
            }
            catch (Exception e)
            {
#if DEBUG_MODE
                Debug.LogWarning($"Aborting StartClient! {e.Message}");
#endif
                networkError.Invoke(e.Message);
            }
            finally
            {
                if (!validClient) CurrentState = state.disconnected;
            }
            if (!validClient) return;

            // start the handshake process here.
            stream = client.GetStream();
            byte[] handshake = EncodePacket(packetType.handshake, false);
            byte[] response = new byte[1024];

#if DEBUG_MODE
            Debug.Log("Connection made, waiting for packet");
#endif
            try
            {
                await stream.ReadAsync(response, 0, response.Length);

#if DEBUG_MODE
                Debug.Log("Packet received. Verifying...");
#endif
                DecodePacket(response);
#if DEBUG_MODE
                Debug.Log("Success! Valid handshake packet. Sending response.");
#endif
                await stream.WriteAsync(handshake, 0, handshake.Length);
                CurrentState = state.connected;
                Connection();
            }
            catch (Exception e)
            {
#if DEBUG_MODE
                Debug.Log("Broken, unknown or otherwise invalid packet received. aborting.");
#endif
                networkError.Invoke($"Broken, unknown or otherwise invalid packet received. Reason: {e.Message} aborting.");
                // terminate connection
                CloseConnection();
            }
        }

        #region ENCODE_PACKETS
        /// <summary>
        /// encode a cardMove packet to send to a peer.
        /// </summary>
        /// <param name="card">card to move</param>
        /// <param name="oldLocation">the card's old location.</param>
        /// <param name="newLocation">the card's new location.</param>
        /// <returns>a byte[1024] packet.</returns>
        private static byte[] EncodePacket(NewVirtualCardParent card, NewVirtualCardParent.location oldLocation, int oldLocationPosition, NewVirtualCardParent.location newLocation)
        {
            byte[] packet = new byte[1024];

            // type and location info.
            packet[0] = (byte) packetType.cardMove;
            packet[1] = (byte) oldLocation;
            packet[2] = (byte) newLocation;
            packet[3] = (byte) oldLocationPosition;

            return packet;
        }

        /// <summary>
        /// Encode a cardAttack packet to send to a peer.
        /// </summary>
        /// <param name="attacker">card that's attacking.</param>
        /// <param name="target">card that's being targetted.</param>
        /// <returns>a byte[1024] packet.</returns>
        private static byte[] EncodePacket(NewVirtualCardParent attacker, NewVirtualCardParent target, bool isSecondAttack)
        {
            byte[] packet = new byte[1024];
            packet[0] = (byte) packetType.cardAttack;

            // encode for spells.
            if (attacker as SpellParent != null)
            {
                packet[1] = (byte)playerOne.Hand.IndexOf(attacker);
                SpellParent spell = (SpellParent)attacker;

                // check if we're targetting ally cards.
                if (spell.Target == SpellParent.spellTarget.allyCards)
                {
                    packet[2] = (byte)playerOne.InPlay.IndexOf(target);
                    if (packet[2] == 255)
                    {
#if DEBUG_MODE
                        Debug.LogWarning("Spell targetting allies is targetting a card outside of player's inPlay array! Attempting to target an enemy card instead.");
#endif
                        packet[2] = (byte)playerTwo.InPlay.IndexOf(target);
                        packet[5] = 1;
                    }
                }
                else
                {
                    packet[2] = (byte)playerTwo.InPlay.IndexOf(target);
                }

                // override to grab from hand.
                packet[4] = 1;
            }

            // encode for minions.
            else
            {
                packet[1] = (byte)playerOne.InPlay.IndexOf(attacker);
                
                // check if this minion is healing.
                MinionParent attackerMinion = (MinionParent)attacker;
                if (attackerMinion.CardEffect == MinionParent.effect.heal)
                {
                    packet[2] = (byte)playerOne.InPlay.IndexOf(target);
                    packet[5] = 1;
                }
                else
                    packet[2] = (byte)playerTwo.InPlay.IndexOf(target);

                packet[4] = 0;
            }
            if (isSecondAttack)
            {
                // check if the second attack is healing.
                if (attacker as TwoAttackParent != null)
                {
                    TwoAttackParent twoAttacker = (TwoAttackParent)attacker;
                    if (twoAttacker.SecondaryCardEffect == MinionParent.effect.heal)
                    {
                        packet[2] = (byte)playerOne.InPlay.IndexOf(target);
                        packet[5] = 1;
                    }
                    else
                        packet[2] = (byte)playerTwo.InPlay.IndexOf(target);
                }
                packet[3] = 1;
            }
            else
            {
                packet[3] = 0;
            }
            // bandaid fix, overwrite second attack value if this card is conscript and it grabbed a target for whatever reason.
            if (attacker.CardName == "Conscript")
            {
                packet[3] = 1;
            }
            return packet;
        }

        private static byte[] EncodePacket(NewVirtualCardParent attacker, Player player, bool isSecondAttack)
        {
            byte[] packet = new byte[1024];
            packet = EncodePacket(attacker, attacker, isSecondAttack);
            packet[5] = 2;
            return packet;
        }
        /// <summary>
        /// encode a sceneSwitch packet to send to a peer.
        /// </summary>
        /// <param name="sceneName">name of the scene to encode.</param>
        /// <returns>a byte[1024] packet.</returns>

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

        /// <summary>
        /// encode a cardArray packet to send to a peer.
        /// </summary>
        /// <param name="cards">list of cards.</param>
        /// <param name="location">location of the list.</param>
        /// <returns>a byte[1024] packet.</returns>
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
        /// Encode a handshake or keepAlive packet to send to a peer.
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
#if DEBUG_MODE
                        Debug.Log("encode handshake");
#endif
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

        /// <summary>
        /// Encodes a cardAdd packet to send to a peer.
        /// </summary>
        /// <param name="card">card to add.</param>
        /// <param name="location">location to add it to.</param>
        /// <returns>a byte[1024] packet.</returns>
        private static byte[] EncodePacket(NewVirtualCardParent card, NewVirtualCardParent.location location)
        {
            byte[] packet = new byte[1024];
            packet[0] = (byte) packetType.cardAdd;
            packet[1] = (byte) location;

            // prepare the card's index to encode into two bytes.
            cardIndex.Details cardDetails = cardIndex.Index.GetDetails(card.CardName);
            short cardToEncode = (short) cardDetails.nameIndexPosition;
            byte highByte = 0;
            byte lowByte = 0;

            // mask out the top 8 bits.
            lowByte = (byte)(cardToEncode & 255);

            // shift right 8 bits and then mask.
            highByte = (byte)((cardToEncode >> 8) & 255);

            packet[2] = highByte;
            packet[3] = lowByte;
            return packet;
        }

        /// <summary>
        /// Encode a keepAlive packet to send to a peer.
        /// </summary>
        /// <param name="type">ALWAYS PASS keepAlive here. (I'll fix this at.. some point.)</param>
        /// <returns>a byte[1024] packet.</returns>
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
        /// encode a cardDeath packet to send to a peer.
        /// </summary>
        /// <param name="isPlayerTwo">whether or not this is player 1 or player 2's card.</param>
        /// <param name="card">card that died.</param>
        /// <returns>a byte[1024] packet.</returns>
        private static byte[] EncodePacket(bool isPlayerTwo, NewVirtualCardParent card)
        {
            byte[] packet = new byte[1024];
            packet[0] = (byte)packetType.cardDeath;

            // encode if this is player two.
            if (isPlayerTwo) packet[1] = 1;
            else packet[1] = 0;

            // encode the card's index.
            short indexToEncode = (short)card.NameIndexPosition;
            byte highByte = 0;
            byte lowByte = 0;

            // mask out the top 8 bits.
            lowByte = (byte)(indexToEncode & 255);

            // shift right 8 bits and then mask.
            highByte = (byte)((indexToEncode >> 8) & 255);

            packet[2] = highByte;
            packet[3] = lowByte;

            // encode the position of the card and length of the inplay array.
            if (isPlayerTwo)
            {
                packet[4] = (byte)playerTwo.InPlay.IndexOf(card);
                packet[5] = (byte)playerTwo.InPlay.Count;
            }
            else
            {
                packet[4] = (byte)playerOne.InPlay.IndexOf(card);
                packet[5] = (byte)playerOne.InPlay.Count;
            }

            return packet;
        }

        /// <summary>
        /// Encode a pause_unpause packet to send to a peer.
        /// </summary>
        /// <param name="pause">Whether to pause or unpause.</param>
        /// <returns>a byte[1024] packet.</returns>
        private static byte[] EncodePacket(bool pause)
        {
            byte[] packet = new byte[1024];
            packet[0] = (byte)packetType.pause_unpause;
            if (pause) packet[1] = 1;
            else packet[1] = 0;
            return packet;
        }
        #endregion

        /// <summary>
        /// Decode a packet and manipulate data accordingly. CAN AND WILL throw exceptions if it receives a broken or unidentified packet.
        /// </summary>
        /// <param name="packet">raw packet to manipulate, should be a byte[1024].</param>
        private static async Task DecodePacket(byte[] packet)
        {
            bool brokenPacket = false;
            Exception e = new Exception("Unidentified, corrupted or otherwise invalid packet received.");
            AddToList(packet);
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
#if DEBUG_MODE
                        Debug.Log("Found cardArray packet");
#endif
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

                            // grab its name and create the card.
                            string cardName = cardIndex.Index.GetName(indexOfCard);
                            card = cardIndex.Index.CreateCard(cardName, (NewVirtualCardParent.location)packet[1]);
                            cards.Add(card);
                        }

                        // place the array in the correct spot.
                        switch ((NewVirtualCardParent.location)packet[1])
                        {
                            case (NewVirtualCardParent.location.deck): { playerTwo.Deck = cards; break; }
                            case (NewVirtualCardParent.location.discard): { playerTwo.Discard = cards; break; }
                            case (NewVirtualCardParent.location.hand): { playerTwo.Hand = cards; break; }
                            case (NewVirtualCardParent.location.inPlay): 
                                {
                                    AddToPreviousInplays(cards);
                                    requestInplayCheck = true;
                                    break; 
                                }
                        }
                        break;
                }
                case ((byte) packetType.cardMove):
                {
#if DEBUG_MODE
                    Debug.Log("found cardMove packet");
#endif
                    // setup to make code nicer
                    List<NewVirtualCardParent> oldList = null;
                    NewVirtualCardParent.location oldLocation = (NewVirtualCardParent.location)packet[1];
                    NewVirtualCardParent.location newLocation = (NewVirtualCardParent.location)packet[2];
                    NewVirtualCardParent cardToMove = null;
                    
                    // setup old location
                    switch ((NewVirtualCardParent.location)packet[1])
                    {
                        case (NewVirtualCardParent.location.deck): { oldList = playerTwo.Deck; break; }
                        case (NewVirtualCardParent.location.discard): { oldList = playerTwo.Discard; break; }
                        case (NewVirtualCardParent.location.hand): { oldList = playerTwo.Hand; break; }
                        case (NewVirtualCardParent.location.inPlay): { oldList = playerTwo.InPlay; break; }
                    }

                    // grab card and attempt to move
                    if (packet[3] < oldList.Count)
                    {
                        cardToMove = oldList[packet[3]];
                    }
                    else
                    {
#if DEBUG_MODE
                        Debug.LogWarning($"Index of card to move ({packet[3]}) is invalid! (size is {oldList.Count}). Ignoring move.");
#endif
                        break;
                    }
                    if (oldLocation == NewVirtualCardParent.location.hand && newLocation == NewVirtualCardParent.location.inPlay)
                    {
                        requestMoveToBattleground = cardToMove;
                    }
                    else if (oldLocation == NewVirtualCardParent.location.inPlay && newLocation == NewVirtualCardParent.location.discard)
                    {
                        playerTwo.MoveCardToDiscard(cardToMove);
                    }
#if DEBUG_MODE
                    else
                    {
                        Debug.LogWarning($"Illegal move from old location {oldLocation} to new location {newLocation}! Double check if a method to move it exists?");
                    }
#endif
                    break;
                }
                case ((byte) packetType.cardAdd):
                {
#if DEBUG_MODE
                    Debug.Log("found cardAdd packet");
#endif
                    NewVirtualCardParent card;

                    // rebuild the card from the info.
                    short indexOfCard = packet[2];
                    indexOfCard <<= 8;
                    indexOfCard += packet[3];

                        // grab its name and create the card.
                        string cardName = cardIndex.Index.GetName(indexOfCard);
                        card = cardIndex.Index.CreateCard(cardName, (NewVirtualCardParent.location)packet[1]);

                        // place the card in the correct spot.
                        switch ((NewVirtualCardParent.location)packet[1])
                    {
                        case (NewVirtualCardParent.location.deck): { playerTwo.Deck.Add(card); break; }
                        case (NewVirtualCardParent.location.discard): { playerTwo.Discard.Add(card); break; }
                        case (NewVirtualCardParent.location.hand): { /*playerTwo.Hand.Add(card);*/ requestCardInstantiation = -2; break; }
                        case (NewVirtualCardParent.location.inPlay): { requestCardInstantiation = indexOfCard; break; }
                    }
                    break;
                }
                case ((byte) packetType.cardAttack):
                {
#if DEBUG_MODE
                        Debug.Log("found cardAttack packet");
#endif
                    NewVirtualCardParent attacker = null;
                    NewVirtualCardParent target = null;

                    // check for any overrides.
                    // hand override.
                    if (packet[4] == 1)
                    {
                        if (playerTwo.Hand.Count <= packet[1])
                        {
#if DEBUG_MODE
                            Debug.LogWarning($"playertwo.Hand ({playerTwo.Hand.Count}) is less than or equal to {packet[1]}. Ignoring card attack to keep connection alive.");
#endif
                            break;
                        }
                        attacker = playerTwo.Hand[packet[1]];
                    }
                    else
                    {
                        if (playerTwo.InPlay.Count <= packet[1])
                        {
#if DEBUG_MODE
                            Debug.LogWarning($"playertwo.InPlay ({playerTwo.InPlay.Count}) is less than or equal to {packet[1]}. Ignoring card attack to keep connection alive.");
#endif
                            break;
                        }
                            attacker = playerTwo.InPlay[packet[1]];
                    }

                    if (attacker as SpellParent != null)
                    {
                        SpellParent spell = (SpellParent)attacker;
                        // overwrite the target if this card is a spell that targets allies.
                        if (spell.Target == SpellParent.spellTarget.allyCards)
                        {
                            if (packet[2] != 255)
                            {
                                if (packet[5] == 1)
                                {
#if DEBUG_MODE
                                    Debug.LogWarning("Found card incorrectly targetting enemies when it should target allies! Double check this card is working right?");
#endif
                                    target = playerOne.InPlay[packet[2]];
                                }
                                else
                                {
                                    target = playerTwo.InPlay[packet[2]];
                                }
                            }
#if DEBUG_MODE
                            else
                            {
                                Debug.LogWarning("Found spell with an invalid target! Setting to no target.");
                            }
#endif
                        }
                        // target of index 255 implies this spell has no target.
                        else if (packet[2] != 255)
                        {
                            // target player 1's cards like normal if the index is valid.
                            target = playerOne.InPlay[packet[2]];
                        }
                    }
                    else
                    {
                        if (packet[5] == 1 && packet[2] < playerTwo.InPlay.Count)
                        {
                            target = playerTwo.InPlay[packet[2]];
                        }
                        else if (packet[2] < playerOne.InPlay.Count)
                        {
                            target = playerOne.InPlay[packet[2]];
                        }
                        else if (packet[5] == 2)
                        {
                            requestPlayer = playerOne;
                        }
#if DEBUG_MODE
                        else
                        {
                            DesyncWarning($"Minion is targetting a card at an invalid index! ({packet[2]}).");
                        }
#endif
                    }
                    requestAttack[0] = attacker;
                    requestAttack[1] = target;
                    if (packet[3] == 1) requestSecondAttack = true;
                    else requestSecondAttack = false;
                    break;
                }
                case ((byte) packetType.cardDeath):
                {
#if DEBUG_MODE
                        Debug.Log("found cardDeath packet");
#endif
                    // this is flipped since the peer's player 2 is us, player 1.
                    if (packet[1] == 0) requestPlayer = playerTwo;
                    else requestPlayer = playerOne;

                    // rebuild the card's index.
                    short indexOfCard = packet[2];
                    indexOfCard <<= 8;
                    indexOfCard += packet[3];

                    // prepare the request.
                    requestKill[0] = indexOfCard;
                    requestKill[1] = packet[4];
                    requestKill[2] = packet[5];
                    break;
                }
                case ((byte) packetType.pause_unpause):
                {
#if DEBUG_MODE
                    Debug.Log("found pause/unpause packet");
#endif
                    if (packet[1] == 1) CurrentState = state.paused;
                    else CurrentState = state.connected;
                    break;
                }
                default:
                {
                        // ONLY throw exceptions if there is not an active connection.
                        if (CurrentState != state.connected) 
                        { 
                            throw e; 
                        }
#if DEBUG_MODE
                    Debug.LogWarning("Unidentified, corrupted or otherwise invalid packet received. Ignoring packet to keep connection alive.");
#endif
                    break;
                }
            }
            // also only throw exceptions if there is not an active connection.
            if (brokenPacket && CurrentState == state.disconnected)
            {
                throw e;
            }
#if DEBUG_MODE
            else if (brokenPacket)
            {
                Debug.LogWarning("Broken packet received.");
            }
#endif
        }
        private static async void Connection()
        {
#if DEBUG_MODE
            Debug.Log($"entering Connection()");
#endif
            // run in the background constantly listening for info as host.
            if (currentMode == mode.host)
            {
                while (CurrentState == state.connected || CurrentState == state.paused)
                {
#if DEBUG_MODE
                    Debug.Log("Host connection while loop");
#endif
                    byte[] packet = new byte[1024];

                    // (As far as I know) Make a task to check if this ever finishes on time.
                    Task<int> result = stream.ReadAsync(packet, 0, packet.Length);

                    // wait for either the task to finish or for timeout.
                    // because of how keepalive packets work right now, this essentially means the host will automatically disconnect if the client doesn't do anything for 1 minute.
                    await Task.WhenAny(result, Task.Delay(TimeSpan.FromSeconds(600)));

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
                        await DecodePacket(packet);
                    }
#if DEBUG_MODE
                    Debug.Log($"post read");
#endif
                    // complete any requests that came from other threads like DecodePacket.
                    CompleteRequests();
                }
            }
            // run in the background constantly listening as client.
            if (currentMode == mode.client)
            {
                while (CurrentState == state.connected || CurrentState == state.paused)
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
                        await Task.WhenAny(result, Task.Delay(TimeSpan.FromSeconds(600)));

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
                            await DecodePacket(packet);
                        }
                    }));

                    // keepalive task.
                    connectionTasks.Add(Task.Run(async () =>
                    {
#if DEBUG_MODE
                        Debug.Log($"keepalive task");
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
                            Debug.Log($"keepalive task was cancelled.");
#endif
                        }
                    }));

                    // wait for both tasks to finish before doing it again, if there's still a connection.
                    await Task.WhenAll(connectionTasks);

                    // complete any requests that came from other threads like DecodePacket.
                    CompleteRequests();
                }
            }
        }

        /// <summary>
        /// Complete any requests that come from DecodePacket and all the 'request' variables.
        /// </summary>
        private static void CompleteRequests()
        {
            // scene change.
            if (Networking.requestSceneChange != "")
            {
                SceneManager.LoadScene(Networking.requestSceneChange);
                Networking.requestSceneChange = "";
            }

            // card instantiation.
            if (Networking.requestCardInstantiation == -2)
            {
                p2Battleground.DrawCardToHand();
                Networking.requestCardInstantiation = -1;
            }
            // create token creatures.
            else if (Networking.requestCardInstantiation == cardIndex.Index.GetDetails("Kitten").nameIndexPosition)
            {
                playerTwo.CommanderCard.PerformAbility();
                Networking.requestCardInstantiation = -1;
            }

            // move to battleground.
            if (requestMoveToBattleground != null)
            {
                CardSelectionManager.Instance.PlayCardToBattleground(requestMoveToBattleground.UnityObject.GetComponent<CardClickHandler>());
                requestMoveToBattleground = null;
            }

            // attack.
            if (requestAttack[0] as MinionParent != null && requestAttack[1] as MinionParent != null)
            {
                // store player 1's selection if one was made.
                CardClickHandler previousSelection = null;
                if (CardSelectionManager.Instance.SelectedCardObject != null) previousSelection = CardSelectionManager.Instance.SelectedCardObject;

                // action.
                CardSelectionManager.Instance.SelectedCardObject = requestAttack[0].UnityObject.GetComponent<CardClickHandler>();
                CardSelectionManager.Instance.TryAttackTarget(requestAttack[1].UnityObject.GetComponent<CardClickHandler>(), requestSecondAttack);
                requestAttack[0] = null;
                requestAttack[1] = null;
                requestSecondAttack = false;

                // restore player 1's selection if needed.
                if (previousSelection != null) CardSelectionManager.Instance.SelectedCardObject = previousSelection;
            }
            // minion attacks player.
            else if (requestAttack[0] as MinionParent != null && requestPlayer != null)
            {                
                // store player 1's selection if one was made.
                CardClickHandler previousSelection = null;
                if (CardSelectionManager.Instance.SelectedCardObject != null) previousSelection = CardSelectionManager.Instance.SelectedCardObject;

                CardSelectionManager.Instance.SelectedCardObject = requestAttack[0].UnityObject.GetComponent<CardClickHandler>();
                CardSelectionManager.Instance.TryAttackPlayer(requestSecondAttack);
                requestAttack[0] = null;
                requestAttack[1] = null;
                requestSecondAttack = false;
                requestPlayer = null;

                // restore player 1's selection if needed.
                if (previousSelection != null) CardSelectionManager.Instance.SelectedCardObject = previousSelection;
            }
            // spell action.
            else if (requestAttack[0] as SpellParent != null)
            {
                // store player 1's selection if one was made.
                CardClickHandler previousSelection = null;
                if (CardSelectionManager.Instance.SelectedCardObject != null) previousSelection = CardSelectionManager.Instance.SelectedCardObject;

                // action.
                CardSelectionManager.Instance.SelectedCardObject = requestAttack[0].UnityObject.GetComponent<CardClickHandler>();
                if (!requestSecondAttack)
                {
                    CardSelectionManager.Instance.TrySpellTarget(requestAttack[1].UnityObject.GetComponent<CardClickHandler>());
                }
                else CardSelectionManager.Instance.TrySpellNoTarget();
                requestAttack[0] = null;
                requestAttack[1] = null;
                requestSecondAttack = false;

                // restore player 1's selection if needed.
                if (previousSelection != null) CardSelectionManager.Instance.SelectedCardObject = previousSelection;
            }

            // death.
            if (requestPlayer != null && requestKill[0] != -1)
            {
                // setup
                int cardNameIndex = requestKill[0];
                int indexOfCard = requestKill[1];
                int inplayCount = requestKill[2];
                
                // ONLY kill the card if it's not already dead.
                if(inplayCount == requestPlayer.InPlay.Count && // these shouldn't be the same if the card died.
                    requestPlayer.InPlay[indexOfCard].NameIndexPosition == cardNameIndex) // card shouldn't be found if it died.
                {
#if DEBUG_MODE
                    Debug.LogWarning("Found card that should've died. Attempting to manually kill to avoid desync.");
#endif
                    requestPlayer.InPlay[indexOfCard].UnityObject.SetActive(false);
                    if (requestPlayer.InPlay[indexOfCard] is MinionParent)
                    {
                        MinionParent killThis = (MinionParent) requestPlayer.InPlay[indexOfCard];
                        killThis.Death();
                    }
                }
                requestPlayer = null;
                for (int i = 0; i < requestKill.Length; i++)
                {
                    requestKill[i] = -1;
                }
            }
            // inPlay cards.
            if (requestInplayCheck)
            {
                int index = (previousInplay.Count < 8) ? previousInplay.Count - 1: 7;
                if (!SameCardArray(playerTwo.InPlay, previousInplay[index]))
                {
#if DEBUG_MODE
                    DesyncWarning("Player 2 in play arrays don't match! attempting to resolve...");
#endif
                    bool foundSolution = false;
                    // search for a previous deck that matches the old one
                    for (int i = previousInplay.Count - 1; i >= 0; i--)
                    {
                        if (SameCardArray(playerTwo.InPlay, previousInplay[i]))
                        {
#if DEBUG_MODE
                            Debug.Log("Found a previous array that matches the player's current array. Reverting.");
#endif
                            // swap out the current deck for an old one
                            while (playerTwo.InPlay.Count != 0)
                            {
                                MinionParent killThis = (MinionParent)playerTwo.InPlay[0];
                                killThis.Death();
                            }
                            for (int j = 0; j < previousInplay[i].Count; j++)
                            {
                                p2Battleground.SpawnCardToInPlay(previousInplay[i][j]);
                            }
                            foundSolution = true;
                            break;
                        }
                    }
                    if (!foundSolution)
                    {
#if DEBUG_MODE
                        Debug.LogError("No previously stored array matches incoming array! Pausing connection and rebuilding deck.");
#endif
                        CurrentState = state.paused;
                        SendPauseUnpause(true);
                        // remove the current cards
                        while (playerTwo.InPlay.Count != 0)
                        {
                            if (playerTwo.InPlay[0] as MinionParent != null)
                            {
                                MinionParent killThis = (MinionParent)playerTwo.InPlay[0];
                                killThis.Death();
                            }
                            // on the offchance a spell ends up in InPlay somehow remove it manually
                            else
                            {
#if DEBUG_MODE
                                Debug.LogWarning("Spell or card that cannot be cast to minion in InPlay! Attempting to manually remove...");
                                NewVirtualCardParent killThisInvalid = (NewVirtualCardParent)playerTwo.InPlay[0];
                                killThisInvalid.UnityObject.SetActive(false);
                                playerTwo.InPlay.RemoveAt(0);

#endif
                            }
                        }
                        // repopulate the array with the one from the cardArray packet
                        for (int j = 0; j < previousInplay[index].Count; j++)
                        {
                            p2Battleground.SpawnCardToInPlay(previousInplay[index][j]);
                        }
                        CardSelectionManager.Instance.RepositionInPlayCards(playerTwo);
                        CurrentState = state.connected;
                        SendPauseUnpause(false);
                    }
                }
                requestInplayCheck = false;
            }
        }

        /// <summary>
        /// Add to the list of previous packets and cap the size to 8.
        /// </summary>
        /// <param name="packet">packet to add.</param>
        private static void AddToList(byte[] packet)
        {
            previousPackets.Add(packet);
            if (previousPackets.Count > 8) previousPackets.RemoveAt(0); 
        }

        /// <summary>
        /// See if both card arrays are the same by their nameIndexPosition value.
        /// </summary>
        /// <param name="first">first array to check.</param>
        /// <param name="second">second array to check.</param>
        /// <returns>whether both arrays are the same.</returns>
        private static bool SameCardArray(List<NewVirtualCardParent> first, List<NewVirtualCardParent> second)
        {
            if (first.Count != second.Count) return false;
            for (int i = 0; i < first.Count; i++)
            {
                if (first[i].NameIndexPosition != second[i].NameIndexPosition) return false;
            }
            return true;
        }

        #region PUBLIC_METHODS
        /// <summary>
        /// Add to the list of previous inplay arrays and cap the size to 8.
        /// </summary>
        /// <param name="inPlay"></param>
        public static void AddToPreviousInplays(List<NewVirtualCardParent> inPlay)
        {
#if DEBUG_MODE
            Debug.Log("Added to previous inPlay arrays");
#endif
            previousInplay.Add(inPlay);
            if (previousInplay.Count > 8) previousInplay.RemoveAt(0);
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
        /// Send a scene switch to a peer.
        /// </summary>
        /// <param name="sceneName">name of the scene to switch to.</param>
        public static void SendSceneSwitch(string sceneName)
        {
#if DEBUG_MODE
            Debug.Log("encode scene switch");
#endif
            byte[] packet = EncodePacket(sceneName);
            if (CurrentState != state.disconnected)
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
#if DEBUG_MODE
            Debug.Log("encode card array");
#endif
            byte[] packet = EncodePacket(cards, location);
            if (CurrentState != state.disconnected)
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
        public static void SendCardMove(NewVirtualCardParent card, NewVirtualCardParent.location Oldlocation, int oldLocationPosition, NewVirtualCardParent.location newLocation)
        {
#if DEBUG_MODE
            Debug.Log("encode card move");
#endif
            byte[] packet = EncodePacket(card, Oldlocation, oldLocationPosition, newLocation);
            if (CurrentState != state.disconnected)
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

        public static void SendCardAdd(NewVirtualCardParent card, NewVirtualCardParent.location location)
        {
#if DEBUG_MODE
            Debug.Log("encode card add");
#endif
            byte[] packet = EncodePacket(card, location);
            if (CurrentState != state.disconnected)
            {
                stream.WriteAsync(packet);
            }
#if DEBUG_MODE
            else
            {
                Debug.LogWarning("Tried to send an add card while disconnected! Double check that network manager is connected to a peer.");
            }
#endif
        }

        /// <summary>
        /// Tell the peer you attacked a card.
        /// </summary>
        /// <param name="attacker">The card attacking.</param>
        /// <param name="target">The target of the attack.</param>
        /// <param name="isSecondAttack">Whether this is the attacker's second attack.</param>
        public static void SendCardAttack(NewVirtualCardParent attacker, NewVirtualCardParent target, bool isSecondAttack)
        {
#if DEBUG_MODE
            Debug.Log("encode card attack");
#endif
            byte[] packet = EncodePacket(attacker, target, isSecondAttack);
            if (CurrentState != state.disconnected)
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

        /// <summary>
        /// Tell the peer you attacked a player.
        /// </summary>
        /// <param name="attacker">the card attacking.</param>
        /// <param name="player">the player being targetted.</param>
        /// <param name="isSecondAttack">Whether this is the attacker's second attack.</param>
        public static void SendCardAttackPlayer(NewVirtualCardParent attacker, Player player, bool isSecondAttack)
        {
#if DEBUG_MODE
            Debug.Log("encode card attack on player");
#endif
            byte[] packet = EncodePacket(attacker, player, isSecondAttack);
            if (CurrentState != state.disconnected)
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

        /// <summary>
        /// Tell the peer a card died.
        /// </summary>
        /// <param name="isPlayerTwo">whether or not this is player 1 or player 2's card.</param>
        /// <param name="cardToDie">the card that died.</param>
        public static void SendCardDeath(bool isPlayerTwo, NewVirtualCardParent cardToDie)
        {
#if DEBUG_MODE
            Debug.Log("encode card death");
#endif
            byte[] packet = EncodePacket(isPlayerTwo, cardToDie); 
            if (CurrentState != state.disconnected)
            {
                stream.WriteAsync(packet);
            }
#if DEBUG_MODE
            else
            {
                Debug.LogWarning("Tried to send a death while disconnected! Double check that network manager is connected to a peer.");
            }
#endif
        }

        /// <summary>
        /// Tell the peer to pause or unpause.
        /// </summary>
        /// <param name="pause">whether to pause or unpause.</param>
        public static void SendPauseUnpause(bool pause)
        {
#if DEBUG_MODE
            Debug.Log("Encode pause/unpause packet");
#endif
            byte[] packet = EncodePacket(pause);
            if (CurrentState != state.disconnected)
            {
                stream.WriteAsync(packet);
            }
#if DEBUG_MODE
            else
            {
                Debug.LogWarning("Tried to pause/unpause while disconnected! Double check that network manager is connected to a peer.");
            }
#endif
        }

        public static void DesyncWarning(string warning)
        {
#if DEBUG_MODE
            Debug.LogWarning($"Desync detected! {warning}.");
#endif
        }
        #endregion
    }
}