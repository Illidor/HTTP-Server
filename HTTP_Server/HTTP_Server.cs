using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HTTP_Server
{
	public class HTTP_Server
	{
        public const String MSG_DIR = "/root/msg/";
        public const String WEB_DIR = "/root/web/";
        public const String VERSION = "HTTP/1.0";
		public const String NAME = "HTTP Server v1.0";

        private bool running = false;

		private TcpListener listener;

		public HTTP_Server(int port)
		{
			listener = new TcpListener(IPAddress.Any, port);
            Console.WriteLine("Listener startet!");
		}

		public void Start()
		{
            Run();

            ///old, no multithreading
			//Thread serverThread = new Thread(new ThreadStart(Run));
            //serverThread.Start();
		}

		private void Run()
		{
			running = true;
			listener.Start();
            
            while (running)
			{

                Console.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now));

                Console.WriteLine("Waiting for connetion...");
				
				TcpClient client = listener.AcceptTcpClient();

				Console.WriteLine("Client connected!");

                ///whithout multithreading
                //HandleClient(client);

                ///old
                //Thread serverThread = new Thread(new ThreadStart(() => HandleClient(client))); //ParameterizedThreadStart()

                ///For Clientbased Multithreading
                Thread serverThread = new Thread(() => HandleClient(client));
                //Thread serverThread = new Thread(new ParameterizedThreadStart(HandleClient));

                //Debug.WriteLine("Creating Thread");

                //Console.WriteLine("Creating Thread");

                serverThread.Start();

                //Debug.WriteLine("Starting Thread");
                
                //Console.WriteLine("Starting Thread");

                ///Throws Errors with multithreading
                

			}

			running = false;

			listener.Stop();
		}

		public void HandleClient(TcpClient client)
		{
			StreamReader reader = new StreamReader(client.GetStream());

			String msg = "";
			while (reader.Peek() != -1)
			{
				msg += reader.ReadLine() + "\n";
			}

			Debug.WriteLine("Request: \n" + msg);
            Console.WriteLine("Request: \n" + msg);

            Request req = Request.GetRequest(msg);
			Response resp = Response.From(req);
			resp.Post(client.GetStream());

            client.Close();
        }
	}
}