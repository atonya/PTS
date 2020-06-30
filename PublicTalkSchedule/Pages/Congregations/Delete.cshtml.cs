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

namespace PublicTalkSchedule.Pages.Congregations
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
        public Congregation Congregation { get; set; }


        // ***** success message *****
        [TempData]
        public string Message { get; set; }



        //OGet
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Congregation = await _db.Congregation.FirstOrDefaultAsync(m => m.Id == id);

            Congregation = await _db.Congregation.FirstOrDefaultAsync(m => m.Id == id);

            if (Congregation == null)
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

            Congregation = await _db.Congregation.FindAsync(id);

            if (Congregation != null)
            {
                _db.Congregation.Remove(Congregation);
                await _db.SaveChangesAsync();

                Message = "The congregation " + Congregation.CongName + " has been deleted successfully";
            }

            return RedirectToPage("./Index");

        }




    }
}