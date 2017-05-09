﻿using System;
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
        private IPHostEntry ipHost { get; set; }                               // -- Server host ip
        private IPAddress ipAddr { get; set; }                                 // -- Server ip adress
        private IPEndPoint ipEndPoint { get; set; }                            // -- Server ip endpoint
        private int port { get; set; }                                         // -- Server port
        SocketPermission permission;                                           // -- Server permission
        Socket sListener;                                                      // -- Server listener
        Socket handler;                                                        // -- Server handler
        public Action<String, Socket> callBackAfterConnexion { get; set; }     // -- Callback called when connexion happens
        public Action<String, String> callBackAfterAnalysis { get; set; }      // -- Callback called when a message income
        public int nbDevices = 10;                                             // -- Max device 
        public static CommunicationService uniqueInstance;

        public static CommunicationService getInstance()
        {
            if (uniqueInstance == null)
            {
               
                if (uniqueInstance == null)
                {
                    uniqueInstance = new CommunicationService();
                }
                
            }
            return uniqueInstance;
        }



        // --  
        // -- Constructor 
        // --
        public CommunicationService()
        {
            try
            {
                // Creates one SocketPermission object for access restrictions
                permission = new SocketPermission(
                NetworkAccess.Accept,     // Allowed to accept connections 
                TransportType.Tcp,        // Defines transport types 
                "",                       // The IP addresses of local host 
                SocketPermission.AllPorts // Specifies all ports 
                );

                // Listening Socket object 
                sListener = null;

                // Ensures the code to have permission to access a Socket 
                permission.Demand();

                // Resolves a host name to an IPHostEntry instance 
                IPHostEntry ipHost = Dns.GetHostEntry("");

                // Gets first IP address associated with a localhost 
                this.ipAddr = ipHost.AddressList[1];

                // Sets port
                this.port = 4510;

                // Creates a network endpoint 
                ipEndPoint = new IPEndPoint(this.ipAddr, port);

                // Create one Socket object to listen the incoming connection 
                sListener = new Socket(
                    this.ipAddr.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                    );

                // Associates a Socket with a local endpoint 
                sListener.Bind(ipEndPoint);

            }
            catch (Exception exc) { Console.WriteLine("Communicationservice : "+exc); }


            try
            {
                // Places a Socket in a listening state and specifies the maximum 
                // Length of the pending connections queue 
                sListener.Listen(10);

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                sListener.BeginAccept(aCallback, sListener);

            }
            catch (Exception exc) { Console.WriteLine("Communicationservice listen : " + exc); }


        }

        // -- 
        // -- Close server
        // --
        public void disconnect()
        {
            sListener.Shutdown(SocketShutdown.Both);
            sListener.Close();
        }

        // -- 
        // -- Accept connexion callback - Happens when a connexion is instanciated
        // --
        private void AcceptCallback(IAsyncResult ar)
        {
            
            // A new Socket to handle remote host communication 
            Socket handler = null;
            Socket listener = null;

            try
            {
                // Receiving byte array 
                byte[] buffer = new byte[1024];
                // Get Listening Socket object 
                listener = (Socket)ar.AsyncState;
                // Create a new socket 
                handler = listener.EndAccept(ar);

                var sIp = (handler.RemoteEndPoint.ToString().Split(':'))[0];    // -- 
                IPAddress rIp = IPAddress.Parse(sIp);                           // -- Get & parse client IP
                string clientIp = rIp.ToString();

                // Using the Nagle algorithm 
                handler.NoDelay = false;

                // Creates one object array for passing data 
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                // Begins to asynchronously receive data 
                handler.BeginReceive(
                    buffer,        // An array of type Byt for received data 
                    0,             // The zero-based position in the buffer  
                    buffer.Length, // The number of bytes to receive 
                    SocketFlags.None,// Specifies send and receive behaviors 
                    new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate 
                    obj            // Specifies infomation for receive operation 
                    );

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(aCallback, listener);

                if (callBackAfterConnexion != null) 
                {
                    callBackAfterConnexion(clientIp, handler);                  // -- Callback if a connexion is set
                }

            }
            catch (Exception exc) { Console.WriteLine("Acceptcallback : " + exc); }


          

        }

        private void ReceiveCallback(IAsyncResult ar) 
        {
            String str = "";
            string clientIp = "";
            try
            {
                // Fetch a user-defined object that contains information 
                object[] obj = new object[2];
                obj = (object[])ar.AsyncState;

                // Received byte array 
                byte[] buffer = (byte[])obj[0];

                // A Socket to handle remote host communication. 
                handler = (Socket)obj[1];

                // Received message 
                string content = string.Empty;

                // -- Client ip
                var sIp = (handler.RemoteEndPoint.ToString().Split(':'))[0];    // -- 
                IPAddress rIp = IPAddress.Parse(sIp);                           // -- Get & parse client IP
                clientIp = rIp.ToString();

                // The number of bytes received. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content += Encoding.UTF8.GetString(buffer, 0,
                        bytesRead);

                    // If message contains "<Client Quit>", finish receiving
                    if (content.IndexOf("<Client Quit>") > -1)
                    {
                        // Convert byte array to string
                        str = content.Substring(0, content.LastIndexOf("<Client Quit>"));
                           
                    
                    }
                    else
                    {
                        // Continues to asynchronously receive data
                        byte[] buffernew = new byte[1024];
                        obj[0] = buffernew;
                        obj[1] = handler;
                        handler.BeginReceive(buffernew, 0, buffernew.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), obj);
                    }

                
                }
            }
            catch (Exception exc) { Console.WriteLine("Receivecallback : " + exc); }

            Console.WriteLine("STR : " + str); 
            if (callBackAfterAnalysis != null)
            {
                callBackAfterAnalysis(clientIp, str);   // -- Launch callback 
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
