using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiiiPay.Data;
using FiiiPay.DTO;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;

namespace FiiiPay.Business.FiiiPay
{
    public class UserDeviceComponent: BaseComponent
    {

        public void DeleteDevice(UserDeviceDeleteIM im)
        {
            new UserDeviceDAC().Delete(im.Id);
        }

        public void UpdateDeviceInfo(Guid accountId, UserDeviceUpdateIM im, string ip, string deviceNumber)
        {
            var deviceList = new UserDeviceDAC().GetUserDeviceByAccountId(accountId);

            if (!deviceList.Any())
            {
                new ApplicationException();
            }

            if (deviceList.All(item => item.DeviceNumber != deviceNumber))
            {
                new ApplicationException();
            }

            new UserDeviceDAC().Update(new UserDevice(){UserAccountId = accountId, Address = im.Address,IP = ip,LastActiveTime = DateTime.UtcNow,Name = im.Name,DeviceNumber = im.DeviceNumber});
        }

        public List<UserDeviceItemOM> GetDeviceList(UserAccount account)
        {
            return new UserDeviceDAC().GetUserDeviceByAccountId(account.Id).Select(item => new UserDeviceItemOM()
            {
                DeviceNumber = item.DeviceNumber,
                IP = item.IP,
                LastActiveTime = item.LastActiveTime.ToUnixTime().ToString(),
                Name = item.Name,
                Address = item.Address,
                Id = item.Id
                
            }).ToList();
        }
    }
}
