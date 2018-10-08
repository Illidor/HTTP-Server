using System;

namespace HTTP_Server
{
	class Program
	{
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server on port 8080!");
            HTTP_Server server = new HTTP_Server(8080);
            server.Start();
                       
        }
	}
}
