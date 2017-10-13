using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataAccess.Model;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace DataAccess
{
    public class XmlAccess
    {
        private XDocument doc;
        private string path;

        public XmlAccess(string path)
        {
            this.path = path;
            doc = XDocument.Load(path);
        }

        //Save the Device device in the xml file
        public void saveDevice(Device device)
        {
            //Créer un nouveau client
            doc.Root.Add(new XElement("Device",
                        new XAttribute("Name", device.Name)
                    ));
            doc.Save(path);
        }
        //Save several devices
        public void saveDevices(IEnumerable<Device> devices)
        {
            foreach (var device in devices)
            {
                saveDevice(device);
            }
            doc.Save("path");
        }
        //return the devices
        public List<Device> getDevices()
        {
            List<Device> devices = new List<Device>();
            devices.AddRange(doc.Root.Descendants("Device")
                .Select(device => new Device()
                {
                    Name = device.Element("Name").Value
                }
            ));
            return devices;
        }
        //remove device
        public void removeDevice(Device device)
        {
            doc.Root.Descendants("Device").Where(d => d.Element("Name").Value == device.Name).Remove();
            doc.Save("path");
        }
        //Save the Contact contact in the xml file
        public void saveContact(Contact contact)
        {
            doc.Root.Add(new XElement("Contact",
                new XAttribute("Name", contact.Name),
                new XAttribute("Number", contact.Number),
                new XAttribute("Email", contact.Email)
                ));
            doc.Save("path");
        }
        //Save several contact
        public void saveContacts(IEnumerable<Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                saveContact(contact);
            }
            doc.Save("path");
        }

        public List<Contact> getContacts()
        {
            var contacts = new List<Contact>();
            contacts.AddRange(doc.Root.Descendants("Contact")
                .Select(contact =>
                    new Contact(contact.Element("Name").Value, contact.Element("Number").Value, contact.Element("Email").Value)
                ));
            return contacts;
        }
        public void removeContact(Contact contact)
        {
            doc.Root.Descendants("Contact")
                .Where(d => d.Element("Name").Value == contact.Name &&
                d.Element("Email").Value == contact.Email &&
                d.Element("Number").Value == contact.Number)
                .Remove();
            doc.Save("path");
        }

        public static void parseConfiguration()
        {

            


            Console.WriteLine("Parsing XML");

            StringBuilder op = new StringBuilder();

            string path = @"C:\Users\MZK\Documents\Visual Studio 2015\Projects\PROJET_DOTNET\NotificationProject\DataAccess\Configuration\notificationConfiguration.xml";
            string readText = File.ReadAllText(path);
            // Create an XmlReader
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode isEnabledNode = doc.DocumentElement.SelectSingleNode("/notificationConfiguration/isEnabled");
            XmlNode smsEnabledNode = doc.DocumentElement.SelectSingleNode("/notificationConfiguration/smsEnabled");
            XmlNode callEnabledNode = doc.DocumentElement.SelectSingleNode("/notificationConfiguration/callEnabled");
            XmlNode otherEnabledNode = doc.DocumentElement.SelectSingleNode("/notificationConfiguration/otherEnabled");
            

            bool configIsEnabled = bool.Parse(isEnabledNode.InnerText);
            bool configSmsEnabled = bool.Parse(smsEnabledNode.InnerText);
            bool configCallEnabled = bool.Parse(callEnabledNode.InnerText);
            bool configOtherEnabled = bool.Parse(otherEnabledNode.InnerText);

            NotificationConfiguration.getInstance().IsEnabled = configIsEnabled;
            NotificationConfiguration.getInstance().SmsEnabled = configSmsEnabled;
            NotificationConfiguration.getInstance().CallEnabled = configCallEnabled;
            NotificationConfiguration.getInstance().OtherEnabled = configOtherEnabled;
     
        }

    }
}
