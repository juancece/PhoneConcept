using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PhoneConcept.Models
{
    public class PhoneNumberCheckViewModel
    {
        private string _countryCodeSelected;

        [Required]
        [Display(Name = "Issuing Country")]
        public string CountryCodeSelected
        {
            get => _countryCodeSelected;
            set => _countryCodeSelected = value?.ToUpperInvariant();
        }

        [Required]
        [Display(Name = "Phone Number")]
        [MaxLength(18)]
        public string PhoneNumberRaw { get; set; }

        [Display(Name = "Valid Number")]
        public bool Valid { get; set; }

        [Display(Name = "Validated Type")]
        public string PhoneNumberType { get; set; }

        [Display(Name = "Region Code")]
        public string RegionCode { get; set; }

        [Display(Name = "International Dialing Format")]
        public string PhoneNumberFormatted { get; set; }

        [Display(Name = "Mobile Dialing Format")]
        public string PhoneNumberMobileDialing { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}