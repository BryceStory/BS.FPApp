using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class ModifyNameModel
    {
        /// <summary>
        /// Address1
        /// </summary>
        [StringLength(100, MinimumLength = 1)]
        public string FirstName { get; set; }
        /// <summary>
        /// Address1
        /// </summary>
        [StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; }
    }
}