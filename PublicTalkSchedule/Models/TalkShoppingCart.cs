using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models
{
    public class TalkShoppingCart
    {
        public int Id { get; set; }

        [Display(Name = "Speaker #:")]
        public int SpkId { get; set; }
        [Display(Name = "Talk #:")]
        public int TalkId { get; set; }
        [Display(Name = "Title:")]
        public string Title { get; set; }

        [ForeignKey("SpkId")]
        public virtual Speaker Speaker { get; set; }

        [ForeignKey("TalkId")]
        public virtual TalkList TalkList { get; set; }
    }
    
}
