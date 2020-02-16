using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

using System.Threading;

namespace Server
{
    public partial class Form1 : Form
    {
       

        public Form1()
        {
            InitializeComponent();
            label2.Text = "Server Initialize...";
        }




        private void label1_Click(object sender, EventArgs e)
        {

        }

        // Thread signal.
        private ManualResetEvent allDone = new ManualResetEvent(false);
        private Socket listener;
        private bool m_Listening;


        private void button1_Click(object sender, EventArgs e) //Start Server
        {
            label2.Text = "Server Running...";

            ThreadStart myThreadDelegate = new ThreadStart(Listen);
            Thread myThread = new Thread(myThreadDelegate);
            myThread.Start();

        }

        private void Listen()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 5222);

            label2.Text = "Listen at: localhost:5222";
            // Create a TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                m_Listening = true;

                while (m_Listening)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    label2.Text = "Waiting for a connection...";
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), null);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();
            // Get the socket that handles the client request.
            Socket newSock = listener.EndAccept(ar);

            agsXMPP.XmppSeverConnection con = new agsXMPP.XmppSeverConnection(newSock);
            //listener.BeginReceive(buffer, 0, BUFFERSIZE, 0, new AsyncCallback(ReadCallback), null);
        }

        private void button2_Click(object sender, EventArgs e)  //Stop Server
        {
            label2.Text = "Server Stopped...";
            m_Listening = false;
            allDone.Set();
            //allDone.Reset();
        }



    } // Ende Form 1


}  //Ende
