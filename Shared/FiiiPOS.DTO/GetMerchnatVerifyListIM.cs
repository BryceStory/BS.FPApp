namespace FiiiPOS.DTO
{
    public class GetMerchnatVerifyListIM
    {
        public string cellphone { get; set; }
        public int countryId { get; set; }
        public int? status { get; set; }
        public string orderByFiled { get; set; }
        public bool isDesc { get; set; }
        public int pageSize { get; set; }
        public int index { get; set; }
    }
}
