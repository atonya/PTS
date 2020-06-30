using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.TalksIncoming
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IList<ScheduleIn> ScheduleIn { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        [BindProperty]
        public SchedIncomingViewModel SchedIncomingVM { get; set; }  

        //used to filter records as ALL Dates, Filled Dates, Open Dates
        [BindProperty]
        public string Option { get; set; }
        public string[] Options = new[] { " All", " Filled", " Open" };     //NOTE the space at the beginning of the string



        // ***** success message - displays message from create, edit or delete page *****
        [TempData]
        public string Message { get; set; }

        // ***** error message - displays message from create, edit or delete page *****
        [TempData]
        public string ErrorMessage { get; set; }



        //OnGet
        public async Task OnGetAsync(DateTime sDate, DateTime eDate, string sStatus)
        {
            //determine if startdate has a value greater than 1/1/0001.  if not, set value to today
            if (sDate == DateTime.MinValue)
            {
                StartDate = DateTime.Now.Date;
            }
            else
            {
                StartDate = sDate.Date;
            }

            //determine if enddate has a value equal to 1/1/0001, if so, set value to today + 90 days
            if (eDate == DateTime.MinValue)
            {
                EndDate = DateTime.Now.Date.AddDays(90);
            }
            else
            {
                EndDate = eDate.Date;
            }


            //query database based on startdate and enddate timeframe 
            SchedIncomingVM = new SchedIncomingViewModel();
            {
                ////ScheduleIn = _db.ScheduleIn.ToList();
                ////.Include(s => s.Speaker).FirstOrDefaultAsync();


                if (sStatus == " All")
                {
                    ScheduleIn = await _db.ScheduleIn.Where(s => s.DOT >= StartDate && s.DOT <= EndDate).ToListAsync();
                    Option = sStatus;
                }
                else if (sStatus == " Filled")
                {
                    ScheduleIn = await _db.ScheduleIn.Where(s => s.DOT >= StartDate && s.DOT <= EndDate).Where(r => r.SpkNum > 0).ToListAsync();
                    Option = sStatus;
                }
                else if (sStatus == " Open")
                {
                    ScheduleIn = await _db.ScheduleIn.Where(s => s.DOT >= StartDate && s.DOT <= EndDate).Where(r => r.SpkNum == null).ToListAsync();
                    Option = sStatus;
                }
                else if (sStatus == null)
                {
                    ScheduleIn = await _db.ScheduleIn.Where(s => s.DOT >= StartDate && s.DOT <= EndDate).ToListAsync();
                    Option = " All";
                }
            }


            SchedIncomingVM = new SchedIncomingViewModel()
            {
                ScheduleIn = await _db.ScheduleIn.ToListAsync()
            };

            ;

        }




        //On Post - Requery
        public async Task<IActionResult> OnPostRequery()
        {
                                  
            //get start and end dates form incoming schedule index page
            var sDate = Convert.ToDateTime(Request.Form["StartDate"]);
            var eDate = Convert.ToDateTime(Request.Form["EndDate"]);

            //get schedule status, then select All, Filled or Open schedules based on radio buttons
            var sStatus = Request.Form["Option"];

            if (sDate <= eDate)
            {
                return RedirectToPage("Index", new { sDate = sDate, eDate = eDate, sStatus = sStatus });
            }
            else
            {
                ErrorMessage = "End Date Must be after or on the Start Date...";
                Console.Beep(2000, 250);
                //<img src="~/images/speaker.png" width="40" />

                return RedirectToPage("Index", new { sDate = sDate, eDate = eDate, sStatus = sStatus });
            }

        }





    }
}