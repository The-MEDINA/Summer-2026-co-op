using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinFormsAppOnline
{
    public partial class Form1 : Form
    {
        string localAddr = "127.0.0.1";
        // me when brainrot
        int port = 6767;
        TcpListener server = null;
        IPAddress local = null;
        bool stopped = false;
        public Form1()
        {
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
                server = new TcpListener(local, port);
                DebugInfoBox.Text += $"\nStart listening..." + Environment.NewLine;
                server.Start();
                StatusBox.Text = $"Open at {localAddr}: {port}!";
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
            DebugInfoBox.Text += "Stopping host..." + Environment.NewLine;
            StatusBox.Text = "Disconnected";
            server?.Stop();
        }

        /// <summary>
        /// allegedly a lot of this background worker stuff is windows forms shenanigans.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while(!stopped)
            {
                Action action = () => StatusBox.Text = $"while loop: waiting for connection...";
                StatusBox.Invoke(action);
            }
            Action exitAction = () => StatusBox.Text = $"Exit while loop: Disconnected";
            StatusBox.Invoke(exitAction);
        }
    }
}
