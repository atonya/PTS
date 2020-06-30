using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models
{
    public class TalkList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Talk #:")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title:" )]
        public string Title { get; set; }

        [Display(Name = "Category:")]
        public string CatDescription { get; set; }

       
    }
}
