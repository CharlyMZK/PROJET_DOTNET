using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Usings for Sockets
using System.Net;
using System.Net.Sockets;

namespace CommunicationService
{
    public class CommunicationService
    {
        private Socket sServer;
        private IPHostEntry ipHost;
        private IPAddress ipAddr;
        private IPEndPoint ipEndPoint;
        private byte[] buffer;

        public CommunicationService()
        {
            this.ipHost = Dns.GetHostEntry("");
            this.ipAddr = ipHost.AddressList[2];
            this.ipEndPoint = new IPEndPoint(ipAddr, 4510);
            this.buffer = new byte[1024];

            this.sServer = new Socket(
                this.ipAddr.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            sServer.Bind(ipEndPoint);
            sServer.Listen(1);

            AsyncCallback aCallback = new AsyncCallback(acceptCallback);
            sServer.BeginAccept(aCallback, sServer);
        }


        private void acceptCallback(IAsyncResult result)
        {
            Console.WriteLine("Something is coming !");
            Socket listener = (Socket)result.AsyncState;
            Socket handler = listener.EndAccept(result);

            byte[] buffer = new byte[1024];
            Object[] obj = new Object[2];
            obj[0] = buffer;
            obj[1] = handler;

            handler.BeginReceive(
                buffer,
                0,
                buffer.Length,
                SocketFlags.None,
                new AsyncCallback(receiveCallback),
                obj
            );


        }

        private void receiveCallback(IAsyncResult result) 
        {
            String message = result.ToString();
             
            Console.WriteLine("OMG UN RESULTAT, VITE CONVERTIR DE BYTE EN STRING !!! - "+message);
        }
    }
}
