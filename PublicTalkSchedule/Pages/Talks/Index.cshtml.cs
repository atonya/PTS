using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PublicTalkSchedule.Pages.Talks
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class IndexModel : PageModel
    {

        private readonly ApplicationDbContext _db;

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


        
        [BindProperty]
        public TalkListViewModel TalkListVM { get; set; }

        //OnGet
        public async Task<IActionResult> OnGet(int productPage = 1, string searchId = null, string searchTitle = null, string searchCatNum = null)
        {
            TalkListVM = new TalkListViewModel()
            {
                TalkListList = await _db.TalkList.ToListAsync()
            };

            // ******************** code to search (filter) by Id, Title or CatNum ********************
            StringBuilder param = new StringBuilder();
            param.Append("/Talks?productPage=:");           //used to track the talks on a page

            // if a value is in a search append it to the query
            param.Append("&searchId=");           
            if (searchId != null)
            {
                param.Append(searchId);
            }

            param.Append("&searchTitle=");
            if (searchTitle != null)
            {
                param.Append(searchTitle);
            }

            param.Append("&searchCatNum=");
            if (searchCatNum != null)
            {
                param.Append(searchCatNum);
            }

            // logic for search critieria.  if all three text boxes have values search on TalkNum only.  If Title and CatNum have values serach
            // on name.  if only CatNnum, search on CatNum.
            if (searchId != null)
            {
                int varID;
                    bool isNumeric = int.TryParse(searchId, out varID);
                if (isNumeric)
                    {
                        //int varID = Int32.Parse(searchId);
                        TalkListVM.TalkListList = await _db.TalkList.Where(u => u.Id == varID).ToListAsync();
                    }
                else
                    {
                        ErrorMessage = "The Talk# search box must contain a numeric only or be empty!  Enter a number or empty the box to proceed";
                    }
                }
            else
            {
                if (searchTitle != null)
                {
                    TalkListVM.TalkListList = await _db.TalkList.Where(u => u.Title.ToLower().Contains(searchTitle.ToLower())).ToListAsync();
                }
                else
                {
                    if (searchCatNum != null)
                    {
                        TalkListVM.TalkListList = await _db.TalkList.Where(u => u.CatDescription.ToLower().Contains(searchCatNum.ToLower())).ToListAsync();
                    }
                }
            }





            // ******************** code for page pagination ********************
            var count = TalkListVM.TalkListList.Count;                      //determine the numbe of records

            TalkListVM.PagingInfo = new PagingInfo                          //set value fo properties in PageInfo class
            {
                CurrentPage = productPage,
                ItemsPerPage = SD.PaginationTalkListPageSize,
                TotalItems = count,
                UrlParam = param.ToString()
            };

            TalkListVM.TalkListList = TalkListVM.TalkListList.OrderBy(p => p.Id)
                 .Skip((productPage - 1) * Utility.SD.PaginationTalkListPageSize)
                 .Take(SD.PaginationTalkListPageSize).ToList();


            return Page();
        }
    }
}