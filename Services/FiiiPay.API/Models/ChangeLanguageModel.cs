﻿using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// Class ChangeLanguageModel
    /// </summary>
    public class ChangeLanguageModel
    {
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        [Required]
        public string Language { get; set; }
    }
}