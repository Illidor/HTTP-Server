using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace HTTP_Server
{
	public class Response
	{
        private const string HTML_MIME = "text/html";
        private Byte[] data = null;
		private String status;
		private String mime;

		private Response(String status, String mime, Byte[] data)
		{
			this.status = status;
			this.data = data;
			this.mime = mime;
			
		}

		public static Response From(Request request)
		{
			if (request == null)
				return MakeNullRequest();

            if (request.Type == "GET")
            {
                String file = Environment.CurrentDirectory + HTTP_Server.WEB_DIR + request.URL;
                FileInfo f = new FileInfo(file);
                if (f.Exists && f.Extension.Contains("."))
                {
                    return MakeFromFile(f);
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(f + "/");
                    if (!di.Exists)
                        return MakePageNotFound();
                    FileInfo[] files = di.GetFiles();
                    foreach (FileInfo ff in files)
                    {
                        String n = ff.Name;
                        if (n.Contains("default.html") || n.Contains("default.htm") || n.Contains("index.htm") || n.Contains("index.html"))
                        return MakeFromFile(ff);
                    }
                }
            }
            else
                MakeMethodNotAllowed();


            return MakePageNotFound();
		}

        private static Response MakeFromFile(FileInfo f)
        {

            ///Wird bereits abgefragt
            //String file = Environment.CurrentDirectory + HTTP_Server.WEB_DIR + "200.html";
            //FileInfo fi = new FileInfo(file);


            FileStream fs = f.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("200 OK", HTML_MIME, d);
            //fs.Close();
            //return new Response("200 OK", "text/html", d);
            
        }

        private static Response MakeNullRequest()
		{
            String file = Environment.CurrentDirectory + HTTP_Server.MSG_DIR + "400.html";
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("400 Bad Request", HTML_MIME, d);
		}

        private static Response MakeMethodNotAllowed()
        {
            String file = Environment.CurrentDirectory + HTTP_Server.MSG_DIR + "405.html";
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("405 Method Not Allowed", HTML_MIME, d);
        }

        private static Response MakePageNotFound()
        {
            String file = Environment.CurrentDirectory + HTTP_Server.MSG_DIR + "404.html";
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("404 Page Not Found", HTML_MIME, d);
        }

        public void Post(NetworkStream stream)
		{
			StreamWriter writer = new StreamWriter(stream);
            string response = String.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Lenght: {4}\r\n",
                HTTP_Server.VERSION, status, HTTP_Server.NAME, mime, data.Length);

            Console.WriteLine(response);
            writer.WriteLine(response);

            //Alternative zur oberen Zeile?
            //writer.Flush(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Lenght: {4}\r\n",
            //HTTP_Server.VERSION, status, HTTP_Server.NAME, mime, data.Length));

            stream.Write(data, 0, data.Length);
		}


        //Alternative?
        //public void Post(NetworkStream stream)
        //{
        //    StringBuilder sbHeader = new StringBuilder();
        //    sbHeader.AppendLine(HTTPServer.VERSION + " " + status);
        //    // CONTENT-LENGTH sbHeader.AppendLine("Content-Length: " + data.Length);
        //    // Append one more line breaks to seperate header and content. sbHeader.AppendLine();
        //    List<byte> response = new List<byte>();
        //    // response.AddRange(bHeadersString);
        //    response.AddRange(Encoding.ASCII.GetBytes(sbHeader.ToString()));
        //    response.AddRange(data); byte[] responseByte = response.ToArray();
        //    stream.Write(responseByte, 0, responseByte.Length);
        //}﻿

    }
}
