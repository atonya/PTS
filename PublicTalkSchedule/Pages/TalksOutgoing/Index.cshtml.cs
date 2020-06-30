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
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.TalksOutgoing
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public SchedOutgoingViewModel SchedOutgoingVM { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }


        public IList<ScheduleOut> ScheduleOut { get; set; }


        // declare search variables
        public SelectList SpkName { get; set; }
        public int SpkId { get; set; }
        public string searchSpkName { get; set; }
        public DateTime StartDate { get; set; }         //date in the past
        public DateTime EndDate { get; set; }           //date in the furthur
               
        // ***** success message - displays message from create, edit or delete page *****
        [TempData]
        public string Message { get; set; }

        // ***** error message - displays message from create, edit or delete page *****
        [TempData]
        public string ErrorMessage { get; set; }



        //********** OnGet **********
        //public void OnGet()
        public async Task OnGetAsync(string spkName)
        {
            //set values fo startdate and enddate
            StartDate = DateTime.Now.Date.AddDays(-45);             //detemine date for 45 days in the past
            EndDate = DateTime.Now.AddDays(183);                    //determine date for six months in the furthur


            // query speaker table to get a list of all speakers from empire ranch ONLY, insert them into the SelectList (SpkID)
            IQueryable<string> spknameQuery = from s in _db.Speaker
                                              where s.CongId == 910588
                                              orderby s.spkLastName, s.spkFirstName
                                              select s.FullName;

            SpkName = new SelectList(await spknameQuery.ToListAsync());



            if (spkName == null)
            {               
                //if a speaker is not select, show all assigned talks
                ScheduleOut = await _db.ScheduleOut.Where(s => s.DOT >= StartDate && s.DOT <= EndDate).OrderBy(o => o.DOT)
                    .Include(s => s.Congregation)
                    .Include(s => s.Speaker)
                    .Include(s => s.TalkList).ToListAsync();
            }
            else
            {
                //if a speaker is selected
                
                //Parse last and first names from searchSpkName.use them in where clause of quey to select speaker
                //this step is required because FullName is [NotMapped] in database
                string lastName = spkName.Substring(0, spkName.IndexOf(','));
                string firstName = spkName.Substring(spkName.LastIndexOf(',') + 2);

                
                //query speaker table to get selected speaker's id (SpkId)
                IQueryable<int> spknumQuery = from s in _db.Speaker
                                              where s.spkLastName == lastName && s.spkFirstName == firstName
                                              orderby s.spkLastName, s.spkFirstName
                                              select s.Id;

                SpkId = spknumQuery.FirstOrDefault();


                //qury to get the last transaction id (transId) used in ScheduleOut table, add to determine what the next number should be
                IQueryable<int> transIdQuery = from s in _db.ScheduleOut
                                               orderby s.Id descending
                                               select s.Id;

                int transId = transIdQuery.FirstOrDefault() + 1; 


                SchedOutgoingVM = new SchedOutgoingViewModel()
                {
                    
                    SpeakerObj = await _db.Speaker.Where(s => s.Id == SpkId).ToListAsync(),
                    ScheduleOutObj = await _db.ScheduleOut.FirstOrDefaultAsync(i => i.Id > transId)
                };

                searchSpkName = spkName;        //reset searchSpkName textbox

                ScheduleOut = await _db.ScheduleOut.OrderBy(o => o.DOT).Where(s => s.DOT >= StartDate && s.DOT <= EndDate).Where(i => i.SpkNum == SpkId)
                    .Include(s => s.Congregation)
                    .Include(s => s.Speaker)
                    .Include(s => s.TalkList).ToListAsync();

            }

        }


            //************ OnPost - Get spkInfo ************
            public async Task<IActionResult> OnPostGetSpkInfo(int id)   
            {
                string spkName = (Request.Form["searchSpkName"]);

                if (spkName == "") 
                {
                    ErrorMessage = "To filter by speaker, you must select a name...";
                }
                
            return RedirectToPage("Index", new {spkName = spkName });
            }

 
    }
}