using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models
{
    public class Speaker
    {
        [Key]
        [Display(Name = "Speaker #:")]
        public int Id { get; set; }

        ////public int SpkNum { get; set; }      //key field used in Access database
        
        [Required]
        [Display(Name = "Last Name:")]
        public string spkLastName { get; set; }
        [Required]
        [Display(Name = "First Name:")]
        public string spkFirstName { get; set; }
        [StringLength(14, MinimumLength = 14, ErrorMessage = "Required format: (###) ###-####")]
        [Display(Name = "Mobile Phone:")]
        public string spkMobilePhone { get; set; }
        [StringLength(14, MinimumLength = 14, ErrorMessage = "Required format: (###) ###-####")]
        [Display(Name = "Home Phone:")]
        public string spkHomePhone { get; set; }
        [Display(Name = "Email:")]
        public string spkEmail { get; set; }

        [Display(Name = "E/MS:")]
        public string EldMs {get;set;}

        [NotMapped]
        [Display(Name = "")]
        public string FullName { get { return string.Concat(spkLastName + ", " + spkFirstName); } }

        [Display(Name = "Congregation:")]
        public string CongName { get; set; }

        [Required]
        public int CongId { get; set; }

        [ForeignKey("CongId")]
        public virtual Congregation Congregation { get; set; }

    }
}
