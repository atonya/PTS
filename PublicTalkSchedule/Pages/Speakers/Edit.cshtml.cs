using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace PublicTalkSchedule.Pages.Speakers
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly PublicTalkSchedule.Data.ApplicationDbContext _db;

        public EditModel(PublicTalkSchedule.Data.ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Speaker Speaker { get; set; }

        [Required(ErrorMessage = " The Congregation field is required ")]
        public int SelectedCongId { set; get; }                 //NOTE set; comes before get;
        public List<SelectListItem> CongItems { set; get; }      //NOTE set; comes before get;

        // selectlist for AppointeAs, used to capture speaker as elder or MS. selectlist with static options
        public SelectList AppointedAs { get; set; }


        // ***** success message *****
        [TempData]
        public string Message { get; set; }


        //OnGet
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Speaker = await _db.Speaker
                .Include(s => s.Congregation).FirstOrDefaultAsync(m => m.Id == id);

            if (Speaker == null)
            {
                return NotFound();
            }


            ViewData["CongId"] = new SelectList(_db.Congregation.OrderBy(c => c.CongName), "Id", "CongName");
            
            ViewData["EldMs"] = new SelectList(_db.Speaker, "Id", "EldMs");
            return Page();
        }



        //OnPost
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            //get the congName based on the CongID which is captured in the CongId SelectList.
            // set congregation name (Speake.CongName) to variable congName
            var congID = Speaker.CongId;                                        //get congrgation Id

            var congNameQuery = from n in _db.Congregation.AsNoTracking()       //query database fo congregation based 
                                where n.Id == congID
                                select n.CongName;
                
            string congName = await congNameQuery.SingleAsync();                //excute query
            
            Speaker.CongName = congName;                                        //set congregation (Speaker.CongName)
           

            //update (save) record
           ////// _db.Attach(Speaker).State = EntityState.Modified;

            try
            {
                ////await _db.SaveChangesAsync();

                //update database by column
                var speakerFromDB = await _db.Speaker.FirstOrDefaultAsync(s => s.Id == Speaker.Id);
                ////speakerFromDB.Id = Speaker.Id;
                speakerFromDB.spkLastName = Speaker.spkLastName;
                speakerFromDB.spkFirstName = Speaker.spkFirstName;
                speakerFromDB.spkMobilePhone = Speaker.spkMobilePhone;
                speakerFromDB.spkHomePhone = Speaker.spkHomePhone;
                speakerFromDB.spkEmail = Speaker.spkEmail;
                speakerFromDB.EldMs = Speaker.EldMs;
                speakerFromDB.CongId = Speaker.CongId;
                speakerFromDB.CongName = Speaker.CongName;

                await _db.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeakerExists(Speaker.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        

        private bool SpeakerExists(int id)
        {
            return _db.Speaker.Any(e => e.Id == id);
        }
    }
}
