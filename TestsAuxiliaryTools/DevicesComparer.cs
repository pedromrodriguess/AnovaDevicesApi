using DevicesApi.Contracts.Requests;
using DevicesApi.Domain;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;

namespace DevicesApi.TestsAuxiliaryTools
{
    public static class DevicesComparer
    {
        public static bool CompareDevicesLists(List<Device> devicesList1, List<Device> devicesList2)
        {
            if (devicesList1 == null || devicesList2 == null)
                return false;

            if (devicesList1.Count != devicesList2.Count)
                return false;

            for (int i = 0; i < devicesList1.Count; i++)
            {
                if (!CompareDevices(devicesList1.ElementAt(i), devicesList2.ElementAt(i)))
                    return false;
            }
            return true;
        }

        public static bool DoesDevicesListContainSpecificDevice(List<Device> devicesList, CreateDeviceRequest deviceRequest)
        {
            if (devicesList == null)
                return false;

            return devicesList.Any(device => CompareDeviceWithDeviceRequest(device, deviceRequest));
        }

        public static bool CompareDevices(Device device1, Device device2)
        {
            if((device1.Device_id != device2.Device_id
                || !device1.Name.Equals(device2.Name)
                || !device1.Location.Equals(device2.Location)))
            {
                return false;
            }
            return true;
        }

        public static bool CompareDeviceWithDeviceRequest(Device device, CreateDeviceRequest deviceRequest)
        {
            if ((!device.Name.Equals(deviceRequest.Name)
                || !device.Location.Equals(deviceRequest.Location)))
            {
                return false;
            }
            return true;
        }

        public static bool CompareDeviceWithDeviceRequest(Device device, UpdateDeviceRequest deviceRequest)
        {
            if ((!device.Name.Equals(deviceRequest.Name)
                || !device.Location.Equals(deviceRequest.Location)))
            {
                return false;
            }
            return true;
        }
    }
}
