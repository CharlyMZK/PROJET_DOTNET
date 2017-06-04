using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DataAccess.Model
{
    public class DevicesController
    {
        public ObservableCollection<Device> Devices { get; set; }
        private static DevicesController _instance;

        public static DevicesController getInstance()
        {
            if (_instance == null)
                _instance = new DevicesController();
            return _instance;
        }
        private DevicesController()
        {
            Devices = new ObservableCollection<Device>();
        }

        public void addDevice(Device device)
        {
            Devices = new ObservableCollection<Device>(Devices);  // -- TODO : Its supposed to work without this line, but it throw an exception.
            Devices.Add(device);                                      // -- Add device on list
        }

        public void deleteDevice(Device device)
        {
            Devices.Remove(device); // -- Remove device on list
        } 
    }



}
