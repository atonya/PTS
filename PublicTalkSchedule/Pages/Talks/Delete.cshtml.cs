using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.Talks
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public TalkList TalkList { get; set; }


        // ***** success message *****
        [TempData]
        public string Message { get; set; }

        //OnGet
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TalkList = await _db.TalkList.FirstOrDefaultAsync(m => m.Id == id);

            if (TalkList == null)
            {
                return NotFound();
            }
            return Page();
        }

        //OnPost
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TalkList = await _db.TalkList.FindAsync(id);

            if (TalkList != null)
            {
                _db.TalkList.Remove(TalkList);
                await _db.SaveChangesAsync();

                Message = "Talk #" + TalkList.Id + " deleted successfully";
            }

            return RedirectToPage("./Index");

        }
    }
}