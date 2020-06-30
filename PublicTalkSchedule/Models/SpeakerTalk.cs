using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models
{
    public class SpeakerTalk
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Speaker #:")]
        public int SpkNum { get; set; }

        [Required]
        [Display(Name = "Talk #:")]
        public int TalkNum { get; set; }

        [Required]
        [Display(Name = "Title:")]
        public string Title { get; set; }

        [NotMapped]
        [Display(Name = "")]
        public string TalkFullTitle { get { return string.Concat(TalkNum + " - " + Title); } }  //used to display talknum and title in a selectlist

        [ForeignKey("SpkNum")]
        public virtual Speaker Speaker { get; set; }

        [ForeignKey("TalkNum")]
        public virtual TalkList Talk { get; set; }

    }
}
