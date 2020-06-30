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

namespace PublicTalkSchedule.Pages.Talks
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // ***** success message *****
        [TempData]
        public string Message { get; set; }


        [BindProperty]
        public TalkList TalkList { get; set; }

        // selectlist used to show and select Category number (aka Id or Cat #).  selectlist for options from a (TalkCategory) database table
        [Required(ErrorMessage = " The Category field is required ")]
        public int SelectedCatId { get; set; }
        public List<SelectListItem> CatItems { get; set; }



        //OnGet
        public async Task<IActionResult> OnGetAsync(int? id, string CatDescription)
        {
            if (id == null)
            {
                return NotFound();
            }

            TalkList = await _db.TalkList.FirstOrDefaultAsync(m => m.Id == id);

            ViewData["CatId"] = new SelectList(_db.TalkCategory.OrderBy(c => c.Id), "CatDes", "CatDes");

            if (TalkList == null)
            {
                return NotFound();
            }
            return Page();
        }


        //OnPost
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //get value inCatDescription field
            var catId = Request.Form["TalkList.CatDescription"];

            //populate catDescription field using TalkCategory as lookup table  
            var talkFromDB = await _db.TalkList.FirstOrDefaultAsync(s => s.Id == TalkList.Id);

            //talkFromDB.TalkNum = TalkList.TalkNum;  NOT UP DATED
            talkFromDB.Title = TalkList.Title;
            talkFromDB.CatDescription = catId;

            await _db.SaveChangesAsync();

            Message = "Talk #" + TalkList.Id + " updated successfully";

            return RedirectToPage("./Index");
        }

        private bool ServiceTypeExists(int id)
        {
            return _db.TalkList.Any(e => e.Id == id);
        }
    }
}
