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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PublicTalkSchedule.Pages.Speakers
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        
        [BindProperty]
        public SpeakerCongregationViewModel SpeakerCongregationVM { get; set; }
        
        // selectlist used to show and select CongNum (aka Id or CongID).  selectlist for options from a database table
        [Required(ErrorMessage =" The Congregation field is required ")]
        public int SelectedCongId { set; get; }                 //NOTE set; comes before get;
        public List<SelectListItem> CongItems { set; get;}      //NOTE set; comes before get;

        // selectlist for AppointeAs, used to capture speaker as elder or MS. selectlist with static options
        public SelectList AppointedAs { get; set; }


        // ***** success message *****
        [TempData]
        public string Message { get; set; }



        //OnGet
        public IActionResult OnGet()
        {
            //query to populate selectlist for elders and ministerial servants (aka EldMs or appointed as)
            AppointedAs = new SelectList(_db.Speaker, nameof(SpeakerCongregationVM.SpkList.Id), nameof(SpeakerCongregationVM.SpkList.EldMs));

            //query to populate selectlist for CongId (aka CongNum)
            CongItems = _db.Congregation.OrderBy(c => c.CongName)
                            .Select(c => new SelectListItem {
                                Value = c.Id.ToString(),
                                Text = c.CongName })
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

            //assign selectlist value to colume (EldMs) on form before saving to database 
            SpeakerCongregationVM.SpkList.EldMs = Request.Form["AppointedAs"];


            //get the congName based on the CongID which is captured in the CongId SelectList.
            // set congregation name (Speake.CongName) to variable congName
            var congID = Int32.Parse(Request.Form["SelectedCongId"]);           //get conggrgation Id

            var congNameQuery = from n in _db.Congregation.AsNoTracking()       //query database fo congregation based 
                                where n.Id == congID
                                select n.CongName;

            string congName = await congNameQuery.SingleAsync();                //excute query


            SpeakerCongregationVM.SpkList.CongId = congID;
            SpeakerCongregationVM.SpkList.CongName = congName;                  //set congregation (Speaker.CongName)


            // save record
            _db.Speaker.Add(SpeakerCongregationVM.SpkList);
            await _db.SaveChangesAsync();


            Message = "The speaker " + SpeakerCongregationVM.SpkList.FullName + " has been successfully aded to the database";
            
            return RedirectToPage("Index");                         //return to Index page

        }            
    }
}