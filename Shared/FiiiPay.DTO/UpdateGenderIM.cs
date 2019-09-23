using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Profile
{
    public class UpdateGenderIM
    {
        /// <summary>
        /// 1：男，0：女
        /// </summary>
        [Range(0, 1)]
        public byte Gender { get; set; }
    }
}
