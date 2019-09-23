using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Foundation.API.Models
{
    public class GetCountryByIdModel
    {
        [Required]
        public int Id { get; set; }
    }
}