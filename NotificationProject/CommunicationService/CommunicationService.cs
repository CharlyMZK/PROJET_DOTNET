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
        private IPHostEntry ipHost { get; set; }
        private IPAddress ipAddr { get; set; }
        private int port { get; set; }
        private IPEndPoint ipEndPoint;
        private byte[] buffer;
        private int bytesRec;
        private string theMessageToReceive;
        private string response;
        public Action<String,Socket> callBackAfterConnexion { get; set; }
        public Action<String,String> callBackAfterAnalysis { get; set; } 

        public CommunicationService()
        {
            this.port = 4510;
            this.ipHost = Dns.GetHostEntry("");
            this.ipAddr = ipHost.AddressList[2];
            this.ipEndPoint = new IPEndPoint(ipAddr, this.port);
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
            

            Socket listener = (Socket)result.AsyncState;
            Socket handler = listener.EndAccept(result);
            Object[] obj = new Object[2]; 

            obj[0] = buffer;
            obj[1] = handler;

            var sIp = (handler.RemoteEndPoint.ToString().Split(':'))[0];
            IPAddress rIp = IPAddress.Parse(sIp);
            string clientIp = rIp.ToString();
             
            Console.WriteLine("Something is coming ! - " +clientIp);

           



            handler.BeginReceive(
                buffer, 
                0,
                buffer.Length,
                SocketFlags.None, 
                new AsyncCallback(ReceiveCallback),
                obj 
            );

  
            if (callBackAfterConnexion != null)
            {
                callBackAfterConnexion(clientIp, handler);
            }
            
        }
         
        private void ReceiveCallback(IAsyncResult ar) 
        { 
            int i;
            
            try
            {
                // Get reply from the server.
                i = sServer.Receive(buffer); 
                /*Console.WriteLine(Encoding.UTF8.GetString(buffer));
                Console.WriteLine("RECEIVE BUFFER : " + System.Text.Encoding.Unicode.GetString(buffer));*/
            }
            catch (SocketException e) 
            {
                Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
               
            }

            Console.WriteLine("------ >" + System.Text.Encoding.Unicode.GetString(buffer));


            if (callBackAfterAnalysis != null)
            {
                callBackAfterAnalysis("Device1", System.Text.Encoding.Unicode.GetString(buffer)); 
            }

            Console.WriteLine("OMG UN RESULTAT, VITE CONVERTIR DE BYTE EN STRING !!! - " + bytesRec);
        } 


    } 
}
