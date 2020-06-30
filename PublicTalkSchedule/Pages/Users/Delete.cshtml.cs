using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.Users
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // ***** success message *****
        [TempData]
        public string Message { get; set; }


        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }

        //**** OnGet ****
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id.Trim().Length == 0)
            {
                return NotFound();
            }

            ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (ApplicationUser == null)
            {
                return NotFound();
            }
            return Page();
        }


        //**** OnPost ****
        public async Task<IActionResult> OnPostAsync()
        {
            //get the user's name
            string userName = Request.Form["ApplicationUser.Name"];
            string TT = ApplicationUser.Name;


            var userInDb = await _db.Users.SingleOrDefaultAsync(u => u.Id == ApplicationUser.Id);

            _db.Users.Remove(userInDb);
            await _db.SaveChangesAsync();

            Message = "User " + userName + " deleted successfully";

            return RedirectToPage("Index");
        }


    }
}