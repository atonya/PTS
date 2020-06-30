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

namespace PublicTalkSchedule.Pages.Speakers
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
        public Speaker Speaker { get; set; }


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

            Speaker = await _db.Speaker.FirstOrDefaultAsync(m => m.Id == id);

            if (Speaker == null)
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

            Speaker = await _db.Speaker.FindAsync(id);

            if (Speaker != null)
            {
                _db.Speaker.Remove(Speaker);
                await _db.SaveChangesAsync();

                Message = "The Speaker " + Speaker.FullName + " deleted successfully";
            }

            return RedirectToPage("./Index");

        }
    }
}