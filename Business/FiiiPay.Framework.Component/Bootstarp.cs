//using Dapper;
using log4net.Config;

namespace FiiiPay.Framework.Component
{
    public class Bootstarp
    {
        public static void Start()
        {
            //SqlMapper.AddTypeHandler(new DapperDateTimeHandler());
            
            XmlConfigurator.Configure();
        }
    }
}
