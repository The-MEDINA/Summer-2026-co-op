using System.Net;
using System.Net.Sockets;

namespace WinFormsAppOnline
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            // Console.WriteLine(hostName);
            // Get the IP
            string myIP = Dns.GetHostEntry(hostName).AddressList[0].ToString();
            IP.Text = $"{hostName}: IP Address is {myIP}\nI don't know if this was needed.\n:>";
            // Console.WriteLine("My IP Address is :" + myIP);
            // Console.ReadKey();
        }

        private void Host_Click(object sender, EventArgs e)
        {
            DebugInfoBox.Text = "Start hosting..." + Environment.NewLine;
            // ip address 127.0.0.1 port 6767 (i'm so brainrotted)
            string localAddr = "127.0.0.1";
            int port = 6767;
            DebugInfoBox.Text += "Create TCPListener server and IPAddress local that is null..." + Environment.NewLine;
            TcpListener server = null;
            IPAddress local = null;
            DebugInfoBox.Text += "\nGetting into the try/catch code..." + Environment.NewLine;
            try
            {
                DebugInfoBox.Text += $"\nCreating local address {localAddr}..." + Environment.NewLine;
                local = IPAddress.Parse(localAddr);
                DebugInfoBox.Text += $"\nCreating new TCPListener at {localAddr}, {port}..." + Environment.NewLine;
                server = new TcpListener(local, port);
                DebugInfoBox.Text += $"\nStart listening..." + Environment.NewLine;
                server.Start();
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
    }
}
