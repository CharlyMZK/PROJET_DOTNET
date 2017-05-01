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

        public DevicesController()
        {
            Devices = new ObservableCollection<Device>();
        }

        public void addDevice(Device device)
        {
            Devices = new ObservableCollection<Device>(Devices);  // -- TODO : Its supposed to work without this line, but it throw an exception.
            Devices.Add(device);                                      // -- Add device on list
        }
    }



}
