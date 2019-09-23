using FiiiPay.Framework.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    public class GetByIdModel<T> where T:struct
    {
        [Range(0,int.MaxValue)]
        public T Id { get; set; }
    }
    public class GetByGuidModel
    {
        [RequiredGuid]
        public Guid Id { get; set; }
    }
    public class GetByStringModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string Id { get; set; }
    }
    public class GetByCodeModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string Code { get; set; }
    }
}