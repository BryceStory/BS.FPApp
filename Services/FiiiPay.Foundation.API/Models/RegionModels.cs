using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.Foundation.API.Models
{
    public class GetStateListModel
    {
        /// <summary>
        /// 国家Id
        /// </summary>
        [Required]
        public int CountryId { get; set; }
    }

    public class GetRegionListModel
    {
        /// <summary>
        /// 上级区域Id
        /// </summary>
        [Required]
        public long ParentId { get; set; }
    }

    public class RegionListOM
    {
        /// <summary>
        /// 区域Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 地区编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string Name_CN { get; set; }
    }
}