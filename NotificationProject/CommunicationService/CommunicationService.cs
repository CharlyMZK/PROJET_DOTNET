using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Usings for Sockets
using System.Net;
using System.Net.Sockets;

namespace BusinessLayer
{
    public class CommunicationService
    {
        private Socket sServer;                                                // -- Server
        private IPHostEntry ipHost { get; set; }                               // -- Server host ip
        private IPAddress ipAddr { get; set; }                                 // -- Server ip adress
        private IPEndPoint ipEndPoint { get; set; }                            // -- Server ip endpoint
        private int port { get; set; }                                         // -- Server port
        private byte[] buffer;                                                 // -- Buffer who contains
        public Action<String, Socket> callBackAfterConnexion { get; set; }      // -- Callback called when connexion happens
        public Action<String, String> callBackAfterAnalysis { get; set; }       // -- Callback called when a message income


        // -- 
        // -- Constructor
        // --
        public CommunicationService()
        {
            this.port = 4510;                                               // -- Server port

            this.ipHost = Dns.GetHostEntry("");                             // -- 
            this.ipAddr = ipHost.AddressList[2];                            // -- Server IP configuration
            this.ipEndPoint = new IPEndPoint(ipAddr, this.port);            // -- 

            this.buffer = new byte[1024];                                   // -- Buffer containing bits

            this.sServer = new Socket(                                      // -- 
                this.ipAddr.AddressFamily,                                  // -- Server instanciation
                SocketType.Stream,                                          // --
                ProtocolType.Tcp                                            // --
            );
            sServer.Bind(ipEndPoint);                                       // -- Server bind
            sServer.Listen(1);                                              // -- Server listen

            AsyncCallback aCallback = new AsyncCallback(acceptCallback);    // -- Launch callback
            sServer.BeginAccept(aCallback, sServer);                        // -- Launch accepts


        }

        // -- 
        // -- Close server
        // --
        public void disconnect()
        {
            sServer.Shutdown(SocketShutdown.Both);
            sServer.Close();

        }

        // -- 
        // -- Accept connexion callback - Happens when a connexion is instanciated
        // --
        private void acceptCallback(IAsyncResult result)
        {
            Socket listener = (Socket)result.AsyncState;                    // -- Client listener
            Socket handler = listener.EndAccept(result);                    // -- Client handler

            Object[] obj = new Object[2];                                   // --
            obj[0] = buffer;                                                // -- Listener & Handler container
            obj[1] = handler;                                               // --

            var sIp = (handler.RemoteEndPoint.ToString().Split(':'))[0];    // -- 
            IPAddress rIp = IPAddress.Parse(sIp);                           // -- Get & parse client IP
            string clientIp = rIp.ToString();                               // --

            handler.BeginReceive(                                           // -- 
                buffer,                                                     // --  Handler begin receive
                0,                                                          // --  ReceiveCallback will be triggered if a message arrive
                buffer.Length,                                              // -- 
                SocketFlags.None,                                           // -- 
                new AsyncCallback(ReceiveCallback),                         // -- 
                obj                                                         // -- 
            );


            if (callBackAfterConnexion != null)                             
            {           
                callBackAfterConnexion(clientIp, handler);                  // -- Callback if a connexion is set
            }

        }

        private void ReceiveCallback(IAsyncResult ar) 
        { 
            int i;
            
            try
            {

                i = sServer.Receive(buffer);    // -- Receive message from client
            }
            catch (SocketException e) 
            {
                Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
               
            }

            if (callBackAfterAnalysis != null)
            { 
                callBackAfterAnalysis("Device1", System.Text.Encoding.Unicode.GetString(buffer));   // -- Launch callback 
            }

        }


        #region getter
        public IPAddress getIpAddress()
        {
            return ipAddr;
        }

        public int getPort()
        {
            return port;
        }
        #endregion

    } 
}
