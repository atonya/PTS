using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PublicTalkSchedule.Pages.Speakers
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class DetailModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailModel(ApplicationDbContext db)
        {
            _db = db;
        }


        [BindProperty]
        public Speaker Speaker { get; set; }
        // selectlist used to show and select CongNum (aka Id or CongID).  selectlist for options from a database table
        [Required(ErrorMessage = " The Congregation field is required ")]
        public int SelectedCongId { set; get; }                 //NOTE set; comes before get;
        public List<SelectListItem> CongItems { set; get; }      //NOTE set; comes before get;

        // selectlist for AppointeAs, used to capture speaker as elder or MS. selectlist with static options
        public SelectList AppointedAs { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Speaker = await _db.Speaker.FirstOrDefaultAsync(s => s.Id == id);


            if (Speaker == null)
            {
                return NotFound();
            }


            return Page();
        }
    }
}