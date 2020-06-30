using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Utility;
using Document = System.Reflection.Metadata.Document;

namespace PublicTalkSchedule.Pages.TalksIncoming
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // ***** success message - displays message from create, edit or delete page *****
        [TempData]
        public string Message { get; set; }

        // ***** error message - displays message from create, edit or delete page *****
        [TempData]
        public string ErrorMessage { get; set; }


        public string lblMessage { get; set; }
        public DateTime currentDate { get; set; }
        public int weekDay { get; set; }
        [Required(ErrorMessage = "The Year field is required... ")]
        public string txtYear { get; set; }
        [Required(ErrorMessage = "The Sunday Date field is required... ")]
        public string txtSunday { get; set; }

        [BindProperty]
        public ScheduleIn scheduleIn { get; set; }


        // OnGet
        public IActionResult OnGet(DateTime sunDate, string newYear)
        {
            //determine if sunDate is a valid Sunday date for the selected year.  if valid, run code. if blank
            //(MinValue equals 01/01/0001 and indicates a blank textbox), skip code. the initialize schedule page appears
            if (sunDate != DateTime.MinValue)   
            {
                txtYear = newYear;

                var strSunday = sunDate.ToString("MM/dd/yyyy");
                txtSunday = strSunday;
            }

            txtYear = newYear;

            return Page();
        }



        // OnPost
        public async Task<IActionResult> OnPostAsync()
        {
            var newYear = Request.Form["txtYear"];                              //get the first Sunday from the page
            DateTime yearEndMarker = Convert.ToDateTime("12/31/" + newYear);    //set marker for the last 

            var newSunday = Request.Form["txtSunday"];                          //get the first Sunday from the page
            DateTime currentSunday = Convert.ToDateTime(newSunday);             //convert newSunday from String to DateTime

           do
            {
                //the next three lines makes each iteration appear as a new record to be saved 
                scheduleIn = new ScheduleIn()
                {
                    DOT = currentSunday
                };

                    _db.ScheduleIn.Add(scheduleIn);
                    await _db.SaveChangesAsync();

                    currentSunday = currentSunday.AddDays(7);

           } while (currentSunday.Date <= yearEndMarker.Date);

            Message = "The year " + newYear + " has been successfully initialized";

            return RedirectToPage("Index");                         //return to Index page
        }



        //On Post - Initialize Year  
        public async Task<IActionResult> OnPostInitializeYear()
        {
            var newYear = Request.Form["txtYear"];      //get the selected year


            //determine if the value on newYear in already in the database
            var yearQuery = _db.ScheduleIn.Where(u => u.DOT.Year == Convert.ToInt32(newYear));      //Seacch the db for records where DOT year = newYear 

            var newYearCount = yearQuery.Count();                               //Determine how records were returned, expecting 0


            if (newYearCount != 0)                                              // in newYearCount is not 0 the year already exist in database 
            {
                ErrorMessage = "ERROR! The year " + newYear + " was NOT successfully initilized.  It was already in the database... ";

                return Page();
            }
            else
            { 
                //add newYear to jan. 1, then convert to datetime. get the day of the week for 1/1/newYear as an integer
                currentDate = Convert.ToDateTime("01/01/" + newYear);
                weekDay = Convert.ToInt32(currentDate.DayOfWeek);

                //determine he first sunday in the selected year.  increase sunDate by 1 until weekDay equals 0 (sunday)
                do
                {
                    currentDate = currentDate.AddDays(1);
                    weekDay = Convert.ToInt32(currentDate.DayOfWeek);
                }

                while (weekDay != 0);
                
                return RedirectToPage("Create", new { sunDate = currentDate, newYear = newYear });
            }
        }

    }
}