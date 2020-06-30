using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.SpeakerTalks
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public SpeakerTalkViewModel SpeakerTalkVM { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        
        // ***** success message - displays message from create, edit or delete page *****
        [TempData]
        public string Message { get; set; }

        // ***** error message - displays message from create, edit or delete page *****
        [TempData]
        public string ErrorMessage { get; set; }



        //On Get
        public async Task<IActionResult> OnGet(int spkId )
        {
            
            SpeakerTalkVM = new SpeakerTalkViewModel()
            {
                SpeakerTalks = _db.SpeakerTalk.Where(c => c.SpkNum == spkId).OrderBy(c => c.TalkNum).ToList(),
                SpeakerObj = _db.Speaker.FirstOrDefault(s => s.Id == spkId),
            };

            
            return Page();
        }





        //OnPost - Delete
        // rather than using the Delete Razor Page the following is used to delete a record
        public async Task<IActionResult> OnPostDelete(int spkId, int? id=null)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get talk's transaction ID in SpeakerTalk table, used to find record to be delete
            var talk = await _db.SpeakerTalk.FindAsync(id);

            //get TalkNum in SpeakerTalk table 
            var talknumQuery = from n in _db.SpeakerTalk
                               where n.Id == id
                               select n.TalkNum;

            int talknum = talknumQuery.First();

            _db.SpeakerTalk.Remove(talk);
            await _db.SaveChangesAsync();
            Message = "Talk # " + talknum + " deleted successfully";

            
            return RedirectToPage("Index", new { spkId = spkId });
        }


    }
}