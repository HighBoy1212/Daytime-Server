using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace Daytime_Server
{
    public partial class Form1 : Form
    {
        // Field to hold socket thatwill listen for connection requests
        private Socket socListen = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            socListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaServer = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipeServer = new IPEndPoint(ipaServer, 33221);
            socListen.Bind(ipeServer);
            socListen.Listen(5);
            Task.Run(vAcceptRequests);

        }

        private void vAcceptRequests()
        {
            while (true)
            {
                try
                {
                    Socket socConnection = socListen.Accept();
                    Task.Run(() => vProcessConnection(socConnection));
                }
                catch
                {
                    return;
                }
            }
        }

        private void vProcessConnection(Socket socConnection)
        {
            byte[] byDaytime = Encoding.ASCII.GetBytes(DateTime.Now.ToString("F"));
            socConnection.Send(byDaytime, byDaytime.Length, SocketFlags.None);
            socConnection.Shutdown(SocketShutdown.Both);
            socConnection.Close();
            socConnection.Dispose();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            socListen.Close();
            socListen.Dispose();
            socListen = null;
        }
    }
}
