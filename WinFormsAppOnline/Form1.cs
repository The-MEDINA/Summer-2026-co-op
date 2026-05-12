using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace WinFormsAppOnline
{
    public partial class Form1 : Form
    {
        string localAddr = "0.0.0.0";
        // me when brainrot
        int port = 6767;
        TcpListener server = null;
        IPEndPoint endpoint;
        bool stopped = false;
        NetworkStream clientStream;
        public Form1()
        {
            InitializeComponent();
            // Get the computer's host name and their ipv4 address.
            // They're also printed out here.
            var host = Dns.GetHostEntry(Dns.GetHostName());
            DebugInfoBox.Text += $"Host name: {Dns.GetHostName()}" + Environment.NewLine;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    DebugInfoBox.Text += $"ip: {ip}" + Environment.NewLine;
                    localAddr = ip.ToString();
                }
            }
        }
        private void Host_Click(object sender, EventArgs e)
        {
            stopped = false;
            DebugInfoBox.Text = "Start hosting..." + Environment.NewLine;
            DebugInfoBox.Text += "Create TCPListener server and IPAddress local that is null..." + Environment.NewLine;
            DebugInfoBox.Text += "\nGetting into the try/catch code..." + Environment.NewLine;
            try
            {
                DebugInfoBox.Text += $"\nCreating new TCPListener at {localAddr}, {port}..." + Environment.NewLine;

                // The server needs to be listening to any ip address that accesses the port for this to work.
                server = new TcpListener(IPAddress.Any, port);

                DebugInfoBox.Text += $"\nRunning background worker..." + Environment.NewLine;

                // In the actual game, this stuff should be async or in another thread. (I think)
                // It at least shouldn't be intertwined with game code imo. It stops a lot if we don't use async functions.
                // And async functions need to be in another thread.
                HostBackgroundWorker.RunWorkerAsync();
            }
            catch (SocketException err)
            {
                DebugInfoBox.Text += $"\nSocketException: {err}" + Environment.NewLine;
            }
            catch (Exception err)
            {
                DebugInfoBox.Text += $"\nGeneral Exception: {err}" + Environment.NewLine;
            }
            finally
            {
                server?.Stop();
            }
        }

        private void StopHosting_Click(object sender, EventArgs e)
        {
            stopped = true;
            HostBackgroundWorker.CancelAsync();
            Action update = () => StatusBox.Text = $"Disconnected";
            StatusBox.Invoke(update);
            update = () => DebugInfoBox.Text += "Stopping host..." + Environment.NewLine;
            DebugInfoBox.Invoke(update);
            server?.Stop();
        }

        /// <summary>
        /// allegedly a lot of this background worker stuff is windows forms shenanigans.
        /// though they run like separate threads, off doing other stuff while code continues elsewhere.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Action update = () => StatusBox.Text = $"while loop: waiting for connection...";
            StatusBox.Invoke(update);
            update = () => DebugInfoBox.Text += $"\nStart listening..." + Environment.NewLine;
            DebugInfoBox.Invoke(update);

            // Start the server here.
            server.Start();
            // As far as I can tell we need to send raw bytes around and convert them to ascii.
            // This throws a wrench into my plans to send a struct over...
            // I think we can figure out a way to send JSON over though since that's kinda just text.
            Byte[] bytes = new Byte[256];
            String data = null;

            update = () => StatusBox.Text = $"Open at {localAddr}: {port}!";
            StatusBox.Invoke(update);

            // The host also needs a client of its own.
            // I'm sure this can be used for two way communication once we actually implement it.
            TcpClient client = new();
 
            while (!stopped)
            {
                update = () => StatusBox.Text = $"while loop";
                StatusBox.Invoke(update);
                try
                {
                    update = () => StatusBox.Text = $"while client == null";
                    StatusBox.Invoke(update);

                    // This waits to accept anyone connecting to the port.
                    // I think it just takes anyone trying to connect so we might want to add something that checks if it's the game connecting.
                    // we can probably do that by having both the host and client send a message both should be able to read, like a word or something.
                    // like for example if we connect and the client doesn't immediately send us the word "BLITZBURN" then we reject them or something.
                    // (that should be possible)
                    client = server.AcceptTcpClient();

                    update = () => StatusBox.Text = $"Connection made!";
                    StatusBox.Invoke(update);
                    update = () => DebugInfoBox.Text += $"Connection made!" + Environment.NewLine;
                    DebugInfoBox.Invoke(update);
                    update = () => DebugInfoBox.Text += $"Setting up buffer and stream..." + Environment.NewLine;
                    DebugInfoBox.Invoke(update);

                    // This stream is needed to actually get info from whoever connected.
                    NetworkStream stream = client.GetStream();
                    int i; // we need this later
                    while (!stopped)
                    {
                        update = () => StatusBox.Text = $"While loop before if i = stream.read != 0";
                        StatusBox.Invoke(update);

                        // this reads straight into the byte buffer.
                        // stream.read also pauses everything until it gets something (blocking call) so i'm not sure if putting it into an if instead of a while loop really did anything.
                        if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                            update = () => DebugInfoBox.Text += $"Received message: {data}" + Environment.NewLine;
                            DebugInfoBox.Invoke(update);

                            // Process the data sent by the client.
                            data = data.ToUpper();
                        }
                    }
                }
                catch (Exception err)
                {
                    update = () => DebugInfoBox.Text += $"\nExiting with exception: " + err.Message + Environment.NewLine;
                    DebugInfoBox.Invoke(update);
                    StopHosting_Click(this, null);
                }
                finally
                {
                    server?.Stop();
                }
            }
            update = () => StatusBox.Text = $"Exit while loop: Disconnected";
            StatusBox.Invoke(update);
        }

        private void Join_Click(object sender, EventArgs e)
        {
            DebugInfoClient.Text = "Start joining..." + Environment.NewLine;
            ClientBackgroundWorker.RunWorkerAsync();
        }

        private void ClientBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Action update = () => DebugInfoClient.Text += $"Move to background worker..." + Environment.NewLine; StatusBox.Invoke(update);
            StatusBox.Invoke(update);
            try
            {
                update = () => DebugInfoClient.Text += $"Attempting to create TcpClient..." + Environment.NewLine;
                StatusBox.Invoke(update);

                // initialize the client.
                TcpClient client = new();
                // resolve_ip is a custom function. We use its result to find a valid ip.
                endpoint = new IPEndPoint(resolve_ip(HostIp.Text), port);

                update = () => DebugInfoClient.Text += $"Attempting to connect to {endpoint}..." + Environment.NewLine;
                StatusBox.Invoke(update);

                // attempt to connect here.
                client.Connect(endpoint);

                update = () => DebugInfoClient.Text += $"Created TcpClient successfully!" + Environment.NewLine;
                StatusBox.Invoke(update);
                update = () => DebugInfoClient.Text += $"Initializing stream..." + Environment.NewLine;
                StatusBox.Invoke(update);

                // get a stream for the client so that it can read/write to the host.
                clientStream = client.GetStream();
            }
            catch (ArgumentNullException err)
            {
                update = () => DebugInfoClient.Text += $"Exiting with ArgumentNullException: {err.Message}" + Environment.NewLine;
                DebugInfoClient.Invoke(update);
            }
            catch (SocketException err)
            {
                update = () => DebugInfoClient.Text += $"Exiting with SocketException: {err.Message}" + Environment.NewLine;
                DebugInfoClient.Invoke(update);
            }
        }

        private void ClientSend_Click(object sender, EventArgs e)
        {
            DebugInfoClient.Text += $"Attempting to send message: {ClientMessagesBox.Text}" + Environment.NewLine;
            try
            {
                // uhhh so I think this line just reads ClientMessagesBox as ascii and then encodes it as raw bytes?
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(ClientMessagesBox.Text);
                // try to send a message down the client's stream.
                clientStream.Write(data, 0, ClientMessagesBox.Text.Length);
                DebugInfoClient.Text += $"Message sent." + Environment.NewLine;
            }
            catch (Exception err)
            {
                DebugInfoClient.Text += $"Failed to send message. " + err.Message + Environment.NewLine;
            }
        }

        /// <summary>
        /// Resolves the given raw string into an IP. Takes and returns IPV4 ONLY.
        /// </summary>
        /// <param name="raw">EITHER a raw ipv4 address OR a computer's hostname.</param>
        /// <returns>an IPAddress object pointing to the other computer.</returns>
        /// <exception cref="Exception">Exception comes from if no ipv4 address was found.</exception>
        private IPAddress resolve_ip(String raw)
        {
            IPAddress ip;
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
    }
}
// GOD I hope ANY of this made sense.
// Because uhh... honestly some of this went over my own head.
// god bless stack overflow (ai sucks) for getting me through this.
// fingers crossed attempt 2 of programming this (when we do it for the actual game) goes much better.
// :>