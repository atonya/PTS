using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models
{

    //THIS TABLE IS NOT USED AT THIS TIME.  THE CLASS (FILE) MAY BE DELETED IN THE FUTURE

    public class SpkTalkList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Speaker #:")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Talk #:")]
        public int TalkNum { get; set; }

        [ForeignKey("SpkId")]
        public virtual Speaker Speaker { get; set; }

        [ForeignKey("TalkId")]
        public virtual TalkList TalkList { get; set; }


    }
}
