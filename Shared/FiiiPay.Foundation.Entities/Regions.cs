using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Foundation.Entities
{
    public class Regions
    {
        public long Id { get; set; }
        public RegionStatus Status { get; set; }
        public int CountryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameCN { get; set; }
        public long ParentId { get; set; }
        public RegionLevel RegionLevel { get; set; }
        public int Sort { get; set; }
    }

    public enum RegionLevel : byte
    {
        Country = 1,
        State = 2,
        City = 3,
        Region = 4
    }
    public enum RegionStatus
    {
        Disable = 0,
        Enable = 1
    }
}
