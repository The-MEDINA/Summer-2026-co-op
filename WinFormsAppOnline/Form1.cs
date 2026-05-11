using System.Net;

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
            IP.Text = $"{hostName}: IP Address is {myIP}";
            // Console.WriteLine("My IP Address is :" + myIP);
            // Console.ReadKey();
        }
    }
}
