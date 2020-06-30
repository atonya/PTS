using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.Congregations
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

       
        [BindProperty]
        public Congregation Congregation { get; set; }
  

        // ***** success message *****
        [TempData]
        public string Message { get; set; }

        // ***** error message - displays message from create, edit or delete page *****
        [TempData]
        public string ErrorMessage { get; set; }



        //used to have day of week appear in SelectList (aka dropdown)
        public DayOfWeek DayOfWeek { get; set; }

        //OnGet
        public IActionResult OnGet()
        {
            return Page();
        }

        
        //OnPost
        public async Task<IActionResult> OnPostAsync()
        {
            //validate required fields are populatd
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //determine if the congregation number is already in the database
            var NewCongNum = Congregation.Id;                       //get the CongNum (Id) that was entered for the new congregation

            var congIdQuery = from u in _db.Congregation.AsNoTracking()     //Seacch the db for a talk with an ID that matches NewTalkNum
                              where u.Id == NewCongNum
                              select u.Id;

            int congIdCount = await congIdQuery.CountAsync();           //Determine how records were returned, expecting 0


            if (congIdCount < 1)                                        // if less than 1 (aka 0) add new talk
            {
                _db.Congregation.Add(Congregation);
                await _db.SaveChangesAsync();

                Message = "The congregation " + Congregation.CongName + " has been successfully adedd to the database";

                return RedirectToPage("Index");                         //return to Index page
            }

            if (congIdCount > 0)                                        //if 1 or more, new talk cannot be added
            {
                ErrorMessage = "ERROR! The congregation " + Congregation.CongName + " was NOT successfully added. The congregation number is being used. Confirm the correct Congregation Number";

                return Page();
            }
                return RedirectToPage("Index");
        }
    }
}