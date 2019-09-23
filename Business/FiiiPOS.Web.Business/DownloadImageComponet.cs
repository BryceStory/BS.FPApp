using FiiiPay.Foundation.Business.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPOS.Web.Business
{
   public class DownloadImageComponent
    {
        public byte[] Download(Guid fileId)
        {
            return new MasterImageAgent().Download(fileId.ToString());
        }
    }


}
