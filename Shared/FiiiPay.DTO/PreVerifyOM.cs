using FiiiPay.Entities.Enums;

namespace FiiiPay.DTO.Profile
{
    public class PreVerifyOM
    {
        public VerifyStatus Lv1Status { get; set; }

        public VerifyStatus Lv2Status { get; set; }
    }
}
