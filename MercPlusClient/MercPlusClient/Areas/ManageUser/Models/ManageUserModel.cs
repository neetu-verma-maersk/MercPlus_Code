//using MercPlusClient.Areas.ManageMasterData.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageUser.Models
{
    public class ManageUserModel
    {
        public Int32 UserId { get; set; }
        
        [Display(Name = "User Id")]
        public string Login { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }

        [Display(Name = "Location Code")]
        public string Loccd { get; set; }

        [Display(Name = "Active")]
        public string ActiveStatus { get; set; }

        [Display(Name = "Approval Amount")]
        [DataType(DataType.Currency)]
        public decimal ApproveAmount { get; set; }

        [Display(Name = "Email Id")]
        public string EmailId { get; set; }

        [Display(Name = "Expired")]
        public string Expired { get; set; }

        public string CountryCode { get; set; }
        public bool isLocationExist { get; set; }
        [Display(Name = "Country")]
        public string CountryDescriptions { get; set; }
        public string CountryCodeAndDescription { get; set; }

        public string strHtml { get; set; }
        public bool showMsg = false;
        

        public Int32 AuthGroupId { get; set; }
        public string AuthGroupName { get; set; }

        [Display(Name = "Permissions Prefix")]
        public string PermissionsPrefix { get; set; }

        // public virtual ManageCountryModel ManageCountryModel { get; set; }

        public string[] SelectedActivePermissionValues { get; set; }
        public string[] SelectedAvailablePermissionValues { get; set; }

        public List<SelectListItem> ActivePermission = new List<SelectListItem>();
        public List<SelectListItem> AvailablePermission = new List<SelectListItem>();

        public List<SelectListItem> ActiveSecWebSitePermission = new List<SelectListItem>();
        public List<SelectListItem> AssignedSecWebSitePermission = new List<SelectListItem>();


        //  public IEnumerable<string> ActivePermission { get; set; }

        public List<SelectListItem> CountryList = new List<SelectListItem>();

        public List<SelectListItem> SecAuthGroupList = new List<SelectListItem>();

        public bool isCountrySelection { get; set; }
       
        public bool isConfirmUpdate { get; set; }
        public bool isDeleteMode { get; set; }
        public int AvailablePermissionCount { get; set; }

        public bool isAdminUserSelected = false;
        public bool isCPHUserSelected = false;
        public bool isSuccess = false;
        public string strMessage = ""; 
        //  public virtual ICollection<Country> objCountry { get; set; }
        //  public string CountryCode { get; set; }

    }

}