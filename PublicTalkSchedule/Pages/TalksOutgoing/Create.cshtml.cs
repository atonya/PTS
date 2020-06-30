using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Word;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly PublicTalkSchedule.Data.ApplicationDbContext _db;

        public CreateModel(PublicTalkSchedule.Data.ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ScheduleOut ScheduleOut { get; set; }


        // declare search variables
        public SelectList SpkName { get; set; }
        public int SpkId { get; set; }

        [Required(ErrorMessage = "The Year field is required... ")]
        public DateTime TalkDate { get; set; }
        
        public int SelectedTalkId { set; get; }
        public List<SelectListItem> TalkItems { set; get; }

        // ***** success message - displays message from create, edit or delete page *****
        [TempData]
        public string Message { get; set; }

        // ***** error message - displays message from create, edit or delete page *****
        [TempData]
        public string ErrorMessage { get; set; }


        //************************* OnGet *************************
        public async Task<IActionResult> OnGetAsync(int? transId, int spkId, int hosconID, DateTime tDate)
        {
            //display record based on its Id (aka transId). if Id is null open new record
            ScheduleOut = await _db.ScheduleOut.FirstOrDefaultAsync(m => m.Id == transId);

            //popluate speaker SelectList based on congregation number
            ViewData["SpkId"] = new SelectList(_db.Speaker.Where(c => c.CongId == 910588).OrderBy(c => c.spkLastName)
                                                          .ThenBy(c => c.spkFirstName), "Id", "FullName");
            
            //populate speakers talklist SelectList based spkId (aka SpkNum or spkId                        
            ViewData["TalkId"] = new SelectList(_db.SpeakerTalk.Where(c => c.SpkNum == spkId)
                                                               .OrderBy(c => c.TalkNum), "TalkNum", "TalkFullTitle");

            //populate host congregation SelectList based on Id (aka CongId)
            ViewData["CongId"] = new SelectList(_db.Congregation.OrderBy(c => c.CongName), "Id", "CongName");

            
            return Page();
        }


        //******************************* OnPost ********************
        public async Task<IActionResult> OnPostAsync()
        {
            //query to get the last transaction id (transId) used in ScheduleOut table
            IQueryable<int> transIdQuery = from s in _db.ScheduleOut
                                           orderby s.Id descending
                                           select s.Id;

            int transId = transIdQuery.FirstOrDefault();
                        
            var spkId = ScheduleOut.SpkNum;



            DateTime temp;
            if (DateTime.TryParse(Request.Form["ScheduleOut.DOT"], out temp))
            {
                // if date is valid continue method
            }
            else
            {
                // if the date is not valid display error message, quit method and return to the Create page
                ErrorMessage = "The Talk Date field is required.  Enter a date ";
                return RedirectToPage("Create");
            }
            // set value of tDate
            DateTime tDate = Convert.ToDateTime(Request.Form["ScheduleOut.DOT"]);





            //get Speaker name ;
            var spkNameQuery = from n in _db.Speaker
                               where n.Id == spkId
                               select n.spkFirstName + " " + n.spkLastName;

            var spkName = spkNameQuery.First();


            //get record to be updated from databased based on transaction Id
            var selectInFromDB = await _db.ScheduleOut.FirstOrDefaultAsync(s => s.Id == transId);

            //update record by column
            ////selectInFromDB.DOT = Convert.ToDateTime(Request.Form["TalkDate"]);               ////DateTime.Parse(Request.Form["ScheduleOut.DOT"]);
            selectInFromDB.DOT = tDate;
            selectInFromDB.SpkNum = spkId;
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


            Message = "The scheduled for   " + spkName + " dated " + String.Format("{0:MMM. d, yyyy}",
                ScheduleOut.DOT.Date) + "   has been created successfully";

            return RedirectToPage("./Index", new { id = ScheduleOut.Id });
        }

        
        //******************* OnPost - Get SpkId (speaker number) ********************
        public async Task<IActionResult> OnPostGetSpkNum(int? transId)  
        {
            DateTime temp;
            if (DateTime.TryParse(Request.Form["ScheduleOut.DOT"], out temp))
            {
                // if date is valid continue method
            }
            else
            {
                // if the date is not valid display error message, quit method and return to the Create page
                ErrorMessage = "The Talk Date field is required.  Enter a date ";
                return RedirectToPage("Create");
            }          
            // set value of tDate
            DateTime tDate = Convert.ToDateTime(Request.Form["ScheduleOut.DOT"]);


            //copy speaker's number from selectlist
            string spkId = (Request.Form["ScheduleOut.SpkNum"]);

            ////DateTime tDate = Convert.ToDateTime(Request.Form["ScheduleOut.DOT"]);

            if (spkId == "")
            {
                ErrorMessage = "To filter by speaker, you must select a name...";
            }

                       
            //update(save) all fields in ScheduleOut table of Db.  at this point only DOT and SpkNum should have values
            _db.ScheduleOut.Add(ScheduleOut);
            await _db.SaveChangesAsync();

            
            //query to get the last transaction id (transId) used in ScheduleOut table
            IQueryable<int> transIdQuery = from s in _db.ScheduleOut
                                           orderby s.Id descending
                                           select s.Id;

            transId = transIdQuery.FirstOrDefault();

            return RedirectToPage("Create", new {transId = transId, congId = 910588, spkId = spkId, tDate = tDate });
        }

    }
}