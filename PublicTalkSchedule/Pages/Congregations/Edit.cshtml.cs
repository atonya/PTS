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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Congregation Congregation { get; set; }


        // ***** success message *****
        [TempData]
        public string Message { get; set; }

        //used to have day of week appear in SelectList (aka dropdown)
        public DayOfWeek DayOfWeek { get; set; }
       

        //OnGet
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Congregation = await _db.Congregation.FirstOrDefaultAsync(m => m.Id == id);

            if (Congregation == null)
            {
                return NotFound();
            }
            

            return Page();
        }



        //OnPost
        public async Task<IActionResult> OnPostAsync(int DayOfWeek)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }



            //update database by column
            var congFromDB = await _db.Congregation.FirstOrDefaultAsync(s => s.Id == Congregation.Id);
            //congFromDB.Id = Congregation.Id;
            congFromDB.CircuitNum = Congregation.CircuitNum;
            congFromDB.CongName = Congregation.CongName;
            congFromDB.khAddress = Congregation.khAddress;
            congFromDB.khCity = Congregation.khCity;
            congFromDB.khPhone = Congregation.khPhone;
            congFromDB.tcLastName = Congregation.tcLastName;
            congFromDB.tcFirstName = Congregation.tcFirstName;
            congFromDB.tcMobilePhone = Congregation.tcMobilePhone;
            congFromDB.tcHomePhone = Congregation.tcHomePhone;
            congFromDB.tcEmail = Congregation.tcEmail;
            congFromDB.cobeLastName = Congregation.cobeLastName;
            congFromDB.cobeFirstName = Congregation.cobeFirstName;
            congFromDB.cobeMobilePhone = Congregation.cobeMobilePhone;
            congFromDB.cobeHomePhone = Congregation.cobeHomePhone;
            congFromDB.MtgDay = Congregation.MtgDay;
            congFromDB.MtgTime = Congregation.MtgTime;         

            await _db.SaveChangesAsync();


            Message = "The Congregation " + Congregation.CongName + " updated successfully";

            return RedirectToPage("./Index");
        }

    }
}