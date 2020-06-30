using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models
{
    public class ScheduleIn
    {
        [Key]
        public int Id { get; set; }

        // formated as a date (ie 
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        [Display(Name = "Talk Date:")]
        public DateTime DOT { get; set; }


        // speaker informaion
            [Display(Name = "Speaker #:")]
        public int? SpkNum { get; set; }
            [Display(Name = "Speaker:")]
        public string SpeakerName { get; set; }
            [Display(Name = "Congregation:")]
        public string SpkCongName { get; set; }               
            [Display(Name = "Cong #:")]
        public int? SpkCongNum { get; set; }

        // talk information
            [Display(Name = "Talk #:")]
        public int? SpkTalkNum { get; set; }
            [Display(Name = "Title:")]
        public string Title { get; set; }

        // foreign keys
        [ForeignKey("SpkNum")]
        public virtual Speaker Speaker { get; set; }
        
    }
}
