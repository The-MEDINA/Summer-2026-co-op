using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinFormsAppOnline
{
    public partial class Form1 : Form
    {
        // TODO: find a way to fix this.
        // Allegedly, 127.0.0.1 is this machine only.
        // find some way to make this your hostname, whilst also letting the client find that.
        string localAddr = "192.168.1.214";
        // me when brainrot
        int port = 6767;
        TcpListener server = null;
        IPAddress local;
        IPEndPoint endpoint;
        bool stopped = false;
        public Form1()
        {
            local = IPAddress.Parse(localAddr);
            endpoint = new IPEndPoint(local, port);
            InitializeComponent();
        }
        private void Host_Click(object sender, EventArgs e)
        {
            stopped = false;
            DebugInfoBox.Text = "Start hosting..." + Environment.NewLine;
            DebugInfoBox.Text += "Create TCPListener server and IPAddress local that is null..." + Environment.NewLine;
            DebugInfoBox.Text += "\nGetting into the try/catch code..." + Environment.NewLine;
            try
            {
                DebugInfoBox.Text += $"\nCreating local address {localAddr}..." + Environment.NewLine;
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

            update = () => StatusBox.Text = $"Open at {localAddr}: {port}!";
            StatusBox.Invoke(update);
            TcpClient client = new();
            while (!stopped)
            {
                update = () => StatusBox.Text = $"while loop";
                StatusBox.Invoke(update);
                try
                {
                    //while (client == null)
                    //{
                        update = () => StatusBox.Text = $"while client == null";
                        StatusBox.Invoke(update);
                        client = server.AcceptTcpClient();
                        update = () => StatusBox.Text = $"Connection made!";
                        StatusBox.Invoke(update);
                        update = () => DebugInfoBox.Text += $"Connection made!" + Environment.NewLine;
                        DebugInfoBox.Invoke(update);
                    //}
                }
                catch (Exception err)
                {
                    update = () => DebugInfoBox.Text += $"\nExiting with exception: " + err.Message + Environment.NewLine;
                    DebugInfoBox.Invoke(update);
                    StopHosting_Click(this, null);
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
            Action update = () => DebugInfoClient.Text += $"Move to background worker..." + Environment.NewLine;
            StatusBox.Invoke(update);
            try
            {
                update = () => DebugInfoClient.Text += $"Attempting to create TcpClient..." + Environment.NewLine;
                StatusBox.Invoke(update);

                TcpClient client = new();

                update = () => DebugInfoClient.Text += $"Attempting to connect to {endpoint}..." + Environment.NewLine;
                StatusBox.Invoke(update);

                client.Connect(endpoint);

                update = () => DebugInfoClient.Text += $"Created TcpClient successfully!" + Environment.NewLine;
                StatusBox.Invoke(update);
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
    }
}
