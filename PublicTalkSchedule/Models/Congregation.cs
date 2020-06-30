using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models
{
    public class Congregation
    {
        [Key]                                                   //id's this column and the primary key for the table
        [DatabaseGenerated(DatabaseGeneratedOption.None)]       //set to none the Db doesnot generate this column's value.  User must add it
        [Display(Name = "Cong. #:")]                            //how the lable will display
        public int Id { get; set; }                             //column name as used in the program

        //****************************** code block not used ******************************
        //key field from the original access database. congnum assigned by the branch
        ////[Required]
        ////[Index(IsUnique = true)]
        ////[Display(Name = "Cong.#")]
        ////public int CongNum { get; set; }

            [Required]
            [Display(Name ="Congregation:")]
        public string CongName { get; set; }

            [Display(Name = "Circuit #:")]
        public string CircuitNum { get; set; }
            [Display(Name = "KH_Address:")]
        public string khAddress { get; set; }
            [Display(Name = "KH_City:")]
        public string khCity { get; set; }
            [StringLength(14, MinimumLength = 14, ErrorMessage = "Required format: (###) ###-####")]    
            [Display(Name = "KH Phone")]
        public string khPhone { get; set; }

            [Display(Name = "tc_Last Name:")]
        public string tcLastName { get; set; }
            [Display(Name = "tc_First Name:")]
        public string tcFirstName { get; set; }
            [StringLength(14, MinimumLength = 14, ErrorMessage = "Required format: (###) ###-####")]    
            [Display(Name = "tc_Mobile:")]
        public string tcMobilePhone { get; set; }
            [StringLength(14, MinimumLength = 14, ErrorMessage = "Required format: (###) ###-####")]
            [Display(Name = "tc_Home:")]
        public string tcHomePhone { get; set; }
            [Display(Name = "tc_Email:")]
        public string tcEmail { get; set; }

            [Display(Name = "cobe_Last Name:")]
        public string cobeLastName { get; set; }
            [Display(Name = "cobe_First Name:")]
        public string cobeFirstName { get; set; }
            [StringLength(14, MinimumLength = 14, ErrorMessage = "Required format: (###) ###-####")]
            [Display(Name = "cobe_Mobile:")]
        public string cobeMobilePhone { get; set; }
            [StringLength(14, MinimumLength = 14, ErrorMessage = "Required format: (###) ###-####")]
            [Display(Name = "cobe_Home:")]
        public string cobeHomePhone { get; set; }

            [Display(Name = "Mtg. Day:")]
        public DayOfWeek MtgDay { get; set; }
            [DisplayFormat(DataFormatString = "{0:t}")]
            [DataType(DataType.Time)]
            [Display(Name = "Mtg. Time:")]
        public DateTime MtgTime { get; set; }
    }
}
