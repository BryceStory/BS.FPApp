using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO
{
    /// <summary>
    /// 用于查询对象详情的泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GetDetailByIdIM<T>
    {
        /// <summary>
        /// 对象的Id
        /// </summary>
        [Required]
        public T Id { get; set; }
    }
}
