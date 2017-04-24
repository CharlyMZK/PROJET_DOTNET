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
        private int bytesRec;
        private string theMessageToReceive;
        private string response;
        public Action<String> callBackAfterConnexion { get; set; }
        public Action<String> callBackAfterAnalysis { get; set; } 

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

        public void disconnect()
        {
            sServer.Shutdown(SocketShutdown.Both);
            sServer.Close();

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
                new AsyncCallback(ReceiveCallback),
                obj
            );
            if(callBackAfterConnexion != null)
            {
                callBackAfterConnexion("acceptCallback");
            }
            
        }

        private void ReceiveCallback(IAsyncResult ar) 
        {
            int i;
            byte[] bytes = new byte[256];
            try
            {
                
                // Get reply from the server.
                i = sServer.Receive(bytes);
                Console.WriteLine(Encoding.UTF8.GetString(bytes));
            }
            catch (SocketException e)
            {
                Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
               
            }


            Console.WriteLine("OMG UN RESULTAT, VITE CONVERTIR DE BYTE EN STRING !!! - " + bytesRec);
        } 


    } 
}
