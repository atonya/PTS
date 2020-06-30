using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.TalksOutgoing
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly PublicTalkSchedule.Data.ApplicationDbContext _db;

        [BindProperty]
       //// public SchedOutgoingViewModel SchedOutgoingVM { get; set; }
        public ScheduleOut ScheduleOut { get; set; }

        public EditModel(PublicTalkSchedule.Data.ApplicationDbContext db)
        {
            _db = db;
        }

        // declare search variables
        public SelectList SpkName { get; set; }
        public int SpkId { get; set; }
        public DateTime TalkDate { get; set; }
        public int SelectedTalkId { set; get; }
        public List<SelectListItem> TalkItems { set; get; }


        // ***** success message - displays message from create, edit or delete page *****
        [TempData]
        public string Message { get; set; }

        // ***** error message - displays message from create, edit or delete page *****
        [TempData]
        public string ErrorMessage { get; set; }


        //******************** OnGet ********************
        public async Task<IActionResult> OnGetAsync(int? transId, int? spkId, DateTime tDate)
        {
            //determine if there is a talk date enterd
            if (tDate == DateTime.MinValue)
            {
                //get DOT in ScheuleOut table based on transI 
                var dotQuery = from c in _db.ScheduleOut
                                    where c.Id == transId
                                    select c.DOT;

                TalkDate = dotQuery.First();                              
            }
            else
            {
                TalkDate = tDate.Date;
            }


            //populate SpkId, TalkId and CongId SelestList controls
            ViewData["SpkId"] = new SelectList(_db.Speaker.Where(c => c.CongId == 910588).OrderBy(c => c.spkLastName)      
                                                          .ThenBy(c => c.spkFirstName), "Id", "FullName");

           ViewData["TalkId"] = new SelectList(_db.SpeakerTalk.Where(c => c.SpkNum == spkId)
                                                               .OrderBy(c => c.TalkNum), "TalkNum", "TalkFullTitle");

            ViewData["CongId"] = new SelectList(_db.Congregation.OrderBy(c => c.CongName), "Id", "CongName");



            ////SchedOutgoingVM = new SchedOutgoingViewModel
            ////{
            ScheduleOut = await _db.ScheduleOut.FirstOrDefaultAsync(m => m.Id == transId);
            ////};



            return Page();
        }



        //******************** OnPost ********************
        public async Task<IActionResult> OnPostAsync()
        {
            var spkId = ScheduleOut.SpkNum;
            var tDate = Convert.ToDateTime(Request.Form["TalkDate"]);


            //used to set value in date field
            /////string talkDate = Request.Form["ScheduleOut.DOT"];

            //get Speaker name 
            var spkNameQuery = from n in _db.Speaker
                                 where n.Id == spkId
                                 select n.spkFirstName + " " + n.spkLastName;

            var spkName = spkNameQuery.First();


            //get record to be updated from databased based on transaction Id
            var selectInFromDB = await _db.ScheduleOut.FirstOrDefaultAsync(s => s.Id == ScheduleOut.Id);

            //update record by column
            selectInFromDB.DOT = Convert.ToDateTime(Request.Form["TalkDate"]);               ////DateTime.Parse(Request.Form["ScheduleOut.DOT"]);
            selectInFromDB.SpkNum = ScheduleOut.SpkNum;
            selectInFromDB.SpkTalkNum = ScheduleOut.SpkTalkNum;
            
            //add host congregation only if a selection is made
            if (Request.Form["ScheduleOut.hostCongNum"] != "")
            {
                selectInFromDB.hostCongNum = int.Parse(Request.Form["ScheduleOut.hostCongNum"]);
            }
            else
            {
                ErrorMessage = "The Host Congregation was not selected ";
            }




            await _db.SaveChangesAsync();


            Message = "The scheduled for   " + spkName + " dated "  + String.Format("{0:MMM. d, yyyy}",
                tDate.Date) + "   has been updated successfully";

            return RedirectToPage("./Index", new { id = ScheduleOut.Id});
        }




        //******************** OnPost - Get SpkId (speaker number) ********************
        public async Task<IActionResult> OnPostGetSpkNum(int id)   //id = transaction Id
        {
            //copy speaker's number from selectlist
            string spkId = Request.Form["ScheduleOut.SpkNum"];

            ////var tDate = Convert.ToDateTime(Request.Form["TalkDate"]);

            //get the value in the DOT field
            ////DateTime tDate = Convert.ToDateTime(Request.Form["ScheduleOut.DOT"]);
            var tDate = Request.Form["TalkDate"];

            if (spkId == "")
            {
                ErrorMessage = "To filter by speaker, you must select a name...";
            }

            //update (save) spkNum in ScheduleOut table of Db
            var scheduleOutFromDB = await _db.ScheduleOut.FirstOrDefaultAsync(c => c.Id == id);

            scheduleOutFromDB.DOT = ScheduleOut.DOT;
            scheduleOutFromDB.SpkNum = ScheduleOut.SpkNum;

            await _db.SaveChangesAsync();

            return RedirectToPage("Edit", new { transId = id, congId = 910588, spkId = spkId, tDate = tDate });
        }

    }
}