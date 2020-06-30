using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.SpeakerTalks
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
        public SpeakerTalkViewModel SpeakerTalkVM { get; set; }

        // ***** success message *****
        [TempData]
        public string Message { get; set; }

        public int TalkNum { get; set; }



        // OnGet
        public async Task<IActionResult> OnGet(int spkId, int? talkId)    //spkId = SpeakeID; talkId = the talk number
        {
            SpeakerTalkVM = new SpeakerTalkViewModel
            {
                SpeakerObj = await _db.Speaker.FirstOrDefaultAsync(c => c.Id == spkId)
            };

            // get all TalkID and Title from TalkList table
            List<String> lstTalkInShoppingCart = _db.TalkShoppingCart
                                                    .Include(c => c.TalkList)
                                                    .Where(c => c.TalkId==talkId)
                                                    .Select(c => c.Title)
                                                    .ToList();

            // remove talk numbers from SelectList if talk number has been added to the TalkShoppingCode
            // NOTE - This Query Is Not Working Correctly
            IQueryable<TalkList> ListOfTalks = from s in _db.TalkList
                                               where (!lstTalkInShoppingCart.Contains(s.Title))
                                               select s;

            SpeakerTalkVM.TalkNumList = ListOfTalks.ToList();

           
            // add new talk to TalkShopkingCart, once added to the cart the talks can be saved to the database
            SpeakerTalkVM.TalkShoppingCart = _db.TalkShoppingCart.Include(c => c.Speaker).Where(c => c.SpkId == spkId)
                                                                                         .OrderBy(c => c.TalkId)
                                                                                         .ToList();

            return Page();
        }


        // OnPost
        public async Task<IActionResult> OnPostAsync(int spkId)
        {
            ////if (ModelState.IsValid)
            ////{

                SpeakerTalkVM.TalkShoppingCart = _db.TalkShoppingCart.Include(c => c.Speaker).Where(c => c.SpkId == SpeakerTalkVM.SpeakerObj.Id).ToList();

                foreach (var talk in SpeakerTalkVM.TalkShoppingCart)
                {
                    SpeakerTalk speakerTalk = new SpeakerTalk
                    {
                        //Id = talk.Id,
                        SpkNum = talk.SpkId,
                        TalkNum = talk.TalkId,
                        Title = talk.Title
                    };

                    _db.SpeakerTalk.Add(speakerTalk);

                }

                _db.TalkShoppingCart.RemoveRange(SpeakerTalkVM.TalkShoppingCart);       //remove records from talkShoppkingCart
                await _db.SaveChangesAsync();                                           //copy new talks to TalkList table

                return RedirectToPage("../Speakers/Index", new { Id = SpeakerTalkVM.SpeakerObj.Id });


            ////}
            ////return Page();
        } 



        // OnPost - Add to Cart
        public async Task<IActionResult> OnPostAddToCart()
        {
            // get talk titled based on TalkId
            var talkTitleQuery = from n in _db.TalkList.AsNoTracking()
                                 where n.Id == SpeakerTalkVM.TalkObj.Id
                                 select n.Title;

            string talktitle = await talkTitleQuery.FirstAsync();

            // set variables to selected values from speaker and talk tables
            TalkShoppingCart objTalkCart = new TalkShoppingCart()
            {
                SpkId = SpeakerTalkVM.SpeakerObj.Id,
                TalkId = SpeakerTalkVM.TalkObj.Id,
                Title = talktitle
            };


            // copy variables to shoppingcart, save and re-direct to create page
            _db.TalkShoppingCart.Add(objTalkCart);
            await _db.SaveChangesAsync();

            return RedirectToPage("Create", new { spkId = SpeakerTalkVM.SpeakerObj.Id} );

        }




        // OnPost - RemoveomCart
        public async Task<IActionResult> OnPostRemoveFromCart(int spkId, int id)   //spkId = SpeakeID; id = transaction Id in shoppingcart
        {
            // get talk's tansaction Id in TalkShoppingCart, find record
            var talk = await _db.TalkShoppingCart.FindAsync(id);

            //get talkId in TalkShoppingCart 
            var talknumQuery = from n in _db.TalkShoppingCart.AsNoTracking()
                                where n.Id == id
                                select n.TalkId;

            int talknum = await talknumQuery.FirstAsync();


            //  remove record from database and save
            _db.TalkShoppingCart.Remove(talk);
            await _db.SaveChangesAsync();

            Message = "Talk # " + talknum + " deleted successfully";


            //return RedirectToPage("Create", new { spkId = SpeakerTalkVM.SpeakerObj.Id, talkId = SpeakerTalkVM.TalkObj.Id });

            return RedirectToPage("Create", new { spkId = SpeakerTalkVM.SpeakerObj.Id });
        }

    }
}