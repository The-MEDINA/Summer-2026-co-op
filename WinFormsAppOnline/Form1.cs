using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace WinFormsAppOnline
{
    public partial class Form1 : Form
    {
        // TODO: Find a way to make the client connect without initially knowing the ip.
        // This sounds a lot harder.
        string localAddr = "0.0.0.0";
        // me when brainrot
        int port = 6767;
        TcpListener server = null;
        IPAddress local;
        IPEndPoint endpoint;
        bool stopped = false;
        NetworkStream clientStream;
        public Form1()
        {
            InitializeComponent();
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
            local = IPAddress.Parse(localAddr);
            endpoint = new IPEndPoint(local, port);
        }
        private void Host_Click(object sender, EventArgs e)
        {
            stopped = false;
            DebugInfoBox.Text = "Start hosting..." + Environment.NewLine;
            DebugInfoBox.Text += "Create TCPListener server and IPAddress local that is null..." + Environment.NewLine;
            DebugInfoBox.Text += "\nGetting into the try/catch code..." + Environment.NewLine;
            try
            {
                DebugInfoBox.Text += $"\nParsing local address {localAddr}..." + Environment.NewLine;
                local = IPAddress.Parse(localAddr);
                DebugInfoBox.Text += $"\nCreating new TCPListener at {localAddr}, {port}..." + Environment.NewLine;
                server = new TcpListener(IPAddress.Any, port);
                DebugInfoBox.Text += $"\nRunning background worker..." + Environment.NewLine;
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
            //DebugInfoBox.Text += "Stopping host..." + Environment.NewLine;
            //StatusBox.Text = "Disconnected";
            server?.Stop();
        }

        /// <summary>
        /// allegedly a lot of this background worker stuff is windows forms shenanigans.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Action update = () => StatusBox.Text = $"while loop: waiting for connection...";
            StatusBox.Invoke(update);

            update = () => DebugInfoBox.Text += $"\nStart listening..." + Environment.NewLine;
            DebugInfoBox.Invoke(update);

            server.Start();
            Byte[] bytes = new Byte[256];
            String data = null;

            update = () => StatusBox.Text = $"Open at {localAddr}: {port}!";
            StatusBox.Invoke(update);
            TcpClient client = new();
            while (!stopped)
            {
                update = () => StatusBox.Text = $"while loop";
                StatusBox.Invoke(update);
                try
                {
                    update = () => StatusBox.Text = $"while client == null";
                    StatusBox.Invoke(update);

                    client = server.AcceptTcpClient();

                    update = () => StatusBox.Text = $"Connection made!";
                    StatusBox.Invoke(update);
                    update = () => DebugInfoBox.Text += $"Connection made!" + Environment.NewLine;
                    DebugInfoBox.Invoke(update);
                    update = () => DebugInfoBox.Text += $"Setting up buffer and stream..." + Environment.NewLine;
                    DebugInfoBox.Invoke(update);

                    NetworkStream stream = client.GetStream();
                    int i;
                    while (!stopped)
                    {
                        update = () => StatusBox.Text = $"While loop before if i = stream.read != 0";
                        StatusBox.Invoke(update);
                        if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            update = () => DebugInfoBox.Text += $"Received message: {data}" + Environment.NewLine;
                            DebugInfoBox.Invoke(update);

                            // Process the data sent by the client.
                            data = data.ToUpper();

                            // byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            // Send back a response.
                            // stream.Write(msg, 0, msg.Length);
                            // Console.WriteLine("Sent: {0}", data);
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
            // Byte[] data = new byte[256];
            Action update = () => DebugInfoClient.Text += $"Move to background worker..." + Environment.NewLine; StatusBox.Invoke(update);
            StatusBox.Invoke(update);
            /*IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation t in connections)
            {
                update = () => DebugInfoClient.Text += $"Local endpoint: {t.LocalEndPoint.Address}" + Environment.NewLine;
                StatusBox.Invoke(update);
                update = () => DebugInfoClient.Text += $"Remote endpoint: {t.RemoteEndPoint.Address}" + Environment.NewLine;
                StatusBox.Invoke(update);
                update = () => DebugInfoClient.Text += $"{t.State}" + Environment.NewLine;
                StatusBox.Invoke(update);
            }*/
            try
            {
                update = () => DebugInfoClient.Text += $"Attempting to create TcpClient..." + Environment.NewLine;
                StatusBox.Invoke(update);

                TcpClient client = new();
                endpoint = new IPEndPoint(resolve_ip(HostIp.Text), port);

                update = () => DebugInfoClient.Text += $"Attempting to connect to {endpoint}..." + Environment.NewLine;
                StatusBox.Invoke(update);

                client.Connect(endpoint);

                update = () => DebugInfoClient.Text += $"Created TcpClient successfully!" + Environment.NewLine;
                StatusBox.Invoke(update);
                update = () => DebugInfoClient.Text += $"Initializing stream..." + Environment.NewLine;
                StatusBox.Invoke(update);

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
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(ClientMessagesBox.Text);
                clientStream.Write(data, 0, ClientMessagesBox.Text.Length);
                DebugInfoClient.Text += $"Message sent." + Environment.NewLine;
            }
            catch (Exception err)
            {
                DebugInfoClient.Text += $"Failed to send message. " + err.Message + Environment.NewLine;
            }
        }

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
