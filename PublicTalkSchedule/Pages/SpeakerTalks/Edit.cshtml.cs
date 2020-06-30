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

namespace PublicTalkSchedule.Pages.SpeakerTalks
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // declare variables
        public SelectList TalkNumList { get; set; }

        public int SelectedTalkNumber { set; get; }                 
        public List<SelectListItem> TalkNumberItems { set; get; }      



        [BindProperty]
        public SpeakerTalk SpeakerTalk { get; set; }

        // ***** success message *****
        [TempData]
        public string Message { get; set; }

        ////public int talkNum { get; set; }



        //On Get
        public async Task OnGetAsync(int spkId, int? id)      //**********spkId = SpeakeID; id = the transaction id number
        {           
            SpeakerTalk = await _db.SpeakerTalk.FirstOrDefaultAsync(m => m.Id == id);
               

            IQueryable<int> talknumQuery = from s in _db.TalkList
                                           orderby s.Id
                                           select s.Id;

            //TalkNumList = new SelectList(await talknumQuery.ToListAsync());   
            ViewData["ID"] = new SelectList(_db.TalkList, "Id", "Id");
        }


        //OnPost
        public async Task<IActionResult> OnPostAsync()
        {
            
            //get record to be updated from databased based on transaction I
            var spktalkFromDB = await _db.SpeakerTalk.FirstOrDefaultAsync(s => s.Id == SpeakerTalk.Id);

            //get TalkTitle in TalkList table based on TalkNum (talk number) in SpeakerTalk table 
            var talktitleQuery = from n in _db.TalkList
                                 where n.Id == SpeakerTalk.TalkNum
                                 select n.Title;

            string talktitle = talktitleQuery.First();


            //update TalkNum and Title only
            spktalkFromDB.TalkNum = SpeakerTalk.TalkNum;
            spktalkFromDB.Title = talktitle;

            await _db.SaveChangesAsync();

            Message = "Talk #" + SpeakerTalk.TalkNum + " updated successfully";

            return RedirectToPage("./Index", new { spkId = SpeakerTalk.SpkNum });
        }
    }
}