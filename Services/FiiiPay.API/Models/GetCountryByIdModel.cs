using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
{
    public class GetCountryByIdModel
    {
        [Required]
        public int Id { get; set; }
    }
}