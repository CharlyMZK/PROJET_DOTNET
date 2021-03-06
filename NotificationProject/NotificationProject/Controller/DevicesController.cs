﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DataAccess.Model;

namespace NotificationProjet.Controller
{
    public class DevicesController
    {
        public List<Device> Devices { get; set; }
        private static DevicesController _instance;

        public static DevicesController getInstance()
        {
            if (_instance == null)
                _instance = new DevicesController();
            return _instance;
        }
        private DevicesController()
        {
            Devices = new List<Device>();
        }

        public Device getDevice(string name)
        {
            foreach(Device dev in Devices)
            {
                if(name == dev.Name)
                {
                    return dev;
                }
            }
            return null;
        }

        public void addDevice(Device device)
        {
            //    Devices = new List<Device>(Devices);  // -- TODO : Its supposed to work without this line, but it throw an exception.
            Devices.Add(device);                                      // -- Add device on list
        }

        public void deleteDevice(Device device)
        {
            Devices.Remove(device); // -- Remove device on list
        }
    }



}
