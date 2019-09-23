using System;
using System.Data;
using Dapper;

namespace FiiiPay.Framework.Component
{
    internal class DapperDateTimeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public override DateTime Parse(object value)
        {
            return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
        }
    }
}
