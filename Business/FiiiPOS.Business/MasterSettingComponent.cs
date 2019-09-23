using System.Collections.Generic;
using FiiiPay.Data;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
namespace FiiiPOS.Business
{
    public class MasterSettingComponent
    {
        public List<MasterSetting> GetSettingByGroupName(string groupName)
        {
            return new MasterSettingDAC().SelectByGroup(groupName);
        }
    }
}