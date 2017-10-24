using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NotificationProject.HelperClasses
{
    public class JSONHandler
    {
        public static JObject stringToJson(string chaine)
        {
            Console.WriteLine(chaine);
            JObject message;
            try
            {
                message = JObject.Parse(chaine);
            }
            catch (Exception ex)
            {
                message = new JObject();
            }
            return message;
        }

        public static string[] interpretation(JObject json)
        {
            string[] res = new string[3] { "", "", "" };
            string type = (string)json["type"];
            string conn = (string)(json["conn"]);
            if (!String.IsNullOrEmpty(type))
            {
                int port;
                string[] adressBuffer = conn.Split(':');
                string[] pairaineKey = adressBuffer[1].Split('@');
                //ip@port
                IPAddress ipAddress = IPAddress.Parse(adressBuffer[0]);
                port = Int32.Parse(pairaineKey[0]);
                string author = (string)json["author"];
                if (type.ToLower() == "connect") //1.Type 2.Author 3.IPaddress@Port
                {
                    res[0] = "Connection";
                    res[1] = author;
                    res[2] = ipAddress.ToString() + ":" + port.ToString() + ":" + pairaineKey[1];

                    Console.WriteLine("Demande Connection");
                    Console.WriteLine("L'appareil " + author + "(" + ipAddress.ToString() + ":" + port.ToString() + ") souhaite se connecter.");
                }
                else if (type.ToLower() == "disconnect") //1.Type 2.Author 3.IPaddress@Port
                {
                    res[0] = "Disconnection";
                    res[1] = author;
                    res[2] = ipAddress.ToString() + ":" + port.ToString();
                    Console.WriteLine("Demande Connection");
                    Console.WriteLine("L'appareil " + author + "(" + ipAddress.ToString() + ":" + port.ToString() + ") souhaite se deconnecter.");
                }

                else if (type.ToLower() == "notification") //1.Type 2.Application 3.Message
                {
                    res = new string[5] { "", "", "", "","" };
                    res[0] = "Notification";
                    Console.WriteLine("Notification");
                    IList<string> allObject = json["object"].Select(t => (string)t).ToList();
                    string application = allObject[0];
                    string date = allObject[1];
                    string message = allObject[2];
                    string title = allObject[3];
                    res[1] = application;
                    res[2] = message;
                    res[3] = title;
                    res[4] = date;
                    //Démonstration utilisation des objets obtenus depuis le JSON
                    Console.WriteLine("L'application " + application + " a reçu le message suivant: '" + message + "' depuis l'appareil de " + author + " à " + date + ".");
                }
                else if (type.ToLower() == "batterystate")
                {
                    res[0] = "batteryState";
                    string etat;
                    string pourcentage;
                    Console.WriteLine("batteryState");
                    IList<string> allObject = json["object"].Select(t => (string)t).ToList();
                    pourcentage = allObject[1];

                    if (allObject[0].ToLower() == "true")
                    {
                        etat = "isCharging";
                    }
                    else
                    {
                        etat = "notCharging";
                    }
                    res[1] = pourcentage;
                    res[2] = etat;
                    //Démonstration utilisation des objets obtenus depuis le JSON
                    Console.WriteLine("L'appareil " + author + " est a " + pourcentage + "%. Etat: " + etat);
                }
            }
            else
                Console.WriteLine("Message invalide.");
            return res;
        }

        public static JObject messageRetour(string case1, string case2, string case3)
        {
            JObject json;
            string res = "";
            if (case1.ToLower() == "connect" || case1.ToLower() == "disconnect")
            {
                res = @"{type: '" + case1 + "', conn: '" + case3 + "', author: '" + case2 + "', object: null}";
            }
            json = stringToJson(res);
            return json;
        }
        public static string sendState(string deviceName, string etat, string pourcentage)
        {
            string res = "";
            if (deviceName != "" && etat != "")
            {
                res = @"{type:'batteryState', author:'" + deviceName + "',conn:'" + deviceName + "',object:{ percent:'" + pourcentage + "',isCharging:'" + etat + "'}}";
            }
            return res;
        }

        public static JObject creationSMS(string author, string receiver, string appareil, string message)
        {
            DateTime dt = DateTime.Now;
            JObject json;
            string sms = @"{type: 'SMS', conn: '" + appareil + "',author: '" + author + "', receiver: '" + receiver + "',object: {application: 'SMS App',Message: '" + message + "',heureDate: '" + dt + "'}}";
            json = stringToJson(sms);
            return json;
        }

        public static string creationSMSString(string author, string appareil, string message, string number)
        {
            var dt = DateTime.Now;
            return "{\"type\": \"smsToSend\", \"conn\": \"" + appareil + "\",\"author\": \"" + author +
                "\",\"object\": {\"application\": \"com.google.android.apps.messaging\",\"message\": \"" + message + "\", \"numbers\": [\"" + number + "\"]}}" + "\n";
        }

        public static string creationAppelString(string author, string appareil, string number)
        {
            return "{\"type\": \"requestCall\", \"conn\": \"" + appareil + "\",\"author\": \"" + author +
                "\",\"object\": { \"number\": \"" + number + "\"}}" + "\n";
        }

        public static string creationDisconnectString(string appareil, string author)
        {
            return "{\"type\": \"disconnectionAcknowledged\", \"conn\": \"" + appareil + "\",\"author\": \"" + author +
                "\"}" + "\n";
        }

        public static string creationContactRequest(string appareil, string author)
        {
            return "{\"type\": \"requestContacts\", \"conn\": \"" + appareil + "\",\"author\": \"" + author +
                "\"}" + "\n";
        }

        public static string creationAcceptConnexionRequest(string appareil, string author)
        {
            return "{\"type\": \"connectionAccepted\", \"conn\": \"" + appareil + "\",\"author\": \"" + author +
              "\"}" + "\n";
        }

        public static string creationRefuseConnexionRequest(string appareil, string author)
        {
            return "{\"type\": \"connectionRefused\", \"conn\": \"" + appareil + "\",\"author\": \"" + author +
              "\"}" + "\n";
        }

    }
}
