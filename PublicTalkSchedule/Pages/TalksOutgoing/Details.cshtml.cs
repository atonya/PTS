using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DetailsModel : PageModel
    {
        private readonly PublicTalkSchedule.Data.ApplicationDbContext _db;

        public DetailsModel(PublicTalkSchedule.Data.ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ScheduleOut ScheduleOut { get; set; }


        // ***** success message *****
        [TempData]
        public string Message { get; set; }




        //************ OnGet ************
        public async Task<IActionResult> OnGetAsync(int? transId)
        {
            if (transId == null)
            {
                return NotFound();
            }

            ScheduleOut = await _db.ScheduleOut
                .Include(s => s.Congregation)
                .Include(s => s.Speaker)
                .Include(s => s.TalkList)
                .FirstOrDefaultAsync(m => m.Id == transId);

            if (ScheduleOut == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}