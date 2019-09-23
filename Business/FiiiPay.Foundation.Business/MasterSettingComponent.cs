using System.Collections.Generic;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Business
{
    public class MasterSettingComponent
    {
        public List<MasterSetting> GetSettingByGroupName(string groupName)
        {
            return new MasterSettingDAC().SelectByGroup(groupName);
        }
    }
}