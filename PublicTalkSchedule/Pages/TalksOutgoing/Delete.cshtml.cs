using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DeleteModel : PageModel
    {
        private readonly PublicTalkSchedule.Data.ApplicationDbContext _db;

        public DeleteModel(PublicTalkSchedule.Data.ApplicationDbContext db)
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


        //************ OnPost ************
        public async Task<IActionResult> OnPostAsync(int? transId)
        {
            if (transId == null)
            {
                return NotFound();
            }

            ScheduleOut = await _db.ScheduleOut.FindAsync(transId);

            
            // get speaker's number and name
            var spkId = ScheduleOut.SpkNum;
            var spkTalkNum = ScheduleOut.SpkTalkNum;

            //get Speaker name 
            var spkNameQuery = from n in _db.Speaker
                               where n.Id == spkId
                               select n.spkFirstName + " " + n.spkLastName;

            var spkName = spkNameQuery.First();

            
            //elete record
            if (ScheduleOut != null)
            {
                _db.ScheduleOut.Remove(ScheduleOut);
                await _db.SaveChangesAsync();

                Message = "The scheduled for   " + spkName + " dated " + String.Format("{0:MMM. d, yyyy}", ScheduleOut.DOT.Date) + "   has been deleted successfully";
            }

            return RedirectToPage("./Index");

        }

    }
}