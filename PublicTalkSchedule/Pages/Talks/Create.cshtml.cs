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
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.Talks
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public TalkList TalkList { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // ***** success message *****
        [TempData]
        public string Message { get; set; }



        // selectlist used to show and select Category number (aka Id or Cat #).  selectlist for options from a (TalkCategory) database table
        [Required(ErrorMessage = " The Category field is required ")]
        public int SelectedCatId { get; set; }                   
        public List<SelectListItem> CatItems { get; set; }



        //OnGet
        public IActionResult OnGet()
        {
            //query to populate selectlist for CatId (aka CatNum)
            CatItems = _db.TalkCategory.OrderBy(c => c.Id)
                            .Select(c => new SelectListItem
                            {
                                Value = c.Id.ToString(),
                                Text = c.CatDes
                            })
                                .ToList();

            return Page();
        }


        //OnPost
        public async Task<IActionResult> OnPostAsync()
        {
            //validate required fields are populatd
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //determine if the talk number is already in the database
            var NewTalkNum = TalkList.Id;                   //get the talk number (Id) that was entered for the new talk

            var TalkIdQuery = from u in _db.TalkList.AsNoTracking()     //Seacch the db for a talk with an ID that matches NewTalkNum
                              where u.Id == NewTalkNum
                              select u.Id;

            int talkIdCount = await TalkIdQuery.CountAsync();           //Determine how many records were returned, expecting 0


            if (talkIdCount < 1)                                        // if less than 1 (aka 0) add new talk
            {
                //get CatNum from page, query talkcategory table for description (CatDes)
                var catId = Int32.Parse(Request.Form["SelectedCatId"]);

                var catDescriptionQuery = from d in _db.TalkCategory.AsNoTracking()
                                          where d.Id == catId
                                          select d.CatDes;

                var catDec = catDescriptionQuery.First();

                TalkList.CatDescription = catDec;           //set value of CatDescription field

                _db.TalkList.Add(TalkList);
                await _db.SaveChangesAsync();

                Message = "Talk #" + TalkList.Id + " created successfully";

                return RedirectToPage("Index");                         //return to Index page
            }

            if (talkIdCount > 0)                                        //if 1 or more, new talk cannot be added
            {             
                Message = "ERROR! Talk #" + TalkList.Id + " Was NOT successfully added. The Talk number is being used. Confirm the correct Talk Number";

                return Page();
             }

                return RedirectToPage("Index");
        }
    }
}