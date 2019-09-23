using System;

namespace FiiiPay.DTO.Invite
{
    public class InviteRankOM
    {
        public string PhoneCode { get; set; }

        public string _cellPhone;
         public string CellPhone
        {
            get => _cellPhone;
            set
            {
                _cellPhone = value;
                if (_cellPhone.Length > 4)
                {
                    _cellPhone = $"*******{_cellPhone.Substring(_cellPhone.Length - 4)}";
                }
                else
                {
                    _cellPhone = $"*******{_cellPhone}";
                }
            }
        }
        public Guid AccountId { get; set; }
        public string CryptoAmount { get; set; }
    }
}
