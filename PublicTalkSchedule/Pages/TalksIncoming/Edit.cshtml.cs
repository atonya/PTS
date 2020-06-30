using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;

namespace PublicTalkSchedule.Pages.TalksIncoming
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public SchedIncomingViewModel SchedIncomingVM { get; set; }
        public string Label { get; set; }
        public string Handler { get; set; }

        // ***** success message *****
        [TempData]
        public string Message { get; set; }


        public int SelectedCongId { set; get; }                 
        public List<SelectListItem> CongItems { set; get; }     

        public int SelectedSpkId { set; get; }                 
        public List<SelectListItem> SpkItems { set; get; }      

        public int SelectedTalkId { set; get; }                 
        public List<SelectListItem> TalkItems { set; get; }      


        //************ On Get ************
        public async Task<IActionResult> OnGetAsync(int transId, int congId, int spkId, int talkId)
        {        
            SchedIncomingVM = new SchedIncomingViewModel
            {
                ScheduleInObj = await _db.ScheduleIn.FirstOrDefaultAsync(m => m.Id == transId)
            };


            //determine if there is a congregation id (number). if not (congId = null) show all congregations in alphabetical order
            //if so filter using the congreggation id. 
            if (congId == -1)
            {
                ViewData["CongId"] = new SelectList(_db.Congregation.OrderBy(c => c.CongName).Where(c => c.Id == congId), "Id", "CongName");
            }
            else
            {
                ViewData["CongId"] = new SelectList(_db.Congregation.OrderBy(c => c.CongName), "Id", "CongName");
            }

            //if the spkID equals 0 show all talks assigned to speakers in the database.  if spkID has a valid number show all talks 
            //for the selected speaker in talk number order
            if (congId == 0)
            {
                ViewData["SpkId"] = new SelectList(_db.Speaker.OrderBy(c => c.spkLastName), "Id", "FullName");
            }
            else
            {
                ViewData["SpkId"] = new SelectList(_db.Speaker.OrderBy(c => c.spkLastName).Where(c => c.CongId == congId), "Id", "FullName");
            };

            //if the talkID equals 0 show all talks assigned to speakers in the database.  if spkID has a valid number show all talks 
            //for the selected speaker in talk number order
            if (spkId == 0)
            {
                ViewData["TalkId"] = new SelectList(_db.SpeakerTalk.OrderBy(c => c.TalkNum), "TalkNum", "TalkNum");
            }
            else
            {
                ViewData["TalkId"] = new SelectList(_db.SpeakerTalk.OrderBy(c => c.TalkNum).Where(c => c.SpkNum == spkId), "TalkNum", "TalkNum");
            };


            return Page();
        }



        //************ OnPost ************
        public async Task<IActionResult> OnPostAsync()
        {
            string spkName = null;
            string talkTitle = null;
            string congName = null;

            //get values in each editable field, set as a vaiable
            var sunDate = DateTime.Parse(Request.Form["SchedIncomingVM.ScheduleInObj.DOT"]);
            int? congId = string.IsNullOrEmpty(Request.Form["SchedIncomingVM.ScheduleInObj.SpkCongNum"]) 
                ? (int?)null : int.Parse(Request.Form["SchedIncomingVM.ScheduleInObj.SpkCongNum"]);
            int? spkId = string.IsNullOrEmpty(Request.Form["SchedIncomingVM.ScheduleInObj.SpkNum"]) 
                ? (int?)null : int.Parse(Request.Form["SchedIncomingVM.ScheduleInObj.SpkNum"]);
            int? talkId = string.IsNullOrEmpty(Request.Form["SchedIncomingVM.ScheduleInObj.SpkTalkNum"]) 
                ? (int?)null : int.Parse(Request.Form["SchedIncomingVM.ScheduleInObj.SpkTalkNum"]);

            if (spkId != null) 
            { 
                //get SpkName in Speaker table based on SpkId (talk number) in Speaker table 
                var spkNameQuery = from n in _db.Speaker
                                   where n.Id == spkId
                                   select n.FullName;

                spkName = spkNameQuery.First();
            }

            if (talkId != null)
            {            
                //get talktitle in TalkList table based on TalkNum (talk number) 
                var talktitleQuery = from n in _db.TalkList
                                     where n.Id == talkId
                                     select n.Title;

                talkTitle = talktitleQuery.First();
            }

            if(congId != null)
            { 
                //get SpkCongName in Congregation table based on congID 
                var congNameQuery = from c in _db.Congregation
                                    where c.Id == congId
                                    select c.CongName;

                congName = congNameQuery.First();
            }


            //get record to be updated from databased based on transaction Id
            var selectInFromDB = await _db.ScheduleIn.FirstOrDefaultAsync(s => s.Id == SchedIncomingVM.ScheduleInObj.Id);

            //update record by column
            selectInFromDB.SpkNum = spkId;
            selectInFromDB.SpeakerName = spkName;
            selectInFromDB.SpkTalkNum = talkId;
            selectInFromDB.Title = talkTitle;
            selectInFromDB.SpkCongNum = congId;
            selectInFromDB.SpkCongName = congName;

            await _db.SaveChangesAsync();
            

            Message = "The scheduled for   " + String.Format("{0:MMM. d, yyyy}",SchedIncomingVM.ScheduleInObj.DOT.Date) + "   has been updated successfully";

            return RedirectToPage("./Index", new { id = SchedIncomingVM.ScheduleInObj.Id });
        }


        //************ OnPost - Get CongId (congregation number)1 ************
        public async Task<IActionResult> OnPostGetCongNum(int? id)   //id = transaction Id
        {

            var congId = Int32.Parse(Request.Form["SchedIncomingVM.ScheduleInObj.SpkCongNum"]);

            //update (save) SpkCongNum in ScheduleIn table of Db
            var scheduleInFromDB = await _db.ScheduleIn.FirstOrDefaultAsync(c => c.Id == id);
            scheduleInFromDB.SpkCongNum = SchedIncomingVM.ScheduleInObj.SpkCongNum;
            await _db.SaveChangesAsync();

            
            return RedirectToPage("Edit", new { transId = id, congId = congId });
        }



        //************ OnPost - Get SpkId (speaker number) ************
        public async Task<IActionResult> OnPostGetSpkNum(int id, int congId)   //id = transaction Id
        {

            var spkId = SchedIncomingVM.ScheduleInObj.SpkNum;

            //update (save) SpkNum in ScheduleIn table of Db
            var scheduleInFromDB = await _db.ScheduleIn.FirstOrDefaultAsync(c => c.Id == id);
            scheduleInFromDB.SpkNum = SchedIncomingVM.ScheduleInObj.SpkNum;
            await _db.SaveChangesAsync();

            
            return RedirectToPage("Edit", new { transId = id, congId = congId, spkId = spkId });
        }



       //************ OnPost - Get TalkId (talk number) ************
       public async Task<IActionResult> OnPostGetTalkNum(int id, int congId)   //id = transaction Id
        {
            var spkId = SchedIncomingVM.ScheduleInObj.SpkNum;
            var talkId = SchedIncomingVM.ScheduleInObj.SpkTalkNum;


            //update (save) TalkNum in ScheduleIn table of Db
            var scheduleInFromDB = await _db.ScheduleIn.FirstOrDefaultAsync(c => c.Id == id);
            scheduleInFromDB.SpkTalkNum = SchedIncomingVM.ScheduleInObj.SpkTalkNum;
            await _db.SaveChangesAsync();

            return RedirectToPage("Edit", new { transId = id, congId = congId, spkId = spkId, talkId = talkId });
        }



        //************ OnPost - FilterReset ************
        public async Task<IActionResult> OnPostFilterReset(int id)   //id = transaction Id
        {
            var scheduleInFromDB = await _db.ScheduleIn.FirstOrDefaultAsync(c => c.Id == id);
            SchedIncomingVM.ScheduleInObj.SpkCongNum = null;
            SchedIncomingVM.ScheduleInObj.SpkNum = null;
            SchedIncomingVM.ScheduleInObj.SpkTalkNum = null;

            int congId = -1;
            int spkId = -1;
            int talkId = -1;

            return RedirectToPage("Edit", new { transId = id, congId = congId, spkId = spkId, talkId = talkId });
        }

    }
}