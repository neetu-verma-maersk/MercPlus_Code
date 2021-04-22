using MercPlusClient.Areas.ManageUser.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageCountryModel
    {
        [Key]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Country")]
        [Display(Name = "Please select Country")]
        public string CountryDescriptions { get; set; }

        [Required(ErrorMessage = "Country")]
        [Display(Name = "Please select Country")]
        public string CountryCodeAndDescription { get; set; }


        public virtual ICollection<ManageUserModel> ManageUserModel { get; set; }
    }
}