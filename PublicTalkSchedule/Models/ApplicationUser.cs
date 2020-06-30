using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models
{
    public class ApplicationUser : IdentityUser
    {
            [Required]
            [Display(Name = "Full Name:")]
        public string Name { get; set; }
       
            [Display(Name = "Speaker #:")]
        public int? SpkNum { get; set; }
    }
}
